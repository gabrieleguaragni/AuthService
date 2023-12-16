using AuthService.Business.Abstractions.Services;
using AuthService.Shared.DTO.Request;
using AuthService.Shared.DTO.Response;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ITokenService _tokenService;
        private IValidator<SetUserRoleRequest> _setUserRoleRequestValidator;
        private IValidator<RemoveUserRoleRequest> _removeUserRoleRequestValidator;

        public RoleController(
            IRoleService roleService, 
            ITokenService tokenService,
            IValidator<SetUserRoleRequest> setUserRoleRequestValidator,
            IValidator<RemoveUserRoleRequest> removeUserRoleRequestValidator
            )
        {
            _roleService = roleService;
            _tokenService = tokenService;
            _setUserRoleRequestValidator = setUserRoleRequestValidator;
            _removeUserRoleRequestValidator = removeUserRoleRequestValidator;
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetUserRoles()
        {
            ValidateTokenResponse tokenData = await _tokenService.ValidateToken(User.Claims);
            return Ok(new { roles = await _roleService.GetUserRoles(tokenData.IDUser) });
        }

        [Authorize]
        [HttpGet("user/{username}")]
        public async Task<IActionResult> GetUserRolesById([FromRoute] string username)
        {
            ValidateTokenResponse tokenData = await _tokenService.ValidateToken(User.Claims);
            return Ok(new { roles = await _roleService.GetUserRolesByUsername(username) });
        }

        [Authorize]
        [HttpPost("set")]
        public async Task<IActionResult> SetUserRole([FromBody] SetUserRoleRequest setUserRoleRequest)
        {
            var validationResult = await _setUserRoleRequestValidator.ValidateAsync(setUserRoleRequest);
            if (!validationResult.IsValid)
            {
                return StatusCode(400, new { message = validationResult.Errors.First().ErrorMessage });
            }

            ValidateTokenResponse tokenData = await _tokenService.ValidateToken(User.Claims);
            await _roleService.SetUserRole(tokenData.IDUser, setUserRoleRequest.UsernameUser, setUserRoleRequest.RoleName);
            return Ok(new { message = "Added successfully" });
        }

        [Authorize]
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveUserRole([FromBody] RemoveUserRoleRequest removeUserRoleRequest)
        {
            var validationResult = await _removeUserRoleRequestValidator.ValidateAsync(removeUserRoleRequest);
            if (!validationResult.IsValid)
            {
                return StatusCode(400, new { message = validationResult.Errors.First().ErrorMessage });
            }

            ValidateTokenResponse tokenData = await _tokenService.ValidateToken(User.Claims);
            await _roleService.RemoveUserRole(tokenData.IDUser, removeUserRoleRequest.UsernameUser, removeUserRoleRequest.RoleName);
            return Ok(new { message = "Removed successfully" });
        }

        [Authorize]
        [HttpGet("list")]
        public async Task<IActionResult> GetRoleList()
        {
            await _tokenService.ValidateToken(User.Claims);
            return Ok(new { roles = await _roleService.GetRoles() });
        }

        [Authorize]
        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole([FromBody] AddRoleRequest addRoleRequest)
        {
            ValidateTokenResponse tokenData = await _tokenService.ValidateToken(User.Claims);
            RoleResponse role = await _roleService.AddRole(tokenData.IDUser, addRoleRequest.Name, addRoleRequest.Description);
            return Ok(new { message = "Added successfully", role });
        }

        [Authorize]
        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveRole([FromBody] RemoveRoleRequest removeRoleRequest)
        {
            ValidateTokenResponse tokenData = await _tokenService.ValidateToken(User.Claims);
            await _roleService.RemoveRole(tokenData.IDUser, removeRoleRequest.Name);
            return Ok(new { message = "Removed successfully" });
        }
    }
}