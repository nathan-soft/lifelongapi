using LifeLongApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LifeLongApi.Data.Repositories
{
    public interface IArticleRepository : IGenericRepository<Article>
    {
        Task<IEnumerable<Article>> GetArticlesByAuthorAsync(int authorId);
        Task<IEnumerable<Article>> GetArticlesByTagAsync(int tagId);
        Task<IEnumerable<Article>> GetArticlesByTitleAsync(string wholeOrPartPhrase);
    }

    public class ArticleRepository : GenericRepository<Article>, IArticleRepository
    {
        public ArticleRepository(IdentityAppContext context) : base(context) { }

        public async Task<IEnumerable<Article>> GetArticlesByAuthorAsync(int authorId)
        {
            return await context.Set<Article>()
                                .Where(u => u.AuthorId == authorId)
                                .ToListAsync();
        }

        public async Task<IEnumerable<Article>> GetArticlesByTagAsync(int tagId)
        {
            return await context.Set<Article>()
                                .Where(u => u.ArticleTags.Any(a => a.TopicId == tagId))
                                .ToListAsync();
        }

        public async Task<IEnumerable<Article>> GetArticlesByTitleAsync(string wholeOrPartPhrase)
        {
            return await context.Set<Article>()
                                .Where(u => u.Title.Contains(wholeOrPartPhrase))
                                .ToListAsync();
        }
    }
}
