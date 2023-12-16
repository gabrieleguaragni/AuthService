using AuthService.Business.Abstractions.Services;
using AuthService.Business.Exceptions;
using AuthService.Repository;
using AuthService.Repository.Models;
using AuthService.Shared.DTO.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Business.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository _repository;

        public TokenService(IConfiguration configuration, IRepository repository)
        {
            _configuration = configuration;
            _repository = repository;
        }

        public string GenerateToken(long iduser, string username, string email)
        {
            var signingkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"]!));
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]!));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                IssuedAt = DateTime.UtcNow,
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"],
                Expires = DateTime.UtcNow.AddYears(1),
                SigningCredentials = new SigningCredentials(signingkey, SecurityAlgorithms.HmacSha512),
                EncryptingCredentials = new EncryptingCredentials(secretKey, SecurityAlgorithms.Aes256KW, SecurityAlgorithms.Aes256CbcHmacSha512),
                Subject = new ClaimsIdentity(new List<Claim>
                {
                   new Claim("IDUser", iduser.ToString()),
                   new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                })
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }


        public async Task<ValidateTokenResponse> ValidateToken(IEnumerable<Claim> claims)
        {
            string? IDUser = claims.FirstOrDefault(c => c.Type == "IDUser")?.Value;
            if (string.IsNullOrEmpty(IDUser))
            {
                throw new HttpStatusException(401, "Invalid token");
            }

            UsersTable? user = await _repository.GetUserById(long.Parse(IDUser));
            if (user == null)
            {
                throw new HttpStatusException(401, "Invalid token");
            }

            return new ValidateTokenResponse()
            {
                IDUser = user.IDUser,
                Username = user.Username,
                Email = user.Email,
                Avatar = user.Avatar,
                Roles = user.UserRoles.Select(ur => ur.Role?.Name).ToList() ?? new List<string?>()
            };
        }
    }
}
