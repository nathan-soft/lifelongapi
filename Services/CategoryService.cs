using AutoMapper;
using LifeLongApi.Codes;
using LifeLongApi.Data.Repositories;
using LifeLongApi.Dtos;
using LifeLongApi.Dtos.Response;
using LifeLongApi.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LifeLongApi.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IMapper _mapper;
        public CategoryService(ICategoryRepository categoryRepo, IMapper mapper)
        {
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<List<CategoryDto>>> GetAllCategoriesAsync(){
            var categories =  await _categoryRepo.GetAllAsync();
            categories = categories.ToList();

            var sr = new ServiceResponse<List<CategoryDto>>();
            sr.Code = StatusCodes.Status200OK;
            sr.Data = _mapper.Map<List<CategoryDto>>(categories);
            sr.Success = true;
            return sr;
        }
        public async Task<ServiceResponse<CategoryDto>> GetCategoryByIdAsync(int id){
            var category = await _categoryRepo.GetByIdAsync(id);
            var sr = new ServiceResponse<CategoryDto>();
            if(category != null){
                sr.Code = StatusCodes.Status200OK;
                sr.Data = _mapper.Map<CategoryDto>(category);
                sr.Success = true;
            }else{
                sr.Code = StatusCodes.Status404NotFound;
                sr.Message = "category does not exist.";
                sr.Success = false;
            }
            return sr;
        }
        public async Task<ServiceResponse<CategoryDto>> GetCategoryByNameAsync(string categoryName){
            var category = await _categoryRepo.GetByNameAsync(categoryName);
            var sr = new ServiceResponse<CategoryDto>();
            if (category != null)
            {
                sr.Code = StatusCodes.Status200OK;
                sr.Data = _mapper.Map<CategoryDto>(category);
                sr.Success = true;
            }
            else
            {
                //Error: category name does not exist in db.
                sr.Code = StatusCodes.Status404NotFound;
                sr.Message = "category name does not exist.";
                sr.Success = false;
            }
            return sr;
        }
        public async Task<ServiceResponse<CategoryDto>> AddCategoryAsync(string categoryName){
            var category = await _categoryRepo.GetByNameAsync(categoryName);
            var sr = new ServiceResponse<CategoryDto>();
            if (category == null)
            {
                sr.Code = StatusCodes.Status404NotFound;
                sr.Success = false;
            }else{
                var newCategory = new Category
                {
                    Name = categoryName
                };

                //insert new category
                await _categoryRepo.InsertAsync(newCategory);
                sr.Code = StatusCodes.Status201Created;
                sr.Data = _mapper.Map<CategoryDto>(await _categoryRepo.GetByNameAsync(categoryName));
                sr.Success = true;
            }
            return sr;
        }

        public async Task<ServiceResponse<CategoryDto>> UpdateCategoryAsync(string oldCategoryName, string newCategoryName){
            //get category.
            var category = await GetCategoryByNameAsync(oldCategoryName);
            //var category = await _categoryRepo.GetByNameAsync(categoryName);
            var sr = new ServiceResponse<CategoryDto>();
            if(category.Data != null){
                //set new category name.
                category.Data.Name = newCategoryName;
                //convert and save the changes made;
                await _categoryRepo.UpdateAsync(_mapper.Map<Category>(category));
                //set the new category name
                sr.Data.Name = newCategoryName;
            }
            return sr;
        }
        public async Task DeleteCategoryAsync(int categoryId){
            var category = await _categoryRepo.GetByIdAsync(categoryId);
            await _categoryRepo.DeleteAsync(category);
        }
    }
}