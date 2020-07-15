using LifeLongApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace LifeLongApi.Data.Repositories
{
    public class UserFieldOfInterestRepository : GenericRepository<UserFieldOfInterest>, IUserFieldOfInterestRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IFollowRepository _followRepo;
        public UserFieldOfInterestRepository(IdentityAppContext context, UserManager<AppUser> userManager, IFollowRepository followRepo) : base(context)
        {
            _userManager = userManager;
            _followRepo = followRepo;
        }

        public  List<int> GetFieldOfInterestIdsForUser(int userId)
        {
            var fieldOfInterests =   context.Set<UserFieldOfInterest>()
                                .Where(ufoi => ufoi.UserId == userId)
                                .Select(t => t.TopicId)
                                .ToList();

            return fieldOfInterests;
        }

        public async Task<List<UserFieldOfInterest>> GetFieldOfInterestsForUserAsync(int userId)
        {
            var fieldOfInterests = await context.Set<UserFieldOfInterest>()
                                .Where(ufoi => ufoi.UserId == userId)
                                .Include(ufoi => ufoi.Topic)
                                .ToListAsync();

            return fieldOfInterests;
        }

        public async Task<bool> IsFieldPartOfUserInterestsAsync(int userId, int fieldOfInterestId)
        {
            var fieldOfInterest = await context.Set<UserFieldOfInterest>()
                                .Where(ufoi => ufoi.UserId == userId && ufoi.TopicId == fieldOfInterestId)
                                .FirstOrDefaultAsync();

            if(fieldOfInterest == null){
                return false;
            }else{
                return true;
            }
        }

        public async Task<List<AppUser>> GetUsersForFieldOfInterestAsync(int fieldOfInterestId)
        {
            var users = await context.Set<UserFieldOfInterest>()
                                .Where(foi => foi.TopicId == fieldOfInterestId)
                                .Select(ufi => ufi.User)
                                //.Include(u => u.UserFieldOfInterests)
                                .ToListAsync();
            
            List<AppUser> mentors = new List<AppUser>();
            foreach (var user in users)
            {
                if(_userManager.IsInRoleAsync(user, "Mentor").Result){
                    var menteesRelationships = await _followRepo.GetAllMenteeRelationshipAsync(user.Id);
                    var uniqueMentees = menteesRelationships.GroupBy(m => m.UserId)
                                                            .Select(i => i.FirstOrDefault())
                                                            .ToList();
                    user.Followers = uniqueMentees ;
                    user.UserFieldOfInterests = await GetFieldOfInterestsForUserAsync(user.Id);
                    mentors.Add(user);
                }
            }
            return mentors;
        }
    }
}

