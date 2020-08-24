using LifeLongApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace LifeLongApi.Data.Repositories
{
    public class TopicRepository : GenericRepository<Topic>, ITopicRepository
    {
        public TopicRepository(IdentityAppContext context) : base(context) { }

        public async Task<Topic> GetByNameAsync(string fieldOfInterest)
        {
            return await context.Set<Topic>().FirstOrDefaultAsync(f => f.Name.ToLower().Contains(fieldOfInterest.ToLower()));
        }
    }
}