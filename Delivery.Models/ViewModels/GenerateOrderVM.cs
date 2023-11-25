using Delivery_Models.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delivery_Models.ViewModels
{
    public class GenerateOrderVM
    {
        public UserDto UserDto { get; set; }

        public IEnumerable<DishBasketDto> DishBasketDtos { get; set; }

		//public OrderCreateDto OrderCreateDto { get; set; }
	}
}
