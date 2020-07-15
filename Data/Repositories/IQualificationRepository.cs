using LifeLongApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeLongApi.Data.Repositories
{
    public interface IQualificationRepository : IGenericRepository<Qualification>
    {
        public Task<List<Qualification>> GetUserQualificationsAsync(int userId);
        public Qualification GetQualificationTypeForUser(int userId, string schoolName, string degree, string major);
    }
    
}