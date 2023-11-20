using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Delivery_Models.Models.Entity
{
    public class OrderBasket 
    {
        [Key]
		public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public Guid DishId { get; set; }

        [ForeignKey("DishId")]

        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int TotalPrice { get; set; }

        [Required]
        public int Amount { get; set; }

        public string? Image { get; set; }
    }
}
