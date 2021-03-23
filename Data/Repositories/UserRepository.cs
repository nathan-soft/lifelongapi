using LifeLongApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LifeLongApi.Data.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetAdministrativeUsersAsync();
        Task<IEnumerable<AppUser>> GetMenteesAsync();
        Task<IEnumerable<AppUser>> GetMentorsAsync();
        Task<IEnumerable<AppUser>> GetNonAdministrativeUsersAsync();
        Task<IEnumerable<AppUser>> GetUsersByFieldOfInterestAsync(int fieldOfInterestId);
    }

    public class UserRepository : IUserRepository
    {
        protected readonly IdentityAppContext _context;
        public UserRepository(IdentityAppContext context) {
            _context = context;
        }

        public async Task<IEnumerable<AppUser>> GetAdministrativeUsersAsync()
        {
            return await _context.Set<AppUser>()
                                .Where(u => u.UserRoles.Any(r => r.Role.Name == "Moderator"))
                                .ToListAsync();
        }

        public async Task<IEnumerable<AppUser>> GetNonAdministrativeUsersAsync()
        {
            return await _context.Set<AppUser>()
                                .Where(u => u.UserRoles.Any(r => r.Role.Name != "Moderator" && r.Role.Name != "Admin"))
                                .ToListAsync();
        }

        public async Task<IEnumerable<AppUser>> GetMentorsAsync()
        {
            return await _context.Set<AppUser>()
                                .Where(u => u.UserRoles.Any(r => r.Role.Name == "Mentor"))
                                .ToListAsync();
        }

        public async Task<IEnumerable<AppUser>> GetMenteesAsync()
        {
            return await _context.Set<AppUser>()
                                .Where(u => u.UserRoles.Any(r => r.Role.Name == "Mentee"))
                                .ToListAsync();
        }

        public async Task<IEnumerable<AppUser>> GetUsersByFieldOfInterestAsync(int fieldOfInterestId)
        {
            var users = await _context.Set<UserFieldOfInterest>()
                                .Where(foi => foi.TopicId == fieldOfInterestId)
                                .Select(ufi => ufi.User)
                                //.Include(u => u.UserFieldOfInterests)
                                .ToListAsync();

            //var users2 = await context.Set<AppUser>()
            //                          .Where(u => u.UserFieldOfInterests.Find(ui => ui.TopicId == fieldOfInterestId) != null)
            //                          .ToListAsync();
            return users;
        }
    }
}
