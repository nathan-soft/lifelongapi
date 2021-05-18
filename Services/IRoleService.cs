using LifeLongApi.Codes;
using LifeLongApi.Dtos;
using LifeLongApi.Dtos.Response;
using LifeLongApi.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LifeLongApi.Services
{
    public interface IRoleService
    {
        public Task<ServiceResponse<RoleDto>> NewRoleAsync(string roleName);

        public Task<ServiceResponse<List<RoleDto>>> GetAdminRolesAsync();

        public Task<ServiceResponse<RoleDto>> GetRoleByNameAsync(string roleName);

        public Task<ServiceResponse<RoleDto>> GetRoleByIdAsync(int roleId);

        public Task<ServiceResponse<RoleDto>> UpdateRoleNameAsync(string oldRoleName, string newRoleName);

        public Task<ServiceResponse<string>> DeleteRoleAsync(string roleName);
    }
}