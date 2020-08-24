using System.Net;
using AutoMapper;
using LifeLongApi.Dtos;
using LifeLongApi.Dtos.Response;
using LifeLongApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using LifeLongApi.Codes;

namespace LifeLongApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MentorshipsController : ControllerBase
    {
        private readonly IUserService _userService;
        private static ApiOkResponseDto _apiOkResponse;
        private static ApiErrorResponseDto _apiErrorResponse;
        private static IFollowService _followService;
        private readonly IMapper _mapper;

        public MentorshipsController(IMapper mapper, IUserService userService,
                    ApiOkResponseDto apiOkResponse, ApiErrorResponseDto apiErrorResponse, IFollowService followService)
        {
            _apiErrorResponse = apiErrorResponse;
            _apiOkResponse = apiOkResponse;
            _followService = followService;
            _mapper = mapper;
            _userService = userService;
        }

        [HttpPost("request")]
        public async Task<ApiResponseDto> RequestMentorship(FollowDto mentorshipRequest)
        {
            try
            {
                var response = await _followService.CreateMentorshipRequestAsync(mentorshipRequest);
                //set status code.
                HttpContext.Response.StatusCode = response.Code;

                if (response.Success)
                {
                    HttpContext.Response.Headers.Add("Location", $"{Request.Host}/");
                    //return data.
                    _apiOkResponse.Data = response.Data;
                    return _apiOkResponse;
                }
                else
                {
                    return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(response);
                }

            }
            catch (Exception ex)
            {
                //set status code.
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                //log and return default custom error
                _apiErrorResponse.Message = ex.Message;
                return _apiErrorResponse;
            }
        }

        [HttpPut("{mentorshipRequestId}")]
        public async Task<ApiResponseDto> UpdateMentorshipRequestAsync(int mentorshipRequestId,
                                                           MentorshipRequestUpdateDto mentorshipCreds)
        {
            ServiceResponse<FriendDto> response;
            ServiceResponse<UnAttendedRequestDto> response1;
            try
            {
                if(mentorshipCreds.Status == AppHelper.FollowStatus.CONFIRMED)
                {
                    response = await _followService.ConfirmMentorshipRequestAsync(mentorshipRequestId, mentorshipCreds);
                    //set status code.
                    HttpContext.Response.StatusCode = response.Code;

                    if (response.Success)
                    {
                        //return data.
                        _apiOkResponse.Data = response.Data;
                        return _apiOkResponse;
                    }
                    else
                    {
                        return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(response);
                    }
                }
                else if(mentorshipCreds.Status == AppHelper.FollowStatus.ONHOLD)
                {
                    response1 = await _followService.PutMentorshipRequestOnHoldAsync(mentorshipRequestId, mentorshipCreds);
                    //set status code.
                    HttpContext.Response.StatusCode = response1.Code;

                    if (response1.Success)
                    {
                        //return data.
                        _apiOkResponse.Data = response1.Data;
                        return _apiOkResponse;
                    }
                    else
                    {
                        return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(response1);
                    }
                }
                else
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    _apiErrorResponse.Message = "Invalid status value provided.";
                    return _apiErrorResponse;
                }
            }
            catch (Exception ex)
            {
                //set status code.
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                //log and return default custom error
                _apiErrorResponse.Message = ex.Message;
                return _apiErrorResponse;
            }
        }

        [HttpDelete("{mentorshipRequestId}")]
        public async Task<ApiResponseDto> DeleteMentorshipRequestOnHoldAsync(int mentorshipRequestId)
        {
            try
            {
                var response = await _followService.DeleteMentorshipRequestAsync(mentorshipRequestId);
                //set status code.
                HttpContext.Response.StatusCode = response.Code;

                if (response.Success)
                {
                    //return data.
                    _apiOkResponse.Data = null;
                    return _apiOkResponse;
                }
                else
                {
                    return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(response);
                }

            }
            catch (Exception ex)
            {
                //set status code.
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                //log and return default custom error
                _apiErrorResponse.Message = ex.Message;
                return _apiErrorResponse;
            }
        }

        // [HttpGet("info/{mentorUsername}/{menteeUsername}")]
        // public async Task<ApiResponseDto> GetMentorshipInfoAsync(string mentorUsername, string menteeUsername)
        // {
        //     try
        //     {
        //         var response = await _userService.GetFriendshipInfoAsync(mentorUsername, menteeUsername);
        //         //set status code.
        //         HttpContext.Response.StatusCode = response.Code;

        //         if (!response.Success)
        //         {
        //             return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(response);
        //         }
        //         else
        //         {
        //             //return data.
        //             _apiOkResponse.Data = response.Data;
        //             return _apiOkResponse;
        //         }

        //     }
        //     catch (Exception ex)
        //     {
        //         //set status code.
        //         HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        //         //log and return default custom error
        //         _apiErrorResponse.Message = ex.Message;
        //         return _apiErrorResponse;
        //     }
        // }
    }


}