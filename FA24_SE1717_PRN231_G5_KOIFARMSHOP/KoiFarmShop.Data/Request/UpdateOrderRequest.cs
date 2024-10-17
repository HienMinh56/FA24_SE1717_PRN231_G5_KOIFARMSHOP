using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Request
{
    public class UpdateOrderRequest
    {
        public string OrderId { get; set; }
        public int Status { get; set; }
        public string? VoucherId { get; set; }
        public string UserId { get; set; }
    }
}
