using AuthService.Repository.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace AuthService.Repository
{
    public interface IRepository
    {
        public void SaveChanges();
        public Task SaveChangesAsync();
        public IDbContextTransaction BeginTransaction();
        public Task<IDbContextTransaction> BeginTransactionAsync();
        public Task<UsersTable?> GetUserById(long iduser);
        public Task<UsersTable?> GetUserByUsername(string username);
        public Task<UsersTable?> GetUserByEmail(string email); 
        public Task AddUserAsync(UsersTable user);
        public Task<UsersTable?> UpdateUserAvatarAsync(long iduser, string avatar);
        public Task<List<RolesTable>> GetRoles();
        public Task<RolesTable?> GetRoleByName(string name);
        public Task AddRoleAsync(RolesTable role);
        public void RemoveRole(RolesTable role);
        public Task<UserRolesTable?> GetUserRole(long iduser, long idrole);
        public Task SetUserRoleAsync(UserRolesTable userRole);
        public void RemoveUserRole(UserRolesTable userRole);
        public Task<bool> CheckUserRole(long iduser, long idrole);
    }
}
