using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Request
{
    public class CreateConsignmentRequest
    {
        public string UserId { get; set; }

        public string KoiId { get; set; }

        public int Type { get; set; }

        public double? DealPrice { get; set; }

        public string Method { get; set; }

        public DateOnly? ConsignmentDate { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }
    }
}
