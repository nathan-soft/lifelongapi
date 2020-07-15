using LifeLongApi.Codes;
using LifeLongApi.Dtos;
using LifeLongApi.Dtos.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeLongApi.Services
{
    public interface IWorkExperienceService
    {
        Task<ServiceResponse<WorkExperienceResponseDto>> AddAsync(WorkExperienceDto workExperience);
        Task<ServiceResponse<WorkExperienceResponseDto>> DeleteWorkExperienceAsync(int workExperienceId);
        Task<ServiceResponse<WorkExperienceResponseDto>> GetByIdAsync(int id);
        Task<ServiceResponse<List<WorkExperienceResponseDto>>> GetWorkExperiencesAsync();
        Task<ServiceResponse<WorkExperienceResponseDto>> UpdateAsync(int workExperienceId, WorkExperienceDto workExperience);
        Task<ServiceResponse<List<WorkExperienceResponseDto>>> GetUserWorkExperiencesAsync(int userId);
    }
}