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

namespace LifeLongApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAppointmentService _appointmentService;
        private static ApiOkResponseDto _apiOkResponse;
        private static ApiErrorResponseDto _apiErrorResponse;
        private readonly IMapper _mapper;

        public UsersController(IMapper mapper, IUserService userService, IAppointmentService appointmentService,
                    ApiOkResponseDto apiOkResponse, ApiErrorResponseDto apiErrorResponse)
        {
            _appointmentService = appointmentService;
            _userService = userService;
            _mapper = mapper;
            _apiErrorResponse = apiErrorResponse;
            _apiOkResponse = apiOkResponse;
        }

        [HttpGet("{username}")]
        public async Task<ApiResponseDto> GetUserAsync(string username)
        {
            try
            {
                var userDto = await _userService.GetUserByUsernameWithRelationshipsAsync(username);
                //set status code.
                HttpContext.Response.StatusCode = userDto.Code;
                if (userDto.Success)
                {
                    //return data.
                    _apiOkResponse.Data = userDto.Data;
                    return _apiOkResponse;
                }
                else
                {
                    //error
                    //return error.
                    return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(userDto);
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

        [HttpPut("{username}/edit")]
        public async Task<ApiResponseDto> EditUserAsync(string username, UserDto userCreds)
        {
            try
            {
                var result = await _userService.UpdateUserAsync(username, userCreds);
                //set status code.
                HttpContext.Response.StatusCode = result.Code;
                if (result.Data != null)
                {
                    //update was successful.
                    return _apiOkResponse = _mapper.Map<ApiOkResponseDto>(result);
                }
                else
                {
                    //error
                    return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(result);
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

        [HttpGet("{username}/requests")]
        public async Task<ApiResponseDto> GetMentorshipRequestsForMentorAsync(string username)
        {
            try
            {
                var response = await _userService.GetMentorshipRequestsAsync(username);
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
                //set status code.
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                //log and return default custom error
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
                //set status code.
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                //log and return default custom error
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
                //set status code.
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                //log and return default custom error
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
                //set status code.
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                //log and return default custom error
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
                //set status code.
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                //log and return default custom error
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
                //set status code.
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                //log and return default custom error
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
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                //log and return default custom error
                _apiErrorResponse.Message = ex.Message;
                return _apiErrorResponse;
            }
        }


        [HttpGet("{mentorUsername}/appointments/{appointmentStatus}")]
        public async Task<ApiResponseDto> GetMentorAppointmentsAsync(string mentorUsername, string appointmentStatus)
        {
            try
            {
                var response = await _appointmentService.GetMentorsAppointmentsAsync(mentorUsername, appointmentStatus);
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
            catch (Exception ex)
            {
                //set status code.
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                //log and return default custom error
                _apiErrorResponse.Message = ex.Message;
                return _apiErrorResponse;
            }
        }
    }


}