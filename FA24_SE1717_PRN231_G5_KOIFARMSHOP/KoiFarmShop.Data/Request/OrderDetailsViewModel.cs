using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Request
{
    public class OrderDetailsViewModel
    {
        public string OrderId { get; set; }
        public string UserId { get; set; }
        public int Status { get; set; }
        public string VoucherId { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
