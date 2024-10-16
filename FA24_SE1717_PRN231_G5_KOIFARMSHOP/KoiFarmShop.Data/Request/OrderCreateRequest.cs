using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Request
{
    public class OrderCreateRequest
    {
        public string UserId { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public string? VoucherCode { get; set; }
    }
}
