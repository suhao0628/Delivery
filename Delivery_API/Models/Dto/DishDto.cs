using Delivery_API.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace Delivery_API.Models.Dto
{
    public class DishDto
    {
        public Guid Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public double Price { get; set; }

        public string? Image { get; set; }

        public bool Vegetarian { get; set; }

        public double Rating { get; set; }

        public DishCategory Category { get; set; }
    }
}
