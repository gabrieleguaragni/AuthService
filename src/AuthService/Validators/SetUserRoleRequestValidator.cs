using AuthService.Shared.DTO.Request;
using FluentValidation;

namespace AuthService.Validators
{
    public class SetUserRoleRequestValidator : AbstractValidator<SetUserRoleRequest>
    {
        public SetUserRoleRequestValidator()
        {
            RuleFor(x => x.UsernameUser)
                .NotNull()
                .NotEmpty().WithMessage("UsernameUser cannot be empty");

            RuleFor(x => x.RoleName)
                .NotNull()
                .NotEmpty().WithMessage("RoleName cannot be empty");
        }
    }
}