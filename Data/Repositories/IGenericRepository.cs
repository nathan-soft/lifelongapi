using LifeLongApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace LifeLongApi.Data.Repositories {
    public interface IGenericRepository<T> {
        public Task<IEnumerable<T>> GetAllAsync ();
        public Task<T> GetByIdAsync (int id);
        public Task InsertAsync (T entity);
        public Task UpdateAsync (T entity);
        public Task DeleteAsync (T entity);
    }
}