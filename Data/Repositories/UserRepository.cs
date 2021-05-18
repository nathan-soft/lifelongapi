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
        Task<IEnumerable<AppUser>> GetAllAsync();
        Task<IEnumerable<AppUser>> GetAdministrativeUsersAsync();
        //Task<IEnumerable<AppUser>> GetMenteesAsync();
        //Task<IEnumerable<AppUser>> GetMentorsAsync();
        Task<IEnumerable<AppUser>> GetNonAdministrativeUsersAsync(string filterString);
        Task<IEnumerable<AppUser>> GetMentorsByFieldOfInterestAsync(int fieldOfInterestId);
    }

    public class UserRepository : IUserRepository
    {
        protected readonly IdentityAppContext _context;
        public UserRepository(IdentityAppContext context) {
            _context = context;
        }

        public async Task<IEnumerable<AppUser>> GetAllAsync()
        {
            return await _context.Set<AppUser>()
                                .Where(u => u.UserRoles.Any(r => r.Role.Name != "Admin"))
                                .ToListAsync();
        }

        public async Task<IEnumerable<AppUser>> GetAdministrativeUsersAsync()
        {
            return await _context.Set<AppUser>()
                                .Where(u => u.UserRoles.Any(r => r.Role.Name == "Moderator") && u.IsActive)
                                .ToListAsync();
        }

        public async Task<IEnumerable<AppUser>> GetNonAdministrativeUsersAsync(string filterString)
        {
            var query = _context.Set<AppUser>()
                                .Where(u => u.UserRoles.Any(r => r.Role.Name == "Mentor" || r.Role.Name == "Mentee") && u.IsActive);

            if (!string.IsNullOrWhiteSpace(filterString))
            {
                query = query.Where(u => u.Email.Contains(filterString) 
                                        || u.FirstName.Contains(filterString) 
                                        || u.LastName.Contains(filterString));
            }

            return await query.ToListAsync();
        }

        //public async Task<IEnumerable<AppUser>> GetMentorsAsync()
        //{
        //    return await _context.Set<AppUser>()
        //                        .Where(u => u.UserRoles.Any(r => r.Role.Name == "Mentor"))
        //                        .ToListAsync();
        //}

        //public async Task<IEnumerable<AppUser>> GetMenteesAsync()
        //{
        //    return await _context.Set<AppUser>()
        //                        .Where(u => u.UserRoles.Any(r => r.Role.Name == "Mentee"))
        //                        .ToListAsync();
        //}

        public async Task<IEnumerable<AppUser>> GetMentorsByFieldOfInterestAsync(int fieldOfInterestId)
        {
            return await _context.Set<AppUser>()
                                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Mentor" )
                                           && u.IsActive
                                           && u.UserFieldOfInterests.Any(ufi => ufi.TopicId == fieldOfInterestId))
                                .ToListAsync();
        }
    }
}
