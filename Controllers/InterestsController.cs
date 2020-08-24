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
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InterestsController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ITopicService _topicService;
        private static ApiOkResponseDto _apiOkResponse;
        private static ApiErrorResponseDto _apiErrorResponse;
        private readonly IMapper _mapper;

        public InterestsController(IMapper mapper, ICategoryService categoryService,
                    ApiOkResponseDto apiOkResponse, ApiErrorResponseDto apiErrorResponse, ITopicService topicService)
        {
            _categoryService = categoryService;
            _topicService = topicService;
            _mapper = mapper;
            _apiErrorResponse = apiErrorResponse;
            _apiOkResponse = apiOkResponse;
        }

        [HttpGet("categories")]
        public async Task<ApiResponseDto> GetCategoriesAsync()
        {
            var categoryDto = await _categoryService.GetAllCategoriesAsync();
            //set status code.
            HttpContext.Response.StatusCode = categoryDto.Code;
            if (!categoryDto.Success)
            {
                //error
                //return error.
                return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(categoryDto);
            }
            else
            {
                //return data.
                _apiOkResponse.Data = categoryDto.Data;
                return _apiOkResponse;
            }
        }

        [HttpGet("topics")]
        public async Task<ApiResponseDto> GetTopicsAsync()
        {
            var topicDto = await _topicService.GetAllFieldOfInterestAsync();
            //set status code.
            HttpContext.Response.StatusCode = topicDto.Code;
            if (!topicDto.Success)
            {
                //error
                //return error.
                return _apiErrorResponse = _mapper.Map<ApiErrorResponseDto>(topicDto);
            }
            else
            {
                //return data.
                _apiOkResponse.Data = topicDto.Data;
                return _apiOkResponse;
            }
        }

    }
}