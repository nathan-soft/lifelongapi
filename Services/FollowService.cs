using AutoMapper;
using LifeLongApi.Codes;
using LifeLongApi.Data.Repositories;
using LifeLongApi.Dtos;
using LifeLongApi.Dtos.Response;
using LifeLongApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static LifeLongApi.Codes.AppHelper;

namespace LifeLongApi.Services
{
    public class FollowService : IFollowService
    {
        private readonly IFollowRepository _followRepo;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly ITopicService _topicService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserService _userService;
        private readonly IUserFieldOfInterestRepository _userFieldOfInterestRepo;
        public FollowService(IFollowRepository followRepo,
                             IMapper mapper,
                             INotificationService notificationService,
                             ITopicService topicService,
                             UserManager<AppUser> userManager,
                             IUserService userService,
                             IUserFieldOfInterestRepository userFieldOfInterestRepo)
        {
            _followRepo = followRepo;
            _mapper = mapper;
            _notificationService = notificationService;
            _topicService = topicService;
            _userManager = userManager;
            _userService = userService;
            _userFieldOfInterestRepo = userFieldOfInterestRepo;
        }

        public async Task<ServiceResponse<UnAttendedRequestDto>> CreateMentorshipRequestAsync(FollowDto requestCreds)
        {
            var sr = new ServiceResponse<UnAttendedRequestDto>();
            //VALIDATIONS
            //Make sure users exist.
            var mentorCreds = await _userManager.FindByNameAsync(requestCreds.MentorUsername);
            var menteeCreds = await _userManager.FindByNameAsync(requestCreds.MenteeUsername);
            if (mentorCreds == null || menteeCreds == null)
            {
                sr.HelperMethod(404, "One of the users does not exist.", false);
                return sr;
            }

            //validate topic name
            var topicCreds = await _topicService.GetFieldOfInterestByNameAsync(requestCreds.TopicName);
            if (topicCreds.Data == null)
            {
                sr.HelperMethod(404, $"Could not find field Of interest with name {requestCreds.TopicName}", false);
                return sr;
            }

            //validate that mentor have listed the topic, with the id of  "topicId" ,as part of their field of interest.
            var mentorHasInterest = await _userFieldOfInterestRepo.IsFieldPartOfUserInterestsAsync(mentorCreds.Id, topicCreds.Data.Id);
            if (!mentorHasInterest)
            {
                sr.HelperMethod(404, "The selected field of interest is not part of the mentor's interest.", false);
                return sr;
            }

            //get existing relationship
            var relationshipExist = await _followRepo.GetMentorshipInfo(menteeCreds.Id, mentorCreds.Id, topicCreds.Data.Id);
            //check if relationship exist already
            if (relationshipExist != null)
            {
                sr.HelperMethod(StatusCodes.Status403Forbidden, "Similar mentorship request already exist.", false);
                return sr;
            }

            var mentorshipRequest = new Follow
            {
                MenteeId = menteeCreds.Id,
                MentorId = mentorCreds.Id,
                TopicId = topicCreds.Data.Id,
                Status = AppHelper.FollowStatus.PENDING.ToString()
            };
            //insert new relationship to db and save.
            await _followRepo.InsertAsync(mentorshipRequest);

            //send notification to mentor
            var message = $"{menteeCreds.FirstName} {menteeCreds.LastName} Requested for <b>Mentorship.</b>";
            await _notificationService.NewNotificationAsync(menteeCreds.Id,
                                                      mentorCreds.Id,
                                                      message,
                                                      NotificationType.MENTORSHIPREQUEST);


            //get recently added mentorship request.
            var recentRequest = await _followRepo.GetMentorshipInfo(menteeCreds.Id, mentorCreds.Id, topicCreds.Data.Id);
            sr.Code = 201;
            sr.Data = _mapper.Map<UnAttendedRequestForMenteeDto>(recentRequest);
            sr.Success = true;
            return sr;

        }

        public async Task<ServiceResponse<FriendDto>> ConfirmMentorshipRequestAsync(int mentorshipId, MentorshipRequestUpdateDto mentorshipRequest)
        {
            var sr = new ServiceResponse<FriendDto>();
            //get pending mentorship
            var foundPendingMentorship = await _followRepo.GetByIdAsync(mentorshipId);
            //Validation
            var result = ValidateMentorshipCred<FriendDto>(foundPendingMentorship, mentorshipRequest);
            if (!result.Success)
            {
                return result;
            }

            //update mentorship request.
            await PerformUpdateAsync(foundPendingMentorship, mentorshipRequest.Status);

            //send notification to mentee.
            string message;
            message = $"{foundPendingMentorship.Mentor.FirstName} {foundPendingMentorship.Mentor.LastName} has accepted your <b>Mentorship</b> request.";

            await _notificationService.NewNotificationAsync(foundPendingMentorship.MentorId,
                                                            foundPendingMentorship.MenteeId,
                                                            message,
                                                            NotificationType.MENTORSHIPREQUEST);

            //return result
            sr.Code = 200;
            sr.Data = _mapper.Map<MentorFriendDto>(foundPendingMentorship);
            sr.Data.MutualInterests = await _userService.GetAllMentorshipMutualInterestAsync(foundPendingMentorship.MentorId,
                                                                                             foundPendingMentorship.MenteeId);
            sr.Success = true;
            return sr;

        }

        public async Task<ServiceResponse<UnAttendedRequestDto>> PutMentorshipRequestOnHoldAsync(int mentorshipId, MentorshipRequestUpdateDto mentorshipRequest)
        {
            var sr = new ServiceResponse<UnAttendedRequestDto>();
            //get pending mentorship
            var foundPendingMentorship = await _followRepo.GetByIdAsync(mentorshipId);
            //Validation
            var result = ValidateMentorshipCred<UnAttendedRequestDto>(foundPendingMentorship, mentorshipRequest);
            if (!result.Success)
            {
                return result;
            }

            //update mentorship request.
            await PerformUpdateAsync(foundPendingMentorship, mentorshipRequest.Status);

            //send notification to mentee.
            string message;
            message = $"{foundPendingMentorship.Mentor.FirstName} {foundPendingMentorship.Mentor.LastName} has put your request for <b>Mentorship</b> on hold.";

            await _notificationService.NewNotificationAsync(foundPendingMentorship.MentorId,
                                                            foundPendingMentorship.MenteeId,
                                                            message,
                                                            NotificationType.MENTORSHIPREQUEST);

            //return result
            sr.Code = 200;
            sr.Data = _mapper.Map<UnAttendedRequestForMentorDto>(foundPendingMentorship);
            sr.Success = true;
            return sr;

        }

        public async Task<ServiceResponse<UnAttendedRequestDto>> DeleteMentorshipRequestAsync(int mentorshipId)
        {
            var sr = new ServiceResponse<UnAttendedRequestDto>();
            //get pending mentorship
            var mentorshipInfo = await _followRepo.GetByIdAsync(mentorshipId);
            //Verify the resource exists
            if (mentorshipInfo == null)
            {
                sr.HelperMethod(404, "Mentorship request not found.", false);
            }

            //delete mentorship info.
            await _followRepo.DeleteAsync(mentorshipInfo);

            //return result
            sr.HelperMethod(200, "Delete Successful.", true);
            return sr;

        }







        private ServiceResponse<T> ValidateMentorshipCred<T>(Follow pendingMentorship,
                                                                           MentorshipRequestUpdateDto creds)
        {
            var sr = new ServiceResponse<T>();
            //check if relationship exist already
            if (pendingMentorship == null)
            {
                sr.HelperMethod(404, "Pending mentorship request not found.", false);
            }
            else if (pendingMentorship.Mentor.UserName != creds.MentorUsername)
            {
                //verify mentor username
                //this is to make sure the mentor's username provided is the same mentor the mentorship request was sent to.
                sr.HelperMethod(403, $"You do not have the permission to perform the action.", false);
            }else if(pendingMentorship.Status == FollowStatus.CONFIRMED.ToString() || creds.Status == FollowStatus.PENDING){
                //should not be able to make changes to a mentorship request that's been confirmed already.
                //or reverting back the status of a request to "Pending".
                sr.HelperMethod(403, "The action is not allowed.", false);
            }else if (pendingMentorship.Status == creds.Status.ToString())
            {
                sr.HelperMethod(400, "No action was taken because the mentorship request is already up to date.", false);
            }
            else
            {
                sr.HelperMethod(200, null, true);
            }
            return sr;
        }

        /// <summary>
        /// updates the status of a mentorship request and saves it to db.
        /// </summary>
        /// <param name="mentorshipRequest">The request to update.</param>
        /// <param name="status">The status to change the request to.</param>
        /// <returns></returns>
        private async Task PerformUpdateAsync(Follow mentorshipRequest, FollowStatus status)
        {
            //update status property
            mentorshipRequest.Status = status.ToString();
            //save changes to db.
            await _followRepo.UpdateAsync(mentorshipRequest);
        }
    }
}