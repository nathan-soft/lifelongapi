﻿using LifeLongApi.Models;
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
                                .OrderBy(f => f.Topic.Name)
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

    }
}

