using LifeLongApi.Codes;
using LifeLongApi.Dtos;
using LifeLongApi.Dtos.Response;
using LifeLongApi.Models;
using System.Threading.Tasks;

namespace LifeLongApi.Services {
    public interface IAuthService {
        public Task<ServiceResponse<TokenDto>> Login (string username, string password);
    }
}