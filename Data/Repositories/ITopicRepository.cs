using LifeLongApi.Models;
using System;
using System.Threading.Tasks;

namespace LifeLongApi.Data.Repositories
{
    public interface ITopicRepository : IGenericRepository<Topic>
    {
        public Task<Topic> GetByNameAsync(string categoryName);
    }
}