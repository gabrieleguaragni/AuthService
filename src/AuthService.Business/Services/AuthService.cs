using AuthService.Business.Abstractions.Kafka;
using AuthService.Business.Abstractions.Services;
using AuthService.Business.Exceptions;
using AuthService.Repository;
using AuthService.Repository.Models;
using AuthService.Shared.DTO.Kafka;
using AuthService.Shared.DTO.Response;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Security.Claims;

namespace AuthService.Business.Services
{
    using BCrypt.Net;

    public class AuthService : IAuthService
    {
        private readonly IRepository _repository;
        private readonly ITokenService _tokenService;
        private readonly IKafkaProducerService _kafkaProducerService;

        public AuthService(IRepository repository, ITokenService tokenService, IKafkaProducerService kafkaProducerService)
        {
            _repository = repository;
            _tokenService = tokenService;
            _kafkaProducerService = kafkaProducerService;
        }

        public async Task<LoginResponse> Login(string email, string password)
        {
            UsersTable? user = await _repository.GetUserByEmail(email);
            if (user == null)
            {
                throw new HttpStatusException(401, "Invalid email or password");
            }

            if (BCrypt.Verify(password+user.Date, user.Password) == false)
            {
                throw new HttpStatusException(401, "Invalid email or password");
            }

            string token = _tokenService.GenerateToken(user.IDUser, user.Username, user.Email);

            await _kafkaProducerService.ProduceNotificationAsync(new SendNotificationKafka()
            {
                IDUser = user.IDUser,
                Message = "Someone has logged in",
                Type = NotificationType.Critical
            });

            return new LoginResponse()
            {
                Username = user.Username,
                Email = user.Email,
                Avatar = user.Avatar,
                Token = token,
                Roles = user.UserRoles.Select(ur => ur.Role?.Name).ToList() ?? new List<string?>()
            };
        }

        public async Task<RefreshTokenResponse> RefreshToken(IEnumerable<Claim> claims)
        {
            ValidateTokenResponse validateTokenResponse = await _tokenService.ValidateToken(claims);
            string newToken = _tokenService.GenerateToken(validateTokenResponse.IDUser, validateTokenResponse.Username, validateTokenResponse.Email);
            return new RefreshTokenResponse()
            {
                IDUser = validateTokenResponse.IDUser,
                Username = validateTokenResponse.Username,
                Email = validateTokenResponse.Email,
                Avatar = validateTokenResponse.Avatar,
                Token = newToken,
                Roles = validateTokenResponse.Roles
            };
        }

        public async Task Register(string username, string email, string password)
        {
            UsersTable? usernameExists = await _repository.GetUserByUsername(username);
            if (usernameExists != null)
            {
                throw new HttpStatusException(409, "Username already taken");
            }

            UsersTable? emailExists = await _repository.GetUserByEmail(email);
            if (emailExists != null)
            {
                throw new HttpStatusException(409, "Email already taken");
            }

            DateTime created = DateTime.Now;
            UsersTable newUser = new()
            {
                Username = username,
                Email = email,
                Password = BCrypt.HashPassword(password+created),
                Avatar = "default.png",
                Date = created
            };

            await _repository.AddUserAsync(newUser);
            await _repository.SaveChangesAsync();

            await _kafkaProducerService.ProduceNotificationAsync(new SendNotificationKafka()
            {
                IDUser = newUser.IDUser,
                Message = "Thank you for the registration",
                Type = NotificationType.Info
            });
        }

        public async Task UploadAvatar(long iduser, IFormFile avatar)
        {
            string uniqueFileName = $"{iduser}_{DateTime.Now.Ticks}_{Guid.NewGuid()}";
            using var memoryStream = new MemoryStream();
            await avatar.CopyToAsync(memoryStream);
            await _kafkaProducerService.ProduceAvatarAsync(new SendAvatarKafka()
            {
                IDUser = iduser,
                File = Convert.ToBase64String(memoryStream.ToArray()),
                FileExtension = Path.GetExtension(avatar.FileName),
                FileName = uniqueFileName
            });
            await UpdateAvatar(iduser, $"{uniqueFileName}.png");
        }

        public async Task UpdateAvatar(long iduser, string fileName)
        {
            UsersTable? user = await _repository.UpdateUserAvatarAsync(iduser, fileName);
            if (user == null)
            {
                throw new HttpStatusException(500, "Error occurred");
            }
            await _repository.SaveChangesAsync();
        }
    }
}
