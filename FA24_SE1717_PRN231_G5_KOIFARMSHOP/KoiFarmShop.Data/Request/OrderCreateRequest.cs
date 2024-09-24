using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Request
{
    public class OrderCreateRequest
    {
        public List<OrderItem> OrderItems { get; set; }
        public string? VoucherId { get; set; }
    }
}
