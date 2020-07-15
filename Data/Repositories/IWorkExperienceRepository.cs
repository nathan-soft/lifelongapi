using LifeLongApi.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LifeLongApi.Data.Repositories
{
    public interface IWorkExperienceRepository : IGenericRepository<WorkExperience>
    {
        public Task<List<WorkExperience>> GetAllForUserAsync(int userId);
        public WorkExperience GetSpecific(int userId, int topicId, string companyName, int startYear);
        
    }
}