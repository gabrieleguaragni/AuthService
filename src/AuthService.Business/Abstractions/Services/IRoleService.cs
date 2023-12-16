using AuthService.Shared.DTO.Response;

namespace AuthService.Business.Abstractions.Services
{
    public interface IRoleService
    {
        public Task<List<string?>> GetUserRoles(long iduser);
        public Task<List<string?>> GetUserRolesByUsername(string username);
        public Task SetUserRole(long idAdministratorUser, string usernameUser, string roleName);
        public Task RemoveUserRole(long idAdministratorUser, string usernameUser, string roleName);
        public Task<List<RoleResponse>> GetRoles();
        public Task<RoleResponse> AddRole(long idAdministratorUser, string roleName, string description);
        public Task RemoveRole(long idAdministratorUser, string roleName);
    }
}
