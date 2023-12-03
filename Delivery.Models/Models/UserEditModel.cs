using Delivery_Models.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace Delivery_Models.Models
{
    public class UserEditModel
    {
        [Required]
        [MinLength(1)]
        public string FullName { get; set; }
        [BirthDateValidation(ErrorMessage = "Birth date can't be later than today")]
        public DateTime? BirthDate { get; set; }

        [Required]
        public Gender Gender { get; set; }

        public string? Address { get; set; }

        [Phone]
        [RegularExpression(@"^\+7 \(\d{3}\) \d{3}-\d{2}-\d{2}-\d{2}$", ErrorMessage = "Please enter a valid phone number in the format +7 (xxx) xxx-xx-xx-xx.")]
        public string? PhoneNumber { get; set; }
    }
    
}
