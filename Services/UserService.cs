using AutoMapper;
using LifeLongApi.Codes;
using LifeLongApi.Dtos;
using LifeLongApi.Data.Repositories;
using LifeLongApi.Dtos.Response;
using LifeLongApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static LifeLongApi.Codes.AppHelper;

namespace LifeLongApi.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IFollowRepository _followRepo;
        private readonly ITopicService _topicService;
        private readonly IQualificationRepository _qualificationRepo;
        private readonly INotificationService _notificationService;

        private readonly IUserFieldOfInterestRepository _userFieldOfInterestRepo;
        private readonly IWorkExperienceRepository _workExperienceRepo;
        private readonly IMapper _mapper;
        public UserService(UserManager<AppUser> userManager,
                           IAppointmentRepository appointmentRepo,
                           IFollowRepository followRepo,
                           ITopicService topicService,
                           IMapper mapper,
                           IUserFieldOfInterestRepository userFieldOfInterestRepo,
                           IQualificationRepository qualificationRepo,
                           INotificationService notificationService,
                           IWorkExperienceRepository workExperienceRepo)
        {
            _userManager = userManager;
            _followRepo = followRepo;
            _topicService= topicService;
            _mapper = mapper;
            _userFieldOfInterestRepo = userFieldOfInterestRepo;
            _qualificationRepo = qualificationRepo;
            _notificationService = notificationService;
            _workExperienceRepo = workExperienceRepo;
        }
        public async Task<ServiceResponse<RegisterDto>> CreateUserAsync(RegisterDto newUser){
            var sr = new ServiceResponse<RegisterDto>();

            if (!string.IsNullOrWhiteSpace(newUser.TimeZone))
            {
                //validate time zone entered.
                try
                {
                    TimeZoneInfo.FindSystemTimeZoneById(newUser.TimeZone);
                }
                catch
                {
                    sr.HelperMethod(400, "Please select a valid time zone.", false);
                    return sr;
                }
                
            }
            
            var convertedUser = _mapper.Map<AppUser>(newUser);

            var foundUser = await _userManager.FindByEmailAsync(newUser.Email);
            
            if (foundUser != null)
            {
                //the email exist in db.
                sr.Code = 400;
                sr.Success = false;
                sr.Message = "email address already exist.";
            }
            else
            {
                //create the user.
                convertedUser.UserName = newUser.Email;
                var result = await _userManager.CreateAsync(convertedUser, newUser.Password);
                if (result.Succeeded)
                {
                    //associate the user with a role
                    await _userManager.AddToRoleAsync(convertedUser, newUser.Role);
                    sr.Code = 201;
                    sr.Data = newUser;
                    sr.Success = true;
                }
            }
            return sr;
        }

        public async Task<ServiceResponse<UserDto>> UpdateUserAsync(string username, UserDto userCreds)
        {
            //get user
            var foundUser = await _userManager.FindByNameAsync(username);
            var sr = new ServiceResponse<UserDto>();
            //check that user exist
            if (foundUser == null)
            {
                //user does not exist.
                sr.HelperMethod(404, "user not found.", false);
            }
            else
            {
                //var convertedUser = _mapper.Map<AppUser>(userCreds);
                //update the user info.
                foundUser.FirstName = userCreds.FirstName;
                foundUser.LastName = userCreds.LastName;
                foundUser.Email = userCreds.Email;
                foundUser.PhoneNumber = userCreds.PhoneNumber;
                foundUser.Address = userCreds.Address;
                foundUser.City = userCreds.City;
                foundUser.State = userCreds.State;
                foundUser.Country = userCreds.Country;

                var result = await _userManager.UpdateAsync(foundUser);
                if (result.Succeeded)
                {
                    //return userInfo
                    sr.Code = 200;
                    sr.Data =  GetUserByUsernameWithRelationshipsAsync(username).Result.Data;
                    sr.Success = true;
                }
            }
            return sr;
        }

        public async Task<ServiceResponse<AppUser>> GetUserByUsernameAsync(string userName){
            var foundUser = await _userManager.FindByNameAsync(userName);
            var sr = new ServiceResponse<AppUser>();
            if (foundUser != null)
            {
                //the user exist in db.
                sr.Code = 200;
                sr.Success = true;
                sr.Data = foundUser;
            }
            else
            {
                //user does not exist.
                sr.HelperMethod(404, "User not found.", false);
            }
            return sr;
        }

        public async Task<ServiceResponse<UserDto>> GetUserByUsernameWithRelationshipsAsync(string userName)
        {
            var sr = new ServiceResponse<UserDto>();
            //get user
            var user = await GetUserByUsernameAsync(userName);
            if(user.Data == null){
                //user not found
                //return error message gotten
                sr.HelperMethod(404, user.Message, false);
                return sr;
            }

            var userDto = _mapper.Map<UserDto>(user.Data);
            var userFieldOfInterests =  _userFieldOfInterestRepo.GetFieldOfInterestIdsForUser(user.Data.Id);
            //get the user's field of interest names.
            userDto.UserFieldOfInterests = _mapper.Map<List<UserFieldOfInterestDto>>(
                                                                    await _userFieldOfInterestRepo
                                                                    .GetFieldOfInterestsForUserAsync(user.Data.Id)
                                                                );
            userDto.Roles = await _userManager.GetRolesAsync(user.Data) as List<string>;
            userDto.UserWorkExperiences = _mapper.Map<List<WorkExperienceResponseDto>>(
                                                                            await _workExperienceRepo
                                                                            .GetAllForUserAsync(user.Data.Id)
                                                                        );
            userDto.Friends =  GetUserFriendsAsync(userName).Result.Data;
          
            sr.Data = userDto;
            sr.Code = 200;
            sr.Success = true;
            return sr;

        }

        public async Task<ServiceResponse<List<SearchResponseDto>>> GetUsersByInterestSearchResultAsync(string searchString)
        {
            var sr = new ServiceResponse<List<SearchResponseDto>>();

            if(String.IsNullOrWhiteSpace(searchString)){
                sr.HelperMethod(404, "Please enter a search phrase", false);
                return sr;
            }

            //make sure field of interest exists in db.
            var topicCreds = await _topicService.GetFieldOfInterestByNameAsync(searchString);
            if (topicCreds.Data == null)
            {
                sr.HelperMethod(404, topicCreds.Message, false);
                return sr;
            }

            var users = await _userFieldOfInterestRepo.GetUsersByFieldOfInterestAsync(topicCreds.Data.Id);
            var searchResult = _mapper.Map<List<SearchResponseDto>>(users);

            sr.Data = searchResult;
            sr.Code = 200;
            sr.Success = true;
            return sr;
        }

        public async Task<ServiceResponse<List<string>>> GetFieldOfInterestNamesForUserAsync(string userName){
            var sr = new ServiceResponse<List<String>>();
            //get user
            var user = await GetUserByUsernameAsync(userName);
            if (user.Data == null)
            {
                //User does not exist
                sr.HelperMethod(404, user.Message, false);
                return sr;
            }
            //save field of interest Ids into a variable
            var userFieldOfInterestIds = _userFieldOfInterestRepo.GetFieldOfInterestIdsForUser(user.Data.Id);
            //variable to hold field of interest names
            var userFieldOfInterestNames = new List<string>();
            foreach (var userFieldOfInterestId in userFieldOfInterestIds)
            {
                //get field of interest with id
                var fieldOfInterest = await _topicService.GetFieldOfInterestByIdAsync(userFieldOfInterestId);
                //add fieldOfInterest name to variable;
                userFieldOfInterestNames.Add(fieldOfInterest.Data.Name);
            }
            
            sr.Data = userFieldOfInterestNames;
            sr.Success = true;
            sr.Code = 200;
            return sr;
        }




        public async Task<ServiceResponse<UserFieldOfInterestDto>> AddFieldOfInterestForUser(UserFieldOfInterestDto creds){
            var sr = new ServiceResponse<UserFieldOfInterestDto>();
            //validations.
            var userCreds = await _userManager.FindByNameAsync(creds.Username);
            if (userCreds == null)
            {
                sr.Code = 404;
                sr.Success = false;
                sr.Message = "No user found with such username";
                return sr;
            }

            //validate that the field of interest to be assigned to user exist in db.
            var topicCreds = await _topicService.GetFieldOfInterestByNameAsync(creds.TopicName);
            if (topicCreds.Data == null)
            {
                sr.Code = 404;
                sr.Success = false;
                sr.Message = topicCreds.Message;
                return sr;
            }

            //make sure the  user isn't already linked with fied of interest.
            if(await _userFieldOfInterestRepo.IsFieldPartOfUserInterestsAsync(userCreds.Id, topicCreds.Data.Id)){
                sr.Code = 409;
                sr.Success = false;
                sr.Message = "User is already associated with field of interest";
                return sr;
            }

            var userFieldOfInterest = new UserFieldOfInterest
            {
                UserId = userCreds.Id,
                TopicId = topicCreds.Data.Id,
                CurrentlyWorking = creds.CurrentlyWorking,
                YearsOfExperience = creds.YearsOfExperience
            };
            //insert new Field Of Interest for user.
            await _userFieldOfInterestRepo.InsertAsync(userFieldOfInterest);
            sr.Code = 201;
            sr.Data = creds;
            sr.Success = true;
            return sr;
        }
        public async Task<bool> DoesUserHaveInterestInFieldAsync(UserFieldOfInterestDto creds)
        {
            var sr = new ServiceResponse<UserFieldOfInterestDto>();
            //VALIDATIONS
            //validate  existence of user
            var mentorCreds = await _userManager.FindByNameAsync(creds.Username);
            if (mentorCreds == null)
            {
                return false;
            }

            //validate topic name
            var topicCreds = await _topicService.GetFieldOfInterestByNameAsync(creds.TopicName);
            if (topicCreds.Data == null)
            {
                return false;
            }

            if(await _userFieldOfInterestRepo.IsFieldPartOfUserInterestsAsync(mentorCreds.Id, topicCreds.Data.Id)){
                return true;
            }else{
                return false;
            }
        }

        
        
        


        
        //USER QUALIFICATIONS SECTION

        public async Task<ServiceResponse<List<QualificationResponseDto>>> GetUserQualificationsAsync(string username)
        {
            var sr = new ServiceResponse<List<QualificationResponseDto>>();
            //verify user exists
            var foundUser = await _userManager.FindByNameAsync(username);
            if(foundUser == null){
                sr.HelperMethod(404, "User not found", false);
                return sr;
            }
            //gets all qualification for a user.
            var userQualifications = await _qualificationRepo.GetUserQualificationsAsync(foundUser.Id);
            
            if (userQualifications == null)
            {
                sr.HelperMethod(
                            204,
                            $"User does not have qualifications.",
                            true
                );
            }
            else
            {
                sr.Code = 200;
                sr.Data = _mapper.Map<List<QualificationResponseDto>>(userQualifications);
                sr.Success = true;
            }
            return sr;
        }

        public async Task<ServiceResponse<QualificationResponseDto>> AddQualificationForUserAsync(QualificationDto qualificationCreds)
        {
            var sr = new ServiceResponse<QualificationResponseDto>();
            //verify user exists
            var foundUser = await _userManager.FindByNameAsync(qualificationCreds.Username);
            if (foundUser == null)
            {
                sr.HelperMethod(404, "User not found", false);
                return sr;
            }

            //make sure years entered are reasonable/valid years.
            if (!ValidateQualificationYears(qualificationCreds.StartYear, qualificationCreds.EndYear, qualificationCreds.QualificationType, out string message))
            {
                sr.HelperMethod(400, message, false);
                return sr;
            }

            //get qualification for a user.
            var userHasQualification = _qualificationRepo.GetQualificationTypeForUser(
                                                                foundUser.Id, 
                                                                qualificationCreds.SchoolName, 
                                                                qualificationCreds.QualificationType, 
                                                                qualificationCreds.Major
                                                            );
            //find out if user already have the exact qualification.
            if(userHasQualification != null){
                //error
                //user cant have the same qualification twice.
                sr.HelperMethod(409, "User already have the qualification.", false);
                return sr;
            }

            //convert to Qualification entity.
            var userQualification = _mapper.Map<Qualification>(qualificationCreds);
            userQualification.UserId = foundUser.Id;
            //gets all qualification for a user.
            await _qualificationRepo.InsertAsync(userQualification);

            //return newly created resource
            sr.Code = 201;
            sr.Data = _mapper.Map<QualificationResponseDto>(_qualificationRepo.GetQualificationTypeForUser(
                                                                foundUser.Id,
                                                                qualificationCreds.SchoolName,
                                                                qualificationCreds.QualificationType,
                                                                qualificationCreds.Major
                                                            ));
            sr.Success = true;
    
            return sr;
        }

        public async Task<ServiceResponse<QualificationResponseDto>> UpdateUserQualificationAsync(int qualificationId, QualificationDto qualificationCreds)
        {
            var sr = new ServiceResponse<QualificationResponseDto>();
            //verify user exists
            var foundUser = await _userManager.FindByNameAsync(qualificationCreds.Username);
            if (foundUser == null)
            {
                sr.HelperMethod(404, "User not found", false);
                return sr;
            }

            //make sure years entered are reasonable/valid years.
            if (!ValidateQualificationYears(qualificationCreds.StartYear, qualificationCreds.EndYear, qualificationCreds.QualificationType, out string message)){
                sr.HelperMethod(400, message, false);
                return sr;
            }
            //find out if the update would cause a conflict.
            var userHasQualification = _qualificationRepo.GetQualificationTypeForUser(
                                                                foundUser.Id,
                                                                qualificationCreds.SchoolName,
                                                                qualificationCreds.QualificationType,
                                                                qualificationCreds.Major
                                                            );
            //find out if user already have exact qualification record.
            if (userHasQualification == null)
            {
                //error
                //user dont have the qualification.
                sr.HelperMethod(404, "No qualification found for user.", false);
                return sr;
            }

            //convert to Qualification entity.
            var userQualification = _mapper.Map<Qualification>(qualificationCreds);
            userQualification.Id = qualificationId;
            userQualification.UserId = foundUser.Id;
            //Update qualification for a user.
            await _qualificationRepo.UpdateAsync(userQualification);

            sr.Code = 200;
            sr.Data = _mapper.Map<QualificationResponseDto>(userQualification); ;
            sr.Success = true;
            return sr;
        }

        public async Task<ServiceResponse<string>> DeleteUserQualificationAsync(int qualificationId)
        {
            var sr = new ServiceResponse<string>();

            //get qualification.
            var userHasQualification = await _qualificationRepo.GetByIdAsync(qualificationId);
            //check exsitence of qualification.
            if (userHasQualification == null)
            {
                //error
                sr.HelperMethod(404, "No qualification matching id was found.", false);
                return sr;
            }
            
            //Delete qualification for a user.
            await _qualificationRepo.DeleteAsync(userHasQualification);

            sr.Code = 200;
            sr.Message = "Qualification delete was successful.";
            sr.Success = true;
            return sr;
        }
        
        




        public async Task<ServiceResponse<List<WorkExperienceResponseDto>>> GetUserWorkExperiencesAsync(string username)
        {
            var sr = new ServiceResponse<List<WorkExperienceResponseDto>>();
            //verify user exists
            var foundUser = await _userManager.FindByNameAsync(username);
            if (foundUser == null)
            {
                sr.HelperMethod(404, "User not found", false);
                return sr;
            }

            var userWorkExperiences = await _workExperienceRepo.GetAllForUserAsync(foundUser.Id);

            sr.Code = StatusCodes.Status200OK;
            sr.Data = _mapper.Map<List<WorkExperienceResponseDto>>(userWorkExperiences);
            sr.Success = true;
            return sr;
        }






        //The term "friends" is used cos they have mutual interest(s).
        public async Task<ServiceResponse<List<FriendDto>>> GetUserFriendsAsync(string username)
        {
            List<FriendDto> friends;
            var sr = new ServiceResponse<List<FriendDto>>();
            //get user
            var user = await GetUserByUsernameAsync(username);
            if (user.Data == null)
            {
                //user not found
                //return error message gotten
                sr.HelperMethod(404, user.Message, false);
                return sr;
            }

            //find out if the user is a mentor or a mentee
            if(await _userManager.IsInRoleAsync(user.Data, "Mentor")){
                //User is a mentor.
                //get all mentorship info for user
                var mentorshipsInfo = await _followRepo.GetAllMentorshipInfoForMentor(user.Data.Id);
                //get unique mentorship info
                //taking the first relationship with a mentee even though there could be more.
                var uniqueMentorshipInfo = mentorshipsInfo.GroupBy(f => f.MenteeId)
                                                          .Select(m => m.FirstOrDefault())
                                                          .ToList();

                friends = _mapper.Map<List<MentorFriendDto>>(uniqueMentorshipInfo)
                                 .Cast<FriendDto>()
                                 .ToList();
                                 
                // friends.ForEach(m =>
                //     {
                //         //loop through all friends,
                //         //get the the mentorship info between the mentor and the current friend/mentee in the loop using the mentte username.
                //         var mentorshipInfo = uniqueMentorshipInfo.Find(p => p.Mentee.UserName == m.User.Username);
                //         //get all field of interest/topic(s) the have in common.
                //         m.MutualInterests = GetAllMentorshipMutualInterestAsync(user.Data.Id,
                //                                                                 mentorshipInfo.MenteeId).Result;
                //     }
                // );

                var tt = friends.Select(async m =>
                    {
                        //loop through all friends,
                        //get the the mentorship info between the mentor and the current friend/mentee in the loop using the mentte username.
                        var mentorshipInfo = uniqueMentorshipInfo.Find(p => p.Mentee.UserName == m.User.Username);
                        //get all field of interest/topic(s) the have in common.
                        m.MutualInterests = await GetAllMentorshipMutualInterestAsync(user.Data.Id,
                                                                  mentorshipInfo.MenteeId);
                        return m.MutualInterests.Count;
                    }
                );

                await Task.WhenAll(tt);

            }else{
                //User is a mentee.
                //get all mentorship info for user
                var mentorshipsInfo = await _followRepo.GetAllMentorshipInfoForMentee(user.Data.Id);
                //get unique mentorship info
                var uniqueMentorshipInfo = mentorshipsInfo.GroupBy(f => f.MentorId)
                                                          .Select(m => m.FirstOrDefault())
                                                          .ToList();
                friends = _mapper.Map<List<MenteeFriendDto>>(uniqueMentorshipInfo)
                                 .Cast<FriendDto>()
                                 .ToList();
                                 
                // friends.ForEach(async m => //Find the mentorship info that contains friend username.
                // {
                //     //Find the mentorship info that contains friend username.
                //     var mentorshipInfo = uniqueMentorshipInfo.Find(p => p.Mentor.UserName == m.User.Username);
                //     m.MutualInterests = await GetAllMentorshipMutualInterestAsync(user.Data.Id, mentorshipInfo.MenteeId);
                // });

                var tt = friends.Select(async m =>
                    {
                        //loop through all friends,
                        //Find the mentorship info that contains friend username.
                        var mentorshipInfo = uniqueMentorshipInfo.Find(p => p.Mentor.UserName == m.User.Username);
                        //get all field of interest/topic(s) the have in common.
                        m.MutualInterests = await GetAllMentorshipMutualInterestAsync(user.Data.Id,
                                                                  mentorshipInfo.MenteeId);
                        return m.MutualInterests.Count;
                    }
                );

                await Task.WhenAll(tt);
            }

            sr.Data = friends;
            sr.Code = 200;
            sr.Success = true;
            return sr;
        }

        public async Task<ServiceResponse<List<UnAttendedRequestDto>>> GetMentorshipRequestsAsync(string mentorUsername)
        {
            List<Follow> requests;
            List<UnAttendedRequestDto> mentorshipRequests;
            var sr = new ServiceResponse<List<UnAttendedRequestDto>>();
            //get user
            var user = await GetUserByUsernameAsync(mentorUsername);
            if (user.Data == null)
            {
                //user not found
                //return error message gotten
                sr.HelperMethod(404, user.Message, false);
                return sr;
            }
            
            //find out if user is a mentor or mentee so as to know what response object to return.
            if(await _userManager.IsInRoleAsync(user.Data, "Mentor")){
                requests = await _followRepo.GetMentorshipRequestsForMentorAsync(user.Data.Id);
                // if(requests.Count == 0){
                //     //to return an emppty lis or not
                //     //return error message gotten
                //     sr.Code = 204;
                //     sr.Success = true;
                //     sr.Data = new List<FollowResponseDto>();
                //     return sr;
                // }
                mentorshipRequests = _mapper.Map<List<UnAttendedRequestForMentorDto>>(requests)
                                            .Cast<UnAttendedRequestDto>()
                                            .ToList();
            }else{
                requests = await _followRepo.GetSentMentorshipRequestsForMenteeAsync(user.Data.Id);
                mentorshipRequests = _mapper.Map<List<UnAttendedRequestForMenteeDto>>(requests)
                                            .Cast<UnAttendedRequestDto>()
                                            .ToList(); ;
            }
           

            sr.Data = mentorshipRequests;
            sr.Code = 200;
            sr.Success = true;
            return sr;
        }

        public async Task<ServiceResponse<List<UnAttendedRequestDto>>> GetFriendshipInfoAsync(string mentorUsername,
                                                                                              string menteeUsername)
        {
            List<Follow> mentorshipInfo;
            //List<UnAttendedRequestDto> mentorshipRequests;
            var sr = new ServiceResponse<List<UnAttendedRequestDto>>();
            //get user
            var mentor = await GetUserByUsernameAsync(mentorUsername);
            var mentee = await GetUserByUsernameAsync(menteeUsername);
            if (mentor.Data == null || mentee.Data == null)
            {
                //user not found
                //return error message gotten
                sr.HelperMethod(404, "One of the usernames provided does not exist.", false);
                return sr;
            }

            //get friendship info
            mentorshipInfo = await _followRepo.GetOngoingMentorshipInfoBetweenUsersAsync(mentor.Data.Id, mentee.Data.Id);

            //sr.Data = mentorshipRequests;
            sr.Code = 200;
            sr.Success = true;
            return sr;
        }

        public async Task<ServiceResponse<List<UsersRelationshipInfoDto>>> GetUsersRelationshipInfoAsync(string mentorUsername,
                                                                                                         string menteeUsername)
        {
            var sr = new ServiceResponse<List<UsersRelationshipInfoDto>>();
            //get user
            var mentor = await GetUserByUsernameAsync(mentorUsername);
            var mentee = await GetUserByUsernameAsync(menteeUsername);
            if (mentor.Data == null || mentee.Data == null)
            {
                //user not found
                //return error message gotten
                sr.HelperMethod(404, "One of the usernames provided does not exist.", false);
                return sr;
            }

            //get mentorship relations info
            var mentorshipInfo = await _followRepo.GetUsersRelationshipInfoAsync(mentor.Data.Id, mentee.Data.Id);

            sr.Data = _mapper.Map<List<UsersRelationshipInfoDto>>(mentorshipInfo);
            sr.Code = 200;
            sr.Success = true;
            return sr;
        }







        public async Task<List<MutualInterestDto>> GetAllMentorshipMutualInterestAsync(int mentorId, int menteeId)
        {
            List<Follow> mentorshipInfo;
            //get friendship info
            mentorshipInfo = await _followRepo.GetOngoingMentorshipInfoBetweenUsersAsync(mentorId, menteeId);
            var mutualInterests = _mapper.Map<List<MutualInterestDto>>(mentorshipInfo.Select(mi => mi.Topic).ToList());
            return mutualInterests;
        }

        private bool ValidateQualificationYears(int startYear, int endYear, string qualificationType, out string message)
        {
            if (startYear > endYear)
            {
                message = "Start year cannot be greater than end year.";
                return false;
            }
            else if (startYear == endYear)
            {
                message = "End year cannot be the same as start year.";
                return false;
            }
            else if (!qualificationType.Contains("Master"))
            {
                var yearNumber = "";
                message = $"End year cannot be {yearNumber} year after start year except for qualifications with master degree.";
                if ((startYear + 1) == endYear)
                {
                    message = "End year cannot be one year after start year except for qualifications with master degree.";
                    return false;
                }
                else if ((startYear + 2) == endYear)
                {
                    message = $"End year cannot be two years after start year except for qualifications with master degree.";
                    return false;
                }
            }
            else if ((startYear + 3) == endYear)
            {
                message = "Please select a valid start year and end year.";
                return false;
            }
            message = "";
            return true;
        }

        
    }
}