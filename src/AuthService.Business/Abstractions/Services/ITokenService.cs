using AuthService.Shared.DTO.Response;
using System.Security.Claims;

namespace AuthService.Business.Abstractions.Services
{
    public interface ITokenService
    {
        public string GenerateToken(long iduser, string username, string email);
        public Task<ValidateTokenResponse> ValidateToken(IEnumerable<Claim> claims);
    }
}
