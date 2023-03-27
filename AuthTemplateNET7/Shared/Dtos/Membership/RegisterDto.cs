using AuthTemplateNET7.Shared.SharedServices;
using System.ComponentModel.DataAnnotations;

namespace AuthTemplateNET7.Shared.Dtos.Membership;
public class RegisterDto : IValidatableObject
{
    [Required(ErrorMessage = "Please fill out the display name field"), MaxLength(32, ErrorMessage = "Your display name may not be longer than 32 characters")]
    public string DisplayName { get; set; }

    [Required(ErrorMessage = "Please enter your email address"),
        MaxLength(128, ErrorMessage = "Your email address may not be longer than 128 characters, and if it is, I feel bad for you every time you need to fill out a paper form as well as for anyone you share your email address with."),
        EmailAddress(ErrorMessage = "This does not appear to be a vaild email address")]
    public string EmailAddress { get; set; }

    [FormFactoryIgnore]
    [Required(ErrorMessage = "Please fill out the password field"), StringLength(128, MinimumLength = 16), DataType(DataType.Password)]
    public string Password { get; set; }

    [FormFactoryIgnore]
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
