using Delivery_Models.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace Delivery_Models.Models.Dto
{
    public class OrderInfoDto
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
    }
}
