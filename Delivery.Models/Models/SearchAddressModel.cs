using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delivery_Models.Models.Enum;

namespace Delivery_Models.Models
{
    public class SearchAddressModel
    {
        public Int64 ObjectId { get; set; }
        public Guid ObjectGuid { get; set; }
        public string? Text { get; set; }
        public GarAddressLevel? ObjectLevel { get; set; }
        public string? ObjectLevelText { get; set; }
    }
}
