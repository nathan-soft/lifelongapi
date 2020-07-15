using AutoMapper;
using LifeLongApi.Data.Repositories;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LifeLongApi.Services
{
    public class FollowService : IFollowService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IMapper _mapper;
        public FollowService(ICategoryRepository categoryRepo, IMapper mapper)
        {
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }

        //public GetByIdAsync
    }
}