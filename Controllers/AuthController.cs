using AutoMapper;
using LifeLongApi.Dtos;
using LifeLongApi.Dtos.Response;
using LifeLongApi.Services;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LifeLongApi.Controllers {
        [ApiController]
        [Route ("api/[controller]")]
        public class AuthController : ControllerBase {
            private readonly IUserService _userService;
            private readonly IAuthService _authService;
             private static ApiOkResponseDto _apiOkResponse;
            private static ApiErrorResponseDto _apiErrorResponse;
            private readonly IMapper _mapper;

            public AuthController (IAuthService authService, IMapper mapper, IUserService userService, 
                        ApiOkResponseDto apiOkResponse, ApiErrorResponseDto apiErrorResponse) 
            {
                _authService = authService;
                _userService = userService;
                _mapper = mapper;
                _apiErrorResponse = apiErrorResponse;
                _apiOkResponse = apiOkResponse;
            }

            [HttpPost("login")]
            public async Task<ApiResponseDto> Login (LoginDto loginCreds) {
                var result = await _authService.Login (loginCreds.Username, loginCreds.Password);
                //set the http response code.
                HttpContext.Response.StatusCode = result.Code;
                if (result.Data != null) {
                     _apiOkResponse = _mapper.Map<ApiOkResponseDto>(result);
                    return _apiOkResponse;
                } else {
                    //error occurred
                    _apiErrorResponse = _mapper.Map<ApiErrorResponseDto> (result);
                    return _apiErrorResponse;
                }
            }

            [HttpPost("register")]
            public async Task<ApiResponseDto> Register (RegisterDto userInfo) {
                //try creating the user.
                var result = await _userService.CreateUserAsync(userInfo);
                 //set status code.
                HttpContext.Response.StatusCode = result.Code;
                if (result.Data != null)
                {
                //user was created successfully
                //set  location header of newly created user.
                Response
                        .Headers
                        .Add(HeaderNames.Location, $"{HttpContext.Request.Host}/api/users/{result.Data.Email}");
                    //Take of password before returning data.
                    result.Data.Password = null;
                    return _apiOkResponse = _mapper.Map<ApiOkResponseDto>(result);
                }else{
                    //error
                    //return error.
                    return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(result);
                }
            }
        }
}