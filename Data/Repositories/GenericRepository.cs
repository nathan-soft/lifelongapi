using LifeLongApi.Dtos;
using LifeLongApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LifeLongApi.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T>  where T : class
    {
        protected readonly IdentityAppContext context;
        private DbSet<T> entities;
        string errorMessage = string.Empty;
        public GenericRepository(IdentityAppContext context)
        {
            this.context = context;
            entities = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await entities.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            //return entities.SingleOrDefault(s => s.Id == id);
            return await entities.FindAsync(id);
        }

        public async Task InsertAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            await entities.AddAsync(entity);
            await context.SaveChangesAsync();
        }
        public async Task UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            await context.SaveChangesAsync();
        }
        public async Task DeleteAsync(T entity)
        {
            //T entity = entities.SingleOrDefault(s => s.Id == id);
            //T entity = await entities.FindAsync(id);
            entities.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}