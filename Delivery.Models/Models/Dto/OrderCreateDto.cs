﻿using System.ComponentModel.DataAnnotations;

namespace Delivery_Models.Models.Dto
{
    public class OrderCreateDto
    {
        [Required]
        public DateTime DeliveryTime { get; set; }

        [Required]
        [MinLength(1)]
        public string Address { get; set; }
    }
}
