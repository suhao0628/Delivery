using Delivery_Models.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Delivery_Models.Models
{
    public class UserRegisterModel
    {
        [Required]
        [MinLength(1)]
        public string FullName { get; set; }

        [Required]
        [MinLength(6)]
        [PasswordValidation]
        public string Password { get; set; }

        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string Email { get; set; }

        public string? Address { get; set; }
        [BirthDateValidation(ErrorMessage = "Birth date can not be later than today")]
        public DateTime? BirthDate { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Phone]
        [RegularExpression(@"^\+7 \(\d{3}\) \d{3}-\d{2}-\d{2}-\d{2}$", ErrorMessage = "Please enter a valid phone number in the format +7 (xxx) xxx-xx-xx-xx.")]
        public string? PhoneNumber { get; set; }
    }
    public class PasswordValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;

            if (string.IsNullOrWhiteSpace(password))
            {
                return new ValidationResult("Password must contain at least one digit");
            }

            // Password should contain at least one digit, one letter, and one special character
            var hasDigit = new Regex(@"[0-9]").IsMatch(password);
            if (!hasDigit)
            {
                return new ValidationResult("Password must contain at least one digit");
            }
            var hasLetter = new Regex(@"[a-zA-Z]").IsMatch(password);
            if (!hasLetter)
            {
                return new ValidationResult("Password must contain at least one letter");
            }
            var hasSpecialChar = new Regex(@"\W|_").IsMatch(password);
            if (!hasSpecialChar)
            {
                return new ValidationResult("Password must contain at least one special char");
            }

            return ValidationResult.Success;
        }
    }

    public class BirthDateValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateTime date)
            {
                return date <= DateTime.Today;
            }

            return false;
        }
    }
}
