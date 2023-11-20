using Delivery_Models.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace Delivery_Models.Models.Dto
{
    public class OrderDto
    {
        public Guid Id { get; set; }

        [Required]
        public DateTime DeliveryTime { get; set; }

        [Required]
        public DateTime OrderTime { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        [Required]
        public double Price { get; set; }
        public List<DishBasketDto> Dishes { get; set; }

        [Required]
        [MinLength(1)]
        public string Address { get; set; }
    }
}
