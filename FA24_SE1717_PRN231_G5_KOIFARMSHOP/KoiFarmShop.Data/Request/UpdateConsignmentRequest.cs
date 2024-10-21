using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Request
{
    public class UpdateConsignmentRequest
    {
        public string ConsignmentId { get; set; }

        public string UserId { get; set; }

        public string KoiId { get; set; }

        public int Type { get; set; }

        public double? DealPrice { get; set; }

        public string Method { get; set; }

        public int Status { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }
    }
}
