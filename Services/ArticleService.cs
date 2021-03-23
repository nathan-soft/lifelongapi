using AutoMapper;
using LifeLongApi.Codes;
using LifeLongApi.Data.Repositories;
using LifeLongApi.Dtos;
using LifeLongApi.Dtos.Response;
using LifeLongApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LifeLongApi.Services
{
    public interface IArticleService
    {
        Task<ServiceResponse<ArticleResponseDto>> AddArticleAsync(ArticleRequestDto article);
        Task<ServiceResponse<ArticleResponseDto>> DeleteArticleAsync(int id);
        Task<ServiceResponse<ArticleResponseDto>> GetArticleByIdAsync(int id);
        Task<ServiceResponse<List<ArticleResponseDto>>> GetArticlesAsync();
        Task<ServiceResponse<IEnumerable<ArticleResponseDto>>> GetArticlesByAuthorAsync(string username);
        Task<ServiceResponse<IEnumerable<ArticleResponseDto>>> GetArticlesByTagAsync(string topicName);
        Task<ServiceResponse<List<ArticleResponseDto>>> GetArticlesByTitleAsync(string searchString);
    }

    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepo;
        private readonly ITopicRepository _topicRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IBlobService _blobService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ArticleService(IArticleRepository articleRepo, ITopicRepository topicRepo, UserManager<AppUser> userManager, IMapper mapper, IBlobService blobService, IHttpContextAccessor httpContextAccessor)
        {
            _articleRepo = articleRepo;
            _blobService = blobService;
            _httpContextAccessor = httpContextAccessor;
            _topicRepo = topicRepo;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<ArticleResponseDto>>> GetArticlesAsync()
        {
            var articles = await _articleRepo.GetAllAsync();
            //categories = categories.ToList();

            var sr = new ServiceResponse<List<ArticleResponseDto>>();
            sr.Code = StatusCodes.Status200OK;
            sr.Data = _mapper.Map<List<ArticleResponseDto>>(articles);
            sr.Success = true;
            return sr;
        }
        public async Task<ServiceResponse<ArticleResponseDto>> GetArticleByIdAsync(int id)
        {
            var article = await _articleRepo.GetByIdAsync(id);
            var sr = new ServiceResponse<ArticleResponseDto>();
            if (article == null)
            {
                return sr.HelperMethod(StatusCodes.Status404NotFound, "Article does not exist.", false);
            }

            sr.Code = StatusCodes.Status200OK;
            sr.Data = _mapper.Map<ArticleResponseDto>(article);
            sr.Success = true;
            return sr;
        }

        public async Task<ServiceResponse<List<ArticleResponseDto>>> GetArticlesByTitleAsync(string searchString)
        {
            var sr = new ServiceResponse<List<ArticleResponseDto>>();

            var articles = await _articleRepo.GetArticlesByTitleAsync(searchString);

            sr.Code = StatusCodes.Status200OK;
            sr.Data = _mapper.Map<List<ArticleResponseDto>>(articles);
            sr.Success = true;
            return sr;
        }

        public async Task<ServiceResponse<IEnumerable<ArticleResponseDto>>> GetArticlesByAuthorAsync(string username)
        {
            var foundUser = await _userManager.FindByNameAsync(username);
            var sr = new ServiceResponse<IEnumerable<ArticleResponseDto>>();
            if (foundUser == null)
            {
                return sr.HelperMethod(404, "author does not exist.", false);
            }

            var articles = await _articleRepo.GetArticlesByAuthorAsync(foundUser.Id);

            sr.Code = StatusCodes.Status200OK;
            sr.Data = _mapper.Map<List<ArticleResponseDto>>(articles);
            sr.Success = true;
            return sr;
        }

        public async Task<ServiceResponse<IEnumerable<ArticleResponseDto>>> GetArticlesByTagAsync(string topicName)
        {
            var topic = await _topicRepo.GetByNameAsync(topicName);
            var sr = new ServiceResponse<IEnumerable<ArticleResponseDto>>();
            if (topic == null)
            {
                return sr.HelperMethod(404, "Tag name does not exist.", false);
            }

            var articles = await _articleRepo.GetArticlesByTagAsync(topic.Id);

            sr.Code = StatusCodes.Status200OK;
            sr.Data = _mapper.Map<List<ArticleResponseDto>>(articles);
            sr.Success = true;
            return sr;
        }

        public async Task<ServiceResponse<ArticleResponseDto>> AddArticleAsync(ArticleRequestDto article)
        {
            var sr = new ServiceResponse<ArticleResponseDto>();
            //VALIDATIONS
            //validate uploaded Image

            var imgValidationResult = ValidateImage(article.UploadedImage);
            if (imgValidationResult != "Valid")
            {
                return sr.HelperMethod(400, imgValidationResult, false);
            }

            //create random name
            var newFileName = GetUniqueFileName(article.UploadedImage.FileName);

            //validate author
            var username = _httpContextAccessor.GetUsernameOfCurrentUser();
            var author = await _userManager.FindByNameAsync(username);

            if (author == null)
            {
                return sr.HelperMethod(404, "Author not found.", false);
            }

            //validate Article Tags.
            List<ArticleTag> articleTags = null;

            try
            {
                articleTags = await IsAllValidTagIdAsync(article.Tags);
            }
            catch (KeyNotFoundException)
            {
                return sr.HelperMethod(404, "One of the Id's is not a valid tag id. Please confirm and try again.", false);
            }

            //upload image to azure storage.
            var imageUrl = await _blobService.UploadFileBlobAsync("blog-images",
                                                                  article.UploadedImage.OpenReadStream(),
                                                                  newFileName);

            //Process.
            //convert to "Article"
            var newArticle = _mapper.Map<Article>(article);
            newArticle.AuthorId = author.Id;
            newArticle.ImageUrl = imageUrl;
            newArticle.ArticleTags = articleTags;

            //insert new Article
            await _articleRepo.InsertAsync(newArticle);
            sr.Code = StatusCodes.Status201Created;
            sr.Data = _mapper.Map<ArticleResponseDto>(newArticle);
            sr.Success = true;

            return sr;
        }

        public async Task<ServiceResponse<ArticleResponseDto>> DeleteArticleAsync(int id)
        {
            var article = await _articleRepo.GetByIdAsync(id);
            var sr = new ServiceResponse<ArticleResponseDto>();
            if (article == null)
            {
                return sr.HelperMethod(StatusCodes.Status404NotFound, "Article does not exist.", false);
            }

            //Delete Image from cloud
            await _blobService.DeleteBlobAsync("blog-images", Path.GetFileName(article.ImageUrl));

            //Delete article from db.
            await _articleRepo.DeleteAsync(article);

            return sr.HelperMethod(200, "Delete Successful.", true);
        }

        private bool ImageGreaterThan2Mb(long imgSize)
        {
            //1 Kb = 1024 Byte
            //1 Mb = 1024 Kb
            //1 Mb = 1024x1024 Byte
            //1 Mb = 1 048 576 Byte

            if (imgSize > (1048576 * 2))
                return true;
            else
                return false;
        }

        private string ValidateImage(IFormFile uploadedImage)
        {
            //valid extensions.
            string[] permittedExtensions = { ".jpg", ".jpeg", ".png" };

            var ext = Path.GetExtension(uploadedImage.FileName).ToLowerInvariant();
            //check valid extensions.
            if (!permittedExtensions.Contains(ext))
            {
                return "Only accepting '.jpeg, '.jpg' or '.png' images.";
            }
            //check content type and others...
            if (!uploadedImage.IsImage())
            {
                return "Please upload an image.";
            }
            //check image size.
            if (ImageGreaterThan2Mb(uploadedImage.Length))
            {
                return "Maximum allowed file size is 2MB.";
            }
            return "Valid";
        }

        /// <summary>
        /// Checks if all the tag ids are valid tag ids. Throws a <see cref="KeyNotFoundException"/> if any is not.
        /// </summary>
        /// <param name="tagIds">The tag ids to perform check against.</param>
        /// <returns>A task that resolves to <see cref="List{ArticleTag}"/> when completed.</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        private async Task<List<ArticleTag>> IsAllValidTagIdAsync(List<string> tags)
        {
            List<ArticleTag> articleTags = new List<ArticleTag>();

            foreach (var tagName in tags)
            {
                //check if Id exist in the "topic table" in the DB
                var tag = await _topicRepo.GetByNameAsync(tagName);
                if (tag == null)
                {
                    //not a valid topic id.
                    throw new KeyNotFoundException();
                }

                articleTags.Add(new ArticleTag
                {
                    TopicId = tag.Id
                });
            }

            return articleTags;
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }

    }

}
