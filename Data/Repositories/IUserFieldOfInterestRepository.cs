using LifeLongApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace LifeLongApi.Data.Repositories
{
    public interface IUserFieldOfInterestRepository : IGenericRepository<UserFieldOfInterest>
    {
        public List<int> GetFieldOfInterestIdsForUser(int userId);
        public Task<List<UserFieldOfInterest>> GetFieldOfInterestsForUserAsync(int userId);
        Task<List<AppUser>> GetUsersForFieldOfInterestAsync(int fieldOfInterestId);
        public Task<bool> IsFieldPartOfUserInterestsAsync(int userId, int fieldOfInterestId);
    }
}