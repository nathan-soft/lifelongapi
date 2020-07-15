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
        private readonly IMapper _mapper;

        public MentorshipsController(IMapper mapper, IUserService userService,
                    ApiOkResponseDto apiOkResponse, ApiErrorResponseDto apiErrorResponse)
        {
            _userService = userService;
            _mapper = mapper;
            _apiErrorResponse = apiErrorResponse;
            _apiOkResponse = apiOkResponse;
        }

        [HttpPost("create")]
        public async Task<ApiResponseDto> MentorshipCreate(FollowDto mentorshipRequest)
        {
            try
            {
                var response = await _userService.CreateOrEditFollowRelationshipAsync(mentorshipRequest);
                //set status code.
                HttpContext.Response.StatusCode = response.Code;

                if (!response.Success)
                {
                    return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(response);
                }
                else
                {
                    HttpContext.Response.Headers.Add("Location", $"{Request.Host}/");
                    //return data.
                    _apiOkResponse.Data = response.Data;
                    return _apiOkResponse;
                }

            }
            catch (Exception ex)
            {
                _apiErrorResponse.Message = ex.Message;
                return _apiErrorResponse;
            }
        }

        [HttpPut("/update")]
        public async Task<ApiResponseDto> MentorshipUpdate(FollowDto mentorshipRequest)
        {
            try
            {
                var response = await _userService.CreateOrEditFollowRelationshipAsync(mentorshipRequest);
                //set status code.
                HttpContext.Response.StatusCode = response.Code;

                if (!response.Success)
                {
                    return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(response);
                }
                else
                {
                    HttpContext.Response.Headers.Add("Location", $"{Request.Host}/");
                    //return data.
                    _apiOkResponse.Data = response.Data;
                    return _apiOkResponse;
                }

            }
            catch (Exception ex)
            {
                _apiErrorResponse.Message = ex.Message;
                return _apiErrorResponse;
            }
        }

    }


}