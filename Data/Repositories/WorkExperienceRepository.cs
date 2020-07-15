using LifeLongApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace LifeLongApi.Data.Repositories
{
    public class WorkExperienceRepository : GenericRepository<WorkExperience>, IWorkExperienceRepository
    {
        public WorkExperienceRepository(IdentityAppContext context) : base(context) { }
        public async Task<List<WorkExperience>> GetAllForUserAsync(int userId){
            return await context.Set<WorkExperience>()
                                .Where(we => we.UserId == userId)
                                .ToListAsync();
        }

        public WorkExperience GetSpecific(int userId, int topicId, string companyName, int startYear)
        {
            return context.Set<WorkExperience>()
                            .Where(we =>
                                    we.UserId == userId
                                    && we.TopicId == topicId
                                    && we.CompanyName == companyName
                                    && we.StartYear == startYear
                                ).FirstOrDefault();
        }
    }
}