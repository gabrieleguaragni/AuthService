using FluentValidation;

namespace AuthService.Validators
{
    public class ImageValidator : AbstractValidator<IFormFile>
    {
        readonly string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

        public ImageValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .NotEmpty().WithMessage("The image cannot be empty")
                .Must(HaveValidFileExtension)
                .WithMessage("Invalid image format! Formats allowed: " + string.Join(", ", allowedExtensions));

            RuleFor(x => x.Length)
               .LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("Image size must not exceed 10 MB");
        }

        private bool HaveValidFileExtension(IFormFile? file)
        {
            if (file == null) return false;
            string fileExtension = Path.GetExtension(file.FileName);
            return allowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }
    }
}
