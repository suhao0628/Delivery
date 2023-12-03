using Delivery_Models.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Delivery_Models.Models.Entity
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MinLength(1)]
        public string FullName { get; set; }

        public DateTime? BirthDate { get; set; }

        [Required]
        public Gender Gender { get; set; }

        public string? Address { get; set; }

        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        [PasswordValidation]
        public string Password { get; set; }

        [Phone]
        [RegularExpression(@"^\+7 \(\d{3}\) \d{3}-\d{2}-\d{2}-\d{2}$", ErrorMessage = "Please enter a valid phone number in the format +7 (xxx) xxx-xx-xx-xx.")]
        public string? PhoneNumber { get; set; }
    }
}
