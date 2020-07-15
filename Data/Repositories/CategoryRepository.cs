using LifeLongApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LifeLongApi.Data.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(IdentityAppContext context) : base(context) { }

        public async Task<Category> GetByNameAsync(string categoryName)
        {
            return await context.Set<Category>().FirstOrDefaultAsync(c => c.Name == categoryName);
        }
    }
}