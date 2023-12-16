using AuthService.Shared.DTO.Request;
using FluentValidation;

namespace AuthService.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email cannot be empty")
            .EmailAddress().WithMessage("Invalid email address");

            RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password cannot be empty");
        }
    }
}
