using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Delivery_API.Models.Entity
{
    public class Basket
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public Guid DishId { get; set; }

        [ForeignKey("DishId")]
        public Dish Dish { get; set; }

        [Required]
        public int Amount { get; set; }
    }
}
