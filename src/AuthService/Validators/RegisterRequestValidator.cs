using AuthService.Shared.DTO.Request;
using FluentValidation;

namespace AuthService.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username cannot be empty")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long")
            .MaximumLength(25).WithMessage("Username cannot exceed 25 characters")
            .Matches("^[a-zA-Z0-9-_]+$").WithMessage("Username can only contain letters, numbers, '-' and '_'");

            RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email cannot be empty")
            .MaximumLength(320).WithMessage("Email cannot exceed 320 characters")
            .EmailAddress().WithMessage("Invalid email address");

            RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password cannot be empty")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .MaximumLength(128).WithMessage("Password cannot exceed 128 characters")
            .Matches("(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^a-zA-Z0-9]).{8,}")
            .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character");
        }
    }
}
