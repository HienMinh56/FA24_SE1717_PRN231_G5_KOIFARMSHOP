using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Request
{
    public class CreateVoucherRequest
    {
        public string VoucherId { get; set; }

        public string VoucherCode { get; set; }

        public double DiscountAmount { get; set; }

        public double MinOrderAmount { get; set; }

        public int Status { get; set; }

        public DateTime? ValidityStartDate { get; set; }

        public DateTime? ValidityEndDate { get; set; }
    }
}
