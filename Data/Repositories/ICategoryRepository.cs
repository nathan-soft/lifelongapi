using LifeLongApi.Models;
using System;
using System.Threading.Tasks;

namespace LifeLongApi.Data.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        public Task<Category> GetByNameAsync(string categoryName);
    }
}