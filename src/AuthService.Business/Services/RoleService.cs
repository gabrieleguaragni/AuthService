using AuthService.Business.Abstractions.Kafka;
using AuthService.Business.Abstractions.Services;
using AuthService.Business.Exceptions;
using AuthService.Repository;
using AuthService.Repository.Models;
using AuthService.Shared.DTO.Kafka;
using AuthService.Shared.DTO.Response;

namespace AuthService.Business.Services
{
    public class RoleService : IRoleService
    {
        private readonly string administrator = "administrator";

        private readonly IRepository _repository;
        private readonly IKafkaProducerService _kafkaProducerService;

        public RoleService(IRepository repository, IKafkaProducerService kafkaProducerService)
        {
            _repository = repository;
            _kafkaProducerService = kafkaProducerService;
        }
        
        public async Task<List<string?>> GetUserRoles(long iduser)
        {
            UsersTable? user = await _repository.GetUserById(iduser);
            if (user != null) return user.UserRoles.Select(ur => ur.Role?.Name).ToList() ?? new List<string?>();
            return new List<string?>();
        }

        public async Task<List<string?>> GetUserRolesByUsername(string username)
        {
            UsersTable? user = await _repository.GetUserByUsername(username);
            if (user == null)
            {
                throw new HttpStatusException(404, "Username not found");
            }

            return user.UserRoles.Select(ur => ur.Role?.Name).ToList() ?? new List<string?>();
        }

        public async Task SetUserRole(long idAdministratorUser, string usernameUser, string roleName)
        {
            // reserved only for administrator user
            List<string?> roles = await GetUserRoles(idAdministratorUser);
            if (!roles.Contains(administrator, StringComparer.OrdinalIgnoreCase))
            {
                throw new HttpStatusException(403, "Invalid permission");
            }

            // prevent set administrator role
            if (roleName.Equals(administrator, StringComparison.OrdinalIgnoreCase))
            {
                throw new HttpStatusException(400, "Unable to set administrator role");
            }

            // check if the user to set role is valid
            UsersTable? user = await _repository.GetUserByUsername(usernameUser);
            if (user == null)
            {
                throw new HttpStatusException(400, "Invalid user to set role");
            }

            // check if the role name is valid
            RolesTable? role = await _repository.GetRoleByName(roleName);
            if (role == null)
            {
                throw new HttpStatusException(400, "Invalid role name");
            }

            // check if user already has this role
            if (await _repository.CheckUserRole(user.IDUser, role.IDRole))
            {
                throw new HttpStatusException(400, "This user already has this role");
            }

            UserRolesTable userRole = new()
            {
                IDUser = user.IDUser,
                IDRole = role.IDRole
            };

            await _repository.SetUserRoleAsync(userRole);
            await _repository.SaveChangesAsync();

            await _kafkaProducerService.ProduceNotificationAsync(new SendNotificationKafka()
            {
                IDUser = user.IDUser,
                Message = "You have just been assigned the role of " + role.Name,
                Type = NotificationType.Critical
            });
        }

        public async Task RemoveUserRole(long idAdministratorUser, string usernameUser, string roleName)
        {
            // reserved only for administrator user
            List<string?> roles = await GetUserRoles(idAdministratorUser);
            if (!roles.Contains(administrator, StringComparer.OrdinalIgnoreCase))
            {
                throw new HttpStatusException(403, "Invalid permission");
            }

            // prevent removing the administrator role
            if (roleName.Equals(administrator, StringComparison.OrdinalIgnoreCase))
            {
                throw new HttpStatusException(400, "Unable to remove the administrator role");
            }

            // check if the user to remove role is valid
            UsersTable? user = await _repository.GetUserByUsername(usernameUser);
            if (user == null)
            {
                throw new HttpStatusException(400, "Invalid user to remove role");
            }

            // check if role name is valid
            RolesTable? role = await _repository.GetRoleByName(roleName);
            if (role == null)
            {
                throw new HttpStatusException(400, "Invalid role name");
            }

            UserRolesTable? userRole = await _repository.GetUserRole(user.IDUser, role.IDRole);
            if (userRole == null)
            {
                throw new HttpStatusException(400, "The user specified does not have this role");
            }
            
            _repository.RemoveUserRole(userRole);
            await _repository.SaveChangesAsync();

            await _kafkaProducerService.ProduceNotificationAsync(new SendNotificationKafka()
            {
                IDUser = user.IDUser,
                Message = "You have just been removed the role of " + role.Name,
                Type = NotificationType.Critical
            });
        }

        public async Task<List<RoleResponse>> GetRoles()
        {
            List<RoleResponse> roles = new();

            foreach (var item in await _repository.GetRoles())
            {
                roles.Add(new RoleResponse()
                {
                    IDRole = item.IDRole,
                    Name = item.Name,
                    Description = item.Description,
                });
            }

            return roles;
        }

        public async Task<RoleResponse> AddRole(long idAdministratorUser, string roleName, string description)
        { 
            // reserved only for administrator user
            List<string?> roles = await GetUserRoles(idAdministratorUser);
            if (!roles.Contains(administrator, StringComparer.OrdinalIgnoreCase))
            {
                throw new HttpStatusException(403, "Invalid permission");
            }

            RolesTable? role = await _repository.GetRoleByName(roleName);
            if (role != null)
            {
                throw new HttpStatusException(400, "This role already exists");
            }

            RolesTable newRole = new()
            {
                Name = roleName,
                Description = description
            };
            
            await _repository.AddRoleAsync(newRole);
            await _repository.SaveChangesAsync();

            return new()
            {
                IDRole = newRole.IDRole,
                Name = newRole.Name,
                Description= newRole.Description
            };
        }

        public async Task RemoveRole(long idAdministratorUser, string roleName)
        {
            // reserved only for the administrator user
            List<string?> roles = await GetUserRoles(idAdministratorUser);
            if (!roles.Contains(administrator, StringComparer.OrdinalIgnoreCase))
            {
                throw new HttpStatusException(403, "Invalid permission");
            }

            // prevent deleting the administrator role
            if (roleName.Equals(administrator, StringComparison.OrdinalIgnoreCase))
            {
                throw new HttpStatusException(400, "Unable to remove the administrator role");
            }

            RolesTable? role = await _repository.GetRoleByName(roleName);
            if (role == null)
            {
                throw new HttpStatusException(404, "This role does not exist");
            }

            _repository.RemoveRole(role);
            await _repository.SaveChangesAsync();
        }
    }
}
