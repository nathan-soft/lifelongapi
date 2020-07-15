using LifeLongApi.Codes;
using LifeLongApi.Dtos;
using LifeLongApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeLongApi.Services
{
    public interface ITopicService
    {
        public Task<ServiceResponse<List<TopicDto>>> GetAllFieldOfInterestAsync();
        public Task<ServiceResponse<TopicDto>> GetFieldOfInterestByIdAsync(int fieldOfInterestId);
        public Task<ServiceResponse<TopicDto>> GetFieldOfInterestByNameAsync(string fieldOfInterestName);
        public Task<ServiceResponse<TopicDto>> AddFieldOfInterestAsync(TopicDto fieldOfInterest);
        public Task<ServiceResponse<TopicDto>> UpdateFieldOfInterestAsync(TopicDto fieldOfInterest, string oldFieldOfInterestName );
        public Task<ServiceResponse<TopicDto>> DeleteFieldOfInterestAsync(int fieldOfInterestId);
    }
}