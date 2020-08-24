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

        public async Task<List<Follow>> GetAllMentorshipInfoForMentor(int mentorId)
        {
            return await context.Set<Follow>()
                                        .Where(m => m.MentorId == mentorId
                                                    && m.Status == AppHelper.FollowStatus.CONFIRMED.ToString())
                                        .ToListAsync();
        }

        public async Task<List<Follow>> GetAllMentorshipInfoForMentee(int menteeId)
        {
            //returns a list of unique mentors providing guidiance to mentee.
            return await context.Set<Follow>()
                                        .Where(m => m.MenteeId == menteeId 
                                                    && m.Status == AppHelper.FollowStatus.CONFIRMED.ToString())
                                        .ToListAsync();
        }

        public async Task<List<Follow>> GetAllMentorshipRequestAsync()
        {
            return await context.Set<Follow>()
                                        .Where(m => m.Status != AppHelper.FollowStatus.CONFIRMED.ToString())
                                        .ToListAsync();
        }

        public async Task<List<Follow>> GetMentorshipRequestsForMentorAsync(int mentorId)
        {
            return await context.Set<Follow>()
                                        .Where(m => m.MentorId == mentorId
                                                    && m.Status != AppHelper.FollowStatus.CONFIRMED.ToString())
                                        .ToListAsync();
        }

        public async Task<List<Follow>> GetSentMentorshipRequestsForMenteeAsync(int menteeId)
        {
            return await context.Set<Follow>()
                                        .Where(m => m.MenteeId == menteeId
                                                    && m.Status != AppHelper.FollowStatus.CONFIRMED.ToString())
                                        .ToListAsync();
        }

        public async Task<List<int>> GetMenteesIdAsync(int mentorId)
        {
            var FollowInfo =  await GetAllMentorshipInfoForMentor(mentorId);
            return FollowInfo.GroupBy(g => g.MenteeId)
                             .Select(x => x.FirstOrDefault().MenteeId)
                             .ToList();
        }

        public async Task<Follow> GetMentorshipInfo(int menteeId, int mentorId, int topicId)
        {
            return await context.Set<Follow>()
                                        .Where(fr => fr.MenteeId == menteeId
                                                            && fr.MentorId == mentorId
                                                            && fr.TopicId == topicId)
                                        .FirstOrDefaultAsync();
        }

        public async Task<List<Follow>> GetAllMentorshipInfoBetweenUsersAsync(int mentorId, int menteeId)
        {
            return await context.Set<Follow>()
                                       .Where(fr => fr.MenteeId == menteeId
                                                            && fr.MentorId == mentorId
                                                            && fr.Status == AppHelper.FollowStatus.CONFIRMED.ToString())
                                       .ToListAsync();
        }
        
    }
}