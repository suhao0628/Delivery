using Delivery_Models.Models.Dto;
using Delivery_Models.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delivery_Models.ViewModels
{
    public class DishFilterVM
    {
       public DishPagedListDto dishPagedListDto { get; set; }

        public DishCategory Categories { get; set; }
        public DishCategory[] categories { get; set; }

        public DishSorting sorting { get; set; }

        public bool vegetarian {  get; set; }

        public int page { get; set; }
    }
}
