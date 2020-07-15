using LifeLongApi.Codes;
using LifeLongApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LifeLongApi.Data.Repositories
{
    public class FollowRepository : GenericRepository<Follow>, IFollowRepository
    {
        public FollowRepository(IdentityAppContext context) : base(context) { }

        //gets all mentees who are currently receiving mentorship from mentor
        public async Task<List<Follow>> GetAllMenteeRelationshipAsync(int mentorId)
        {
            return await context.Set<Follow>()
                                        .Where(m => m.FollowingMentorId == mentorId
                                                    && m.Status == AppHelper.FollowStatus.CONFIRMED.ToString())
                                        .ToListAsync();
        }

        //gets all mentor who are mentoring this mentee.
        public async Task<List<Follow>> GetAllMentorRelationshipAsync(int menteeId)
        {
            return await context.Set<Follow>()
                                        .Where(m => m.UserId == menteeId 
                                                    && m.Status == AppHelper.FollowStatus.CONFIRMED.ToString())
                                        .ToListAsync();
        }

        public async Task<List<Follow>> GetAllMentorshipRequestAsync()
        {
            return await context.Set<Follow>()
                                        .Where(m => m.Status != AppHelper.FollowStatus.CONFIRMED.ToString())
                                        .ToListAsync();
        }

        public async Task<List<Follow>> GetMentorshipRequestsForUserAsync(int mentorId)
        {
            return await context.Set<Follow>()
                                        .Where(m => m.FollowingMentorId == mentorId
                                                    && m.Status != AppHelper.FollowStatus.CONFIRMED.ToString())
                                        .ToListAsync();
        }

        public List<int> GetMenteesId(int mentorId)
        {
            return  GetAllMenteeRelationshipAsync(mentorId)
                                        .Result
                                        .GroupBy(g => g.UserId).Select(x => x.FirstOrDefault().UserId)
                                        .ToList();
        }

        public async Task<Follow> GetFollowRelationshipAsync(int menteeId, int mentorId, int topicId)
        {
            return await context.Set<Follow>()
                                        .Where(fr => fr.UserId == menteeId
                                                            && fr.FollowingMentorId == mentorId
                                                            && fr.TopicId == topicId)
                                        .FirstOrDefaultAsync();
        }
    }
}