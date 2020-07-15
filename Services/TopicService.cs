using AutoMapper;
using LifeLongApi.Codes;
using LifeLongApi.Data.Repositories;
using LifeLongApi.Dtos;
using LifeLongApi.Dtos.Response;
using LifeLongApi.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LifeLongApi.Services
{
    public class TopicService : ITopicService
    {
        private readonly ITopicRepository _fieldOfInterestRepo;
        private readonly IMapper _mapper;
        public TopicService(ITopicRepository fieldOfInterestRepo, IMapper mapper)
        {
            _fieldOfInterestRepo = fieldOfInterestRepo;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<List<TopicDto>>> GetAllFieldOfInterestAsync()
        {
            var topics = await _fieldOfInterestRepo.GetAllAsync();
            topics = topics.ToList();

            var sr = new ServiceResponse<List<TopicDto>>();
            sr.Code = StatusCodes.Status200OK;
            sr.Data = _mapper.Map<List<TopicDto>>(topics);
            sr.Success = true;
            return sr;
        }
        public async Task<ServiceResponse<TopicDto>> GetFieldOfInterestByIdAsync(int id)
        {
            var topic = await _fieldOfInterestRepo.GetByIdAsync(id);
            var sr = new ServiceResponse<TopicDto>();
            if (topic != null)
            {
                sr.Code = StatusCodes.Status200OK;
                sr.Data = _mapper.Map<TopicDto>(topic);
                sr.Success = true;
            }
            else
            {
                sr.Code = StatusCodes.Status404NotFound;
                sr.Message = "Field of interest not found.";
                sr.Success = false;
            }
            return sr;
        }
        public async Task<ServiceResponse<TopicDto>> GetFieldOfInterestByNameAsync(string fieldOfInterestName)
        {
            var category = await _fieldOfInterestRepo.GetByNameAsync(fieldOfInterestName);
            var sr = new ServiceResponse<TopicDto>();
            if (category != null)
            {
                sr.Code = StatusCodes.Status200OK;
                sr.Data = _mapper.Map<TopicDto>(category);
                sr.Success = true;
            }
            else
            {
                //Error: category name does not exist in db.
                sr.Code = StatusCodes.Status404NotFound;
                sr.Message = "field of interest does not exist.";
                sr.Success = false;
            }
            return sr;
        }
        public async Task<ServiceResponse<TopicDto>> AddFieldOfInterestAsync(TopicDto fieldOfInterest)
        {
            var sr = new ServiceResponse<TopicDto>();
            //get field Of Interest.
            var foundEntity = await GetFieldOfInterestByNameAsync(fieldOfInterest.Name);
            if(foundEntity.Data != null){
                //duplicate name
                sr.HelperMethod(409, "A record with same name already exists", false);
                return sr;
            }

            //insert new Field of interest.
            await _fieldOfInterestRepo.InsertAsync(_mapper.Map<Topic>(fieldOfInterest));
            sr.Code = 201;
            sr.Data = fieldOfInterest;
            sr.Success = true;
            return sr;
        }

        public async Task<ServiceResponse<TopicDto>> UpdateFieldOfInterestAsync(TopicDto fieldOfInterest, string oldfieldOfInterestName)
        {
            //get field Of Interest.
            var fofi = await GetFieldOfInterestByNameAsync(oldfieldOfInterestName);
            //var category = await _categoryRepo.GetByNameAsync(categoryName);
            var sr = new ServiceResponse<TopicDto>();
            if (fofi.Data == null)
            {
                //no record found
                sr.HelperMethod(404, "No record found", false);
            }else{
                //convert and save the changes made;
                await _fieldOfInterestRepo.UpdateAsync(_mapper.Map<Topic>(fieldOfInterest));
                sr.Code = 201;
                sr.Data = fieldOfInterest;
                sr.Success = true;
            }
            return sr;
        }
        public async Task<ServiceResponse<TopicDto>> DeleteFieldOfInterestAsync(int fieldOfInterestId)
        {
            var sr = new ServiceResponse<TopicDto>();
            var fieldOfInterest = await _fieldOfInterestRepo.GetByIdAsync(fieldOfInterestId);
            if(fieldOfInterest != null){
                await _fieldOfInterestRepo.DeleteAsync(fieldOfInterest);
                sr.Code = 204;
                sr.Success = true;
                return sr;
            }else{
                sr.HelperMethod(404, "No field of interest found", false);
                return sr;
            }
                
        }
    
    }
}