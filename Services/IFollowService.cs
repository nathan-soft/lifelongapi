using LifeLongApi.Codes;
using LifeLongApi.Dtos;
using LifeLongApi.Dtos.Response;
using LifeLongApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeLongApi.Services
{
    public interface IFollowService
    {
        Task<ServiceResponse<FriendDto>> ConfirmMentorshipRequestAsync(int mentorshipId, AppHelper.FollowStatus status);
        Task<ServiceResponse<UnAttendedRequestDto>> CreateMentorshipRequestAsync(FollowDto requestCreds);
        Task<ServiceResponse<UnAttendedRequestDto>> DeleteMentorshipRequestAsync(int mentorshipId);
        Task<ServiceResponse<UnAttendedRequestDto>> PutMentorshipRequestOnHoldAsync(int mentorshipId, AppHelper.FollowStatus status);
    }
}