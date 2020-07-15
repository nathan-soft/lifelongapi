using LifeLongApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LifeLongApi.Data.Repositories
{
    public class QualificationRepository : GenericRepository<Qualification>, IQualificationRepository
    {
        public QualificationRepository(IdentityAppContext context) : base(context) { }
        public async Task<List<Qualification>> GetUserQualificationsAsync(int userId){
            return await context.Set<Qualification>()
                                .Where(q => q.UserId == userId)
                                .ToListAsync();
        }

        public Qualification GetQualificationTypeForUser(int userId, string schoolName, string degree, string major)
        {
            return context.Set<Qualification>()
                                .Where(q => q.UserId == userId && q.SchoolName == schoolName && q.QualificationType == degree && q.Major == major).FirstOrDefault();
        }
    }
}