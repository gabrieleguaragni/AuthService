using AuthService.Shared.DTO.Response;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AuthService.Business.Abstractions.Services
{
    public interface IAuthService
    {
        public Task<LoginResponse> Login(string email, string password);
        public Task<RefreshTokenResponse> RefreshToken(IEnumerable<Claim> claims);
        public Task Register(string username, string email, string password);
        public Task UploadAvatar(long iduser, IFormFile avatar);
        public Task UpdateAvatar(long iduser, string fileName);
    }
}
