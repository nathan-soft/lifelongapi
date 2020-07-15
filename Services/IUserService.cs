using LifeLongApi.Codes;
using LifeLongApi.Dtos;
using LifeLongApi.Dtos.Response;
using LifeLongApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeLongApi.Services {
    public interface IUserService {
        public Task<ServiceResponse<RegisterDto>> CreateUserAsync (RegisterDto newUser);

        // public ServiceResponse<List<RoleDto>> GetUsers();

        public Task<ServiceResponse<AppUser>> GetUserByUsernameAsync (string username);
        public Task<ServiceResponse<UserDto>> GetUserByUsernameWithRelationshipsAsync (string userName);
        public Task<ServiceResponse<UserDto>> UpdateUserAsync (string username, UserDto userCreds);
        public Task<ServiceResponse<FollowResponseDto>> CreateOrEditFollowRelationshipAsync (FollowDto requestCreds);
        //public Task<ServiceResponse<FollowResponseDto>> UpdateFollowRelationshipAsync(FollowDto requestCreds);
        public Task<ServiceResponse<UserFieldOfInterestDto>> AddFieldOfInterestForUser (UserFieldOfInterestDto creds);
        public Task<bool> DoesUserHaveInterestInFieldAsync (UserFieldOfInterestDto creds);
        public Task<ServiceResponse<List<string>>> GetFieldOfInterestNamesForUserAsync (string userName);

        public Task<ServiceResponse<List<QualificationResponseDto>>> GetUserQualificationsAsync (string username);
        public Task<ServiceResponse<QualificationResponseDto>> AddQualificationForUserAsync (QualificationDto qualificationCreds);
        public Task<ServiceResponse<QualificationResponseDto>> UpdateUserQualificationAsync (int qualificationId, QualificationDto qualificationCreds);
        public Task<ServiceResponse<string>> DeleteUserQualificationAsync (int qualificationId);

        // public Task<ServiceResponse<RoleDto>> GetRoleByEmailAsync(int userEmail);

        // public Task<ServiceResponse<RoleDto>> UpdateUserAsync(string oldRoleName, string newRoleName);

        // public Task<ServiceResponse<string>> DeleteUser(string username);

        public Task<ServiceResponse<List<WorkExperienceResponseDto>>> GetUserWorkExperiencesAsync (string username);
        public Task<ServiceResponse<List<SearchResponseDto>>> GetUsersByInterestSearchResultAsync (string searchString);
        Task<ServiceResponse<List<FriendDto>>> GetUserFriendsAsync (string username);
        Task<ServiceResponse<List<FollowResponseDto>>> GetMentorshipRequestsAsync (string mentorUsername);
    }
}