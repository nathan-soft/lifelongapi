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
using System.Web;

namespace LifeLongApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IAppointmentService _appointmentService;
        private readonly INotificationService _notificationService;
        private static ApiOkResponseDto _apiOkResponse;
        private static ApiErrorResponseDto _apiErrorResponse;
        private readonly IMapper _mapper;

        public UsersController(IMapper mapper, IUserService userService, IRoleService roleService, IAppointmentService appointmentService, INotificationService notificationService, ApiOkResponseDto apiOkResponse, ApiErrorResponseDto apiErrorResponse)
        {
            _appointmentService = appointmentService;
            _userService = userService;
            _roleService = roleService;
            _notificationService = notificationService;
            _mapper = mapper;
            _apiErrorResponse = apiErrorResponse;
            _apiOkResponse = apiOkResponse;
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet]
        public async Task<ApiResponseDto> GetNonAdministrativeUsersAsync(string searchString)
        {
            var users = await _userService.GetNonAdministrativeUsersAsync(searchString);
            //set status code.
            HttpContext.Response.StatusCode = users.Code;
            //return data.
            _apiOkResponse.Data = users.Data;
            return _apiOkResponse;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admins")]
        public async Task<ApiResponseDto> GetAdministrativeUsersAsync()
        {
            var users = await _userService.GetAdministrativeUsersAsync();
            //set status code.
            HttpContext.Response.StatusCode = users.Code;
            //return data.
            _apiOkResponse.Data = users.Data;
            return _apiOkResponse;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ApiResponseDto> CreateUser(CreateUserDto userInfo)
        {
            try
            {
                //try creating the user.
                var result = await _userService.CreateUserAsync(userInfo, userInfo.UserRoles.ToArray());
                //set status code.
                HttpContext.Response.StatusCode = result.Code;
                if (result.Success)
                {
                    //user was created successfully
                    //set  location header of newly created user.
                    Response
                            .Headers
                            .Add(HeaderNames.Location, $"{HttpContext.Request.Host}/api/users/{result.Data.Email}");
                    return _apiOkResponse = _mapper.Map<ApiOkResponseDto>(result);
                }
                else
                {
                    //error
                    //return error.
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

        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet("roles")]
        public async Task<ApiResponseDto> GetAdminRolesAsync()
        {
            try
            {
                //try creating the user.
                var result = await _roleService.GetAdminRolesAsync();
                //set status code.
                HttpContext.Response.StatusCode = result.Code;
                if (result.Success)
                {
                    return _apiOkResponse = _mapper.Map<ApiOkResponseDto>(result);
                }
                else
                {
                    //error
                    //return error.
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

        [HttpGet("{username}")]
        public async Task<ApiResponseDto> GetUserAsync(string username)
        {
            try
            {
                var userDto = await _userService.GetUserByUsernameAsync(username);
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

        [HttpPut("{username}")]
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

        [HttpDelete("{username}")]
        public async Task<ApiResponseDto> DeleteUserAsync(string username)
        {
            try
            {
                var response = await _userService.DeleteUserAsync(username);
                //set status code.
                HttpContext.Response.StatusCode = response.Code;
                if (response.Success)
                {
                    //return data.
                    _apiOkResponse.Data = response.Message;
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

        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost("SendMail")]
        public async Task<ApiResponseDto> SendUserMailAsync(MailDto mailInfo)
        {
            try
            {
                var response = await _userService.SendMailToUserAsync(mailInfo);
                //set status code.
                HttpContext.Response.StatusCode = response.Code;
                if (response.Success)
                {
                    //return data.
                    _apiOkResponse.Data = response.Message;
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

        [HttpGet("{username}/notifications")]
        public async Task<ApiResponseDto> GetUserNotifications(string username)
        {
            try
            {
                var response = await _notificationService.GetUserNotificationsAsync(username);
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
        public async Task<ApiResponseDto> GetMentorsByInterestsAsync(string searchString)
        {
            searchString = HttpUtility.UrlDecode(searchString);
            try
            {
                var response = await _userService.GetMentorsByFieldOfInterestAsync(searchString);
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
                var response = await _appointmentService.GetMentorAppointmentsAsync(mentorUsername,
                                                                                                   appointmentStatus);
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