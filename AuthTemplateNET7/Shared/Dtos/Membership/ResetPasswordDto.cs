using AuthTemplateNET7.Shared.SharedServices;
using System.ComponentModel.DataAnnotations;

namespace AuthTemplateNET7.Shared.Dtos.Membership;

//added

public class ResetPasswordDto : IValidatableObject
{
    public Guid ResetToken { get; set; }

    [Required(ErrorMessage = "Please fill out the password field"), StringLength(128, MinimumLength = 16), DataType(DataType.Password)]
    public string Password { get; set; }

    [Compare("Password")]
    [Required(ErrorMessage = "Please confirm your password"), StringLength(128, MinimumLength = 16), DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        ValidationResult[] result = new ValidationResult[1];

        PasswordValidationService passwordValidationService = new();

        bool passwordIsValid = passwordValidationService.IsValid(Password);

        if (!passwordIsValid)
        {
            ValidationResult validationResult = new ValidationResult("Password must contain at least one uppercase letter, one lowercase letter, a number, and at least one any of these special characters: `~!@#$%^&*()_-[]{}|;:<>,./", new[] { "Password" });
            result[0] = validationResult;
        }

        return result;
    }
}
