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
    public class WorkExperienceController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWorkExperienceService _workExperienceService;
        private static ApiOkResponseDto _apiOkResponse;
        private static ApiErrorResponseDto _apiErrorResponse;
        private readonly IMapper _mapper;

        public WorkExperienceController(IMapper mapper, IUserService userService,
                    ApiOkResponseDto apiOkResponse, ApiErrorResponseDto apiErrorResponse, 
                    IWorkExperienceService workExperienceService)
        {
            _userService = userService;
            _mapper = mapper;
            _apiErrorResponse = apiErrorResponse;
            _apiOkResponse = apiOkResponse;
            _workExperienceService = workExperienceService;
        }

        [HttpPost]
        public async Task<ApiResponseDto> AddWorkExperience(WorkExperienceDto userWorkExperience)
        {
            try
            {
                var response = await _workExperienceService.AddAsync(userWorkExperience);
                //set status code.
                HttpContext.Response.StatusCode = response.Code;
                if (!response.Success)
                {
                    return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(response);
                }
                else
                {
                    HttpContext.Response.Headers.Add("Location", $"{Request.Host}/api/workexperience/{response.Data.Id}");
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
        public async Task<ApiResponseDto> UpdatWorkExperience(int workExperienceId, WorkExperienceDto workExperience)
        {
            try
            {
                var response = await _workExperienceService.UpdateAsync(workExperienceId, workExperience);
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

        [HttpDelete("{workExperienceId}")]
        public async Task<ApiResponseDto> DeleteWorkExperience(int workExperienceId)
        {
            try
            {
                var response = await _workExperienceService.DeleteWorkExperienceAsync(workExperienceId);
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