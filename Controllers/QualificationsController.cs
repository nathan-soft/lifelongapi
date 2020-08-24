using AutoMapper;
using LifeLongApi.Dtos;
using LifeLongApi.Dtos.Response;
using LifeLongApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class QualificationsController : ControllerBase
    {
        private readonly IUserService _userService;
        private static ApiOkResponseDto _apiOkResponse;
        private static ApiErrorResponseDto _apiErrorResponse;
        private readonly IMapper _mapper;

        public QualificationsController(IMapper mapper, IUserService userService,
                    ApiOkResponseDto apiOkResponse, ApiErrorResponseDto apiErrorResponse)
        {
            _userService = userService;
            _mapper = mapper;
            _apiErrorResponse = apiErrorResponse;
            _apiOkResponse = apiOkResponse;
        }

        [HttpPost]
        public async Task<ApiResponseDto> AddQualificationForUser(QualificationDto userQualification)
        {
            try
            {
                var response = await _userService.AddQualificationForUserAsync(userQualification);
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
                //set status code.
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                //log and return default custom error
                _apiErrorResponse.Message = ex.Message;
                return _apiErrorResponse;
            }
        }

        [HttpPut("{qualificationId}")]
        public async Task<ApiResponseDto> UpdatUserQualification(int qualificationId, QualificationDto userQualification)
        {
            try
            {
                var response = await _userService.UpdateUserQualificationAsync(qualificationId, userQualification);
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

        [HttpDelete("{qualificationId}")]
        public async Task<ApiResponseDto> DeleteQualification(int qualificationId)
        {
            try
            {
                var response = await _userService.DeleteUserQualificationAsync(qualificationId);
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

    }
}