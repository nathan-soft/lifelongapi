using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using LifeLongApi.Dtos;
using LifeLongApi.Dtos.Response;
using LifeLongApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LifeLongApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private static ApiOkResponseDto _apiOkResponse;
        private static ApiErrorResponseDto _apiErrorResponse;
        private readonly IMapper _mapper;

        public AppointmentsController(IMapper mapper, IAppointmentService appointmentService,
                    ApiOkResponseDto apiOkResponse, ApiErrorResponseDto apiErrorResponse)
        {
            _appointmentService = appointmentService;
            _mapper = mapper;
            _apiErrorResponse = apiErrorResponse;
            _apiOkResponse = apiOkResponse;
        }

        [Authorize(Roles = "Mentor")]
        [HttpPost]
        public async Task<ApiResponseDto> NewAppointmentAsync(AppointmentDto appointmentCreds)
        {
            try
            {
                var response = await _appointmentService.AddAppointmentAsync(appointmentCreds);
                //set status code.
                HttpContext.Response.StatusCode = response.Code;

                if (response.Success)
                {
                    //set location of new resource.
                    HttpContext.Response.Headers.Add("Location", $"{Request.Host}/api/appointments/{response.Data.Id}");
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

        [Authorize(Roles = "Mentor")]
        [HttpPut("{appointmentId}")]
        public async Task<ApiResponseDto> ChangeAppointmentAsync(int appointmentId, AppointmentDto appointmentCreds)
        {
            try
            {
                var response = await _appointmentService.EditAppointmentAsync(appointmentId, appointmentCreds);
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

        [Authorize(Roles = "Mentor")]
        [HttpDelete("{appointmentId}")]
        public async Task<ApiResponseDto> DeleteAppointmentAsync(int appointmentId)
        {
            try
            {
                var response = await _appointmentService.DeleteAppointmentAsync(appointmentId);
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