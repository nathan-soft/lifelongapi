using AutoMapper;
using LifeLongApi.Codes;
using LifeLongApi.Data.Repositories;
using LifeLongApi.Dtos;
using LifeLongApi.Dtos.Response;
using LifeLongApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LifeLongApi.Services
{

    public class WorkExperienceService : IWorkExperienceService
    {
        private readonly IWorkExperienceRepository _workExperienceRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITopicRepository _topicRepo;
        private readonly IMapper _mapper;
        public WorkExperienceService(IWorkExperienceRepository workExperienceRepo, UserManager<AppUser> userManager, IMapper mapper, ITopicRepository topicRepo)
        {
            _workExperienceRepo = workExperienceRepo;
            _mapper = mapper;
            _topicRepo = topicRepo;
            _userManager = userManager;
        }

        public async Task<ServiceResponse<List<WorkExperienceResponseDto>>> GetWorkExperiencesAsync()
        {
            var workExperiences = await _workExperienceRepo.GetAllAsync();
            workExperiences = workExperiences.ToList();

            var sr = new ServiceResponse<List<WorkExperienceResponseDto>>();
            sr.Code = StatusCodes.Status200OK;
            sr.Data = _mapper.Map<List<WorkExperienceResponseDto>>(workExperiences);
            sr.Success = true;
            return sr;
        }

        public async Task<ServiceResponse<List<WorkExperienceResponseDto>>> GetUserWorkExperiencesAsync(int userId)
        {
            var userWorkExperiences = await _workExperienceRepo.GetAllForUserAsync(userId);

            var sr = new ServiceResponse<List<WorkExperienceResponseDto>>();
            sr.Code = StatusCodes.Status200OK;
            sr.Data = _mapper.Map<List<WorkExperienceResponseDto>>(userWorkExperiences);
            sr.Success = true;
            return sr;
        }

        public async Task<ServiceResponse<WorkExperienceResponseDto>> GetByIdAsync(int id)
        {
            var workExperience = await _workExperienceRepo.GetByIdAsync(id);
            var sr = new ServiceResponse<WorkExperienceResponseDto>();
            if (workExperience != null)
            {
                sr.Code = StatusCodes.Status200OK;
                sr.Data = _mapper.Map<WorkExperienceResponseDto>(workExperience);
                sr.Success = true;
            }
            else
            {
                sr.HelperMethod(StatusCodes.Status404NotFound, "Work experience does not exist.", false);
            }
            return sr;
        }

        public async Task<ServiceResponse<WorkExperienceResponseDto>> AddAsync(WorkExperienceDto workExperience)
        {
            var sr = new ServiceResponse<WorkExperienceResponseDto>();

            //get user
            var foundUser = await _userManager.FindByNameAsync(workExperience.Username);
            if (foundUser == null)
            {
                //user does not exist.
                sr.HelperMethod(404, "user not found.", false);
                return sr;
            }

            //make sure there's a field of interest with supplied name
            var fieldOfInterest = await _topicRepo.GetByNameAsync(workExperience.FieldOfInterest);
            if (fieldOfInterest == null)
            {
                //user does not exist.
                sr.HelperMethod(404, "The field of interest does not exist.", false);
                return sr;
            }

            //convert/map
            var newWorkExperience = _mapper.Map<WorkExperience>(workExperience);
            //replace username with id
            newWorkExperience.UserId = foundUser.Id;
            //and fieldOfInterest name with id
            newWorkExperience.TopicId = fieldOfInterest.Id;

            //Check for duplicate
            var userExperience = _workExperienceRepo
                                    .GetSpecific(
                                        foundUser.Id, 
                                        fieldOfInterest.Id, 
                                        newWorkExperience.CompanyName,
                                        newWorkExperience.StartYear
                                    );

            if (userExperience != null)
            {
                //duplicate record found.
                sr.HelperMethod(404, "Work experience for user alredy exist.", false);
                return sr;
            }

            //insert new work experience
            await _workExperienceRepo.InsertAsync(newWorkExperience);

            //get recently inserted record
            userExperience = _workExperienceRepo.GetSpecific(foundUser.Id, fieldOfInterest.Id, newWorkExperience.CompanyName, newWorkExperience.StartYear);

            sr.Code = 201;
            sr.Success = true;
            sr.Data = _mapper.Map<WorkExperienceResponseDto>(userExperience);
            return sr;
        }

        public async Task<ServiceResponse<WorkExperienceResponseDto>> UpdateAsync(int workExperienceId, WorkExperienceDto workExperience)
        {
            var sr = new ServiceResponse<WorkExperienceResponseDto>();
            //get user work experience.
            var userWorkExperience = await _workExperienceRepo.GetByIdAsync(workExperienceId);
            if (userWorkExperience == null)
            {
                //work experience does not exist.
                sr.HelperMethod(StatusCodes.Status404NotFound, "No work experience record found.", false);
                return sr;
            }

            //get user
            var foundUser = await _userManager.FindByNameAsync(workExperience.Username);
            if (foundUser == null)
            {
                //user does not exist.
                sr.HelperMethod(StatusCodes.Status404NotFound, "user not found.", false);
                return sr;
            }

            //make sure there's a field of interest with supplied name
            var fieldOfInterest = await _topicRepo.GetByNameAsync(workExperience.FieldOfInterest);
            if (fieldOfInterest == null)
            {
                //user does not exist.
                sr.HelperMethod(StatusCodes.Status404NotFound, "The field of interest does not exist.", false);
                return sr;
            }
            //convert/map
            userWorkExperience = _mapper.Map<WorkExperience>(workExperience);
            //replace username with id
            userWorkExperience.UserId = foundUser.Id;
            //and fieldOfInterest name with id
            userWorkExperience.TopicId = fieldOfInterest.Id;
            await _workExperienceRepo.UpdateAsync(userWorkExperience);

            sr.Data = _mapper.Map<WorkExperienceResponseDto>(userWorkExperience);
            return sr;
        }
        public async Task<ServiceResponse<WorkExperienceResponseDto>> DeleteWorkExperienceAsync(int workExperienceId)
        {
            var sr = new ServiceResponse<WorkExperienceResponseDto>();
            var workExperience = await _workExperienceRepo.GetByIdAsync(workExperienceId);
            await _workExperienceRepo.DeleteAsync(workExperience);

            sr.HelperMethod(StatusCodes.Status200OK, "Delete successful.", true);
            return sr;
        }
    }
}