using LifeLongApi.Codes;
using LifeLongApi.Dtos;
using LifeLongApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeLongApi.Services
{
    public interface ICategoryService
    {
         public Task<ServiceResponse<List<CategoryDto>>> GetAllCategoriesAsync();
        public Task<ServiceResponse<CategoryDto>> GetCategoryByIdAsync(int id);
        public Task<ServiceResponse<CategoryDto>> GetCategoryByNameAsync(string categoryName);
        public Task<ServiceResponse<CategoryDto>> AddCategoryAsync(string categoryName);
        public Task<ServiceResponse<CategoryDto>> UpdateCategoryAsync(string oldCategoryName, string newCategoryName);
        public Task DeleteCategoryAsync(int categoryId);
    }
}