using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LifeLongApi.Dtos;
using LifeLongApi.Dtos.Response;
using LifeLongApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LifeLongApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private static ApiOkResponseDto _apiOkResponse;
        private static ApiErrorResponseDto _apiErrorResponse;
        private readonly IMapper _mapper;

        public BlogsController(IMapper mapper, IArticleService articleService,
                    ApiOkResponseDto apiOkResponse, ApiErrorResponseDto apiErrorResponse)
        {
            _articleService = articleService;
            _mapper = mapper;
            _apiErrorResponse = apiErrorResponse;
            _apiOkResponse = apiOkResponse;
        }

        // GET: api/<BlogsController>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ApiResponseDto> GetAllBlogPosts()
        {
            try
            {
                var response = await _articleService.GetArticlesAsync();
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

        [AllowAnonymous]
        // GET api/<BlogsController>/5
        [HttpGet("{postId}")]
        public async Task<ApiResponseDto> GetBlogPostAsync(int postId)
        {
            try
            {
                var response = await _articleService.GetArticleByIdAsync(postId);
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

        [AllowAnonymous]
        // GET api/<BlogsController>/Author/authorName
        [HttpGet("Authors/{authorEmail}")]
        public async Task<ApiResponseDto> GetBlogPostByAuthorAsync(string authorEmail)
        {
            try
            {
                var response = await _articleService.GetArticlesByAuthorAsync(authorEmail);
                //set status code.
                HttpContext.Response.StatusCode = response.Code;

                if (response.Success)
                {
                    //return data.
                    _apiOkResponse.Data = response.Data.ToList();
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

        [AllowAnonymous]
        // GET api/<BlogsController>/Tags/tagName
        [HttpGet("Tags/{tagName}")]
        public async Task<ApiResponseDto> GetBlogPostByTopicAsync(string tagName)
        {
            try
            {
                var response = await _articleService.GetArticlesByTagAsync(tagName);
                //set status code.
                HttpContext.Response.StatusCode = response.Code;

                if (response.Success)
                {
                    //return data.
                    _apiOkResponse.Data = response.Data.ToList();
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

        // POST api/<BlogsController>
        [HttpPost]
        public async Task<ApiResponseDto> NewBlogPostAsync([FromForm]ArticleRequestDto blogPost)
        {
            try
            {
                var response = await _articleService.AddArticleAsync(blogPost);
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

        // PUT api/<BlogsController>/5
        [HttpPut("{id}")]
        public async Task<ApiResponseDto> UpdateBlogPostAsync(int postId, [FromForm] ArticleRequestDto UpdatedBlogPost)
        {
            try
            {
                var response = await _articleService.AddArticleAsync(UpdatedBlogPost);
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

        // DELETE api/<BlogsController>/5
        [HttpDelete("{id}")]
        public async Task<ApiResponseDto> DeleteAsync(int postId)
        {
            var response = await _articleService.DeleteArticleAsync(postId);
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
    }
}
