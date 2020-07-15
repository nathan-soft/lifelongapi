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

namespace LifeLongApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private static ApiOkResponseDto _apiOkResponse;
        private static ApiErrorResponseDto _apiErrorResponse;
        private readonly IMapper _mapper;

        public UsersController(IMapper mapper, IUserService userService,
                    ApiOkResponseDto apiOkResponse, ApiErrorResponseDto apiErrorResponse)
        {
            _userService = userService;
            _mapper = mapper;
            _apiErrorResponse = apiErrorResponse;
            _apiOkResponse = apiOkResponse;
        }

        [HttpGet("{username}")]
        public async Task<ApiResponseDto> GetUserAsync(string username)
        {
            var userDto = await _userService.GetUserByUsernameWithRelationshipsAsync(username);
            //set status code.
            HttpContext.Response.StatusCode = userDto.Code;
            if(!userDto.Success){
                //error
                //return error.
                return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(userDto);
            }else{
                //return data.
                _apiOkResponse.Data = userDto.Data;
               return _apiOkResponse;
            }
        }

        // [Authorize(Roles = "Admin")]
        // [HttpGet]
        // public async Task<ApiResponseDto> GetAllUserAsync()
        // {
        //     var user = await _userService.();
        //     //set status code.
        //     HttpContext.Response.StatusCode = user.Code;
        //     if (!user.Success)
        //     {
        //         //error
        //         //return error.
        //         return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(user);
        //     }
        //     else
        //     {
        //         //remove personal datas that's not meant to be shared by converting returned data to userDto
        //         var convertedUserInfo = _mapper.Map<UserDto>(user);
        //         //return data.
        //         return _apiOkResponse = _mapper.Map<ApiOkResponseDto>(convertedUserInfo);
        //     }
        // }

        // [Authorize(Roles = "Admin")]
        // [HttpPost]
        // public ActionResult UserCreate([FromBody] UserDto Dto)
        // {
        //     return Created("/", null);
        // }

        [HttpGet("mentors/{mentesUsername}/requests")]
        public async Task<ApiResponseDto> GetMentorshipRequestsForMentorAsync(string mentesUsername)
        {
            try
            {
                var response = await _userService.GetMentorshipRequestsAsync(mentesUsername);
                //set status code.
                HttpContext.Response.StatusCode = response.Code;

                if(!response.Success){
                    return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(response);
                }else{
                    //return data.
                     _apiOkResponse.Data = response.Data;
                    return _apiOkResponse;
                }
                
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                 _apiErrorResponse.Message = ex.Message;
                return _apiErrorResponse;
            }
        }


         [HttpGet("{username}/friends")]
        public async Task<ApiResponseDto> GetFriendsAsync(string username)
        {
            try
            {
                var response = await _userService.GetUserFriendsAsync(username);
                //set status code.
                HttpContext.Response.StatusCode = response.Code;

                if (!response.Success)
                {
                    return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(response);
                }
                else
                {
                    //return data.
                    _apiOkResponse.Data = response.Data;
                    return _apiOkResponse;
                }

            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                _apiErrorResponse.Message = ex.Message;
                return _apiErrorResponse;
            }
        }




        
        
        
        [HttpPost("interests")]
        public async Task<ApiResponseDto> NewFieldOfInterestForUser(UserFieldOfInterestDto userInterest)
        {
            try
            {
                var response = await _userService.AddFieldOfInterestForUser(userInterest);
                //set status code.
                HttpContext.Response.StatusCode = response.Code;

                if (!response.Success)
                {
                    return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(response);
                }
                else
                {
                    //return data.
                    _apiOkResponse.Data = response.Data;
                    return _apiOkResponse;
                }

            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                _apiErrorResponse.Message = ex.Message;
                return _apiErrorResponse;
            }
        }

        // [HttpDelete("{UserId}")]
        // public ActionResult UserDelete(string UserId)
        // {
        //     try
        //     {
        //         //try to delete
        //         return Ok();
        //     }
        //     catch (System.Exception ex)
        //     {
        //         return BadRequest(ex.Message);
        //     }
        // }

        [HttpGet("{username}/qualifications")]
        public async Task<ApiResponseDto> GetUserQualifications(string username)
        {
            try
            {
                var response = await _userService.GetUserQualificationsAsync(username);
                //set status code.
                HttpContext.Response.StatusCode = response.Code;

                if (!response.Success)
                {
                    return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(response);
                }
                else
                {
                    //return data.
                    _apiOkResponse.Data = response.Data;
                    return _apiOkResponse;
                }

            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                _apiErrorResponse.Message = ex.Message;
                return _apiErrorResponse;
            }
        }

        [HttpGet("{username}/interests")]
        public async Task<ApiResponseDto> GetUserFieldOfInterests(string username)
        {
            try
            {
                var response = await _userService.GetFieldOfInterestNamesForUserAsync(username);
                //set status code.
                HttpContext.Response.StatusCode = response.Code;

                if (!response.Success)
                {
                    return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(response);
                }
                else
                {
                    //return data.
                    _apiOkResponse.Data = response.Data;
                    return _apiOkResponse;
                }

            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                _apiErrorResponse.Message = ex.Message;
                return _apiErrorResponse;
            }
        }

        [HttpGet("{username}/work-experiences")]
        public async Task<ApiResponseDto> GetUserWorkExperiencesAsync(string username)
        {
            try
            {
                var response = await _userService.GetUserWorkExperiencesAsync(username);
                //set status code.
                HttpContext.Response.StatusCode = response.Code;

                if (!response.Success)
                {
                    return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(response);
                }
                else
                {
                    //return data.
                    _apiOkResponse.Data = response.Data;
                    return _apiOkResponse;
                }

            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                _apiErrorResponse.Message = ex.Message;
                return _apiErrorResponse;
            }
        }

        [AllowAnonymous]
        [HttpGet("interests/{searchString}")]
        public async Task<ApiResponseDto> GetUsersByInterestsAsync(string searchString)
        {
            try
            {
                var response = await _userService.GetUsersByInterestSearchResultAsync(searchString);
                //set status code.
                HttpContext.Response.StatusCode = response.Code;

                if (!response.Success)
                {
                    return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(response);
                }
                else
                {
                    //return data.
                    _apiOkResponse.Data = response.Data;
                    return _apiOkResponse;
                }

            }
            catch (Exception ex)
            {
                //set status code.
                HttpContext.Response.StatusCode = 500;
                _apiErrorResponse.Message = ex.StackTrace;
                return _apiErrorResponse;
            }
        }

        
    }


}