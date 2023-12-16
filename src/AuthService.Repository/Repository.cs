using AuthService.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace AuthService.Repository
{
    public class Repository : IRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public Repository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public void SaveChanges() => _applicationDbContext.SaveChanges();

        public async Task SaveChangesAsync() => await _applicationDbContext.SaveChangesAsync();

        public IDbContextTransaction BeginTransaction() => _applicationDbContext.Database.BeginTransaction();

        public async Task<IDbContextTransaction> BeginTransactionAsync() => await _applicationDbContext.Database.BeginTransactionAsync();

        public async Task<UsersTable?> GetUserById(long iduser)
        {
            return await _applicationDbContext.Users.Where(x => x.IDUser == iduser).Include(u => u.UserRoles).ThenInclude(ur => ur.Role).SingleOrDefaultAsync();
        }

        public async Task<UsersTable?> GetUserByUsername(string username)
        {
            return await _applicationDbContext.Users.Where(x => x.Username == username).Include(u => u.UserRoles).ThenInclude(ur => ur.Role).SingleOrDefaultAsync();
        }

        public async Task<UsersTable?> GetUserByEmail(string email)
        {
            return await _applicationDbContext.Users.Where(x => x.Email == email).Include(u => u.UserRoles).ThenInclude(ur => ur.Role).SingleOrDefaultAsync();
        }

        public async Task AddUserAsync(UsersTable user)
        {
            await _applicationDbContext.Users.AddAsync(user);
        }

        public async Task<UsersTable?> UpdateUserAvatarAsync(long iduser, string avatar)
        {
            var user = await _applicationDbContext.Users.Where(x => x.IDUser == iduser).SingleOrDefaultAsync();
            if (user is null)
                return null;

            user.Avatar = avatar;
            _applicationDbContext.Users.Update(user);
            return user;
        }

        public async Task<List<RolesTable>> GetRoles()
        {
            return await _applicationDbContext.Roles.ToListAsync();
        }

        public async Task<RolesTable?> GetRoleByName(string name)
        {
            return await _applicationDbContext.Roles.Where(x=>x.Name==name).SingleOrDefaultAsync();
        }

        public async Task AddRoleAsync(RolesTable role)
        {
            await _applicationDbContext.Roles.AddAsync(role);
        }

        public void RemoveRole(RolesTable role)
        {
            _applicationDbContext.Roles.Remove(role);
        }

        public async Task RemoveRole(string name)
        {
            RolesTable? role = await GetRoleByName(name);
            if (role != null) _applicationDbContext.Roles.Remove(role);
        }

        public async Task<UserRolesTable?> GetUserRole(long iduser, long idrole)
        {
            return await _applicationDbContext.UserRoles.Where(x => x.IDUser==iduser && x.IDRole == idrole).SingleOrDefaultAsync();
        }

        public async Task SetUserRoleAsync(UserRolesTable userRole)
        {
            await _applicationDbContext.UserRoles.AddAsync(userRole);
        }

        public void RemoveUserRole(UserRolesTable userRole)
        {
            _applicationDbContext.UserRoles.Remove(userRole);
        }

        public async Task<bool> CheckUserRole(long iduser, long idrole)
        {
            return await _applicationDbContext.UserRoles.Where(x => x.IDUser == iduser && x.IDRole == idrole).SingleOrDefaultAsync() != null;
        }
    }
}
