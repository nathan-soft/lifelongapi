using LifeLongApi.Codes;
using LifeLongApi.Dtos;
using LifeLongApi.Dtos.Response;
using LifeLongApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using static LifeLongApi.Codes.AppHelper;

namespace LifeLongApi.Services {
    public interface IUserService {
        Task<ServiceResponse<List<AppUserResponseDto>>> GetNonAdministrativeUsersAsync(string filterString);
        Task<ServiceResponse<List<AppUserResponseDto>>> GetAdministrativeUsersAsync();
        public Task<ServiceResponse<UserDto>> GetUserByUsernameAsync (string username);
        Task<ServiceResponse<AppUserResponseDto>> CreateUserAsync(NewUserDto newUser, params string[] roles);
        public Task<ServiceResponse<UserDto>> UpdateUserAsync (string username, UserDto userCreds);
        public Task<ServiceResponse<string>> DeleteUserAsync(string userEmail);
        public Task<ServiceResponse<string>> SendMailToUserAsync(MailDto mailInfo);


        public Task<ServiceResponse<List<SearchResponseDto>>> GetMentorsByFieldOfInterestAsync(string searchString);
        public Task<ServiceResponse<UserFieldOfInterestDto>> AddFieldOfInterestForUser (UserFieldOfInterestDto creds);
        public Task<bool> DoesUserHaveInterestInFieldAsync (UserFieldOfInterestDto creds);
        public Task<ServiceResponse<List<string>>> GetFieldOfInterestNamesForUserAsync (string userName);


        public Task<ServiceResponse<List<QualificationResponseDto>>> GetUserQualificationsAsync (string username);
        public Task<ServiceResponse<QualificationResponseDto>> AddQualificationForUserAsync (QualificationDto qualificationCreds);
        public Task<ServiceResponse<QualificationResponseDto>> UpdateUserQualificationAsync (int qualificationId, QualificationDto qualificationCreds);
        public Task<ServiceResponse<string>> DeleteUserQualificationAsync (int qualificationId);

        

        public Task<ServiceResponse<List<WorkExperienceResponseDto>>> GetUserWorkExperiencesAsync (string username);
        

        Task<ServiceResponse<List<FriendDto>>> GetUserFriendsAsync (string username);
        Task<ServiceResponse<List<UnAttendedRequestDto>>> GetMentorshipRequestsAsync (string mentorUsername);
        Task<ServiceResponse<List<UnAttendedRequestDto>>> GetFriendshipInfoAsync(string mentorUsername, string menteeUsername);
        Task<List<MutualInterestDto>> GetAllMentorshipMutualInterestAsync(int mentorId, int menteeId);
        Task<ServiceResponse<List<UsersRelationshipInfoDto>>> GetUsersRelationshipInfoAsync(string mentorUsername, string menteeUsername);
        
    }
}