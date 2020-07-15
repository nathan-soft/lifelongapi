using LifeLongApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeLongApi.Data.Repositories
{
    public interface IFollowRepository : IGenericRepository<Follow>
    {
        Task<List<Follow>> GetAllMenteeRelationshipAsync(int mentorId);
        Task<List<Follow>> GetAllMentorRelationshipAsync(int menteeId);
        Task<List<Follow>> GetAllMentorshipRequestAsync();

        // public Task AddFollowAsync(Follow followCred);
        // public Task<Follow> UpdateAsync(Follow followCred);
        //public Task DeleteFollowAsync(int mentorId, int menteeId, int topicId);
        public Task<Follow> GetFollowRelationshipAsync(int menteeId, int mentorId, int topicId);
        List<int> GetMenteesId(int mentorId);
        Task<List<Follow>> GetMentorshipRequestsForUserAsync(int mentorId);
    }
}