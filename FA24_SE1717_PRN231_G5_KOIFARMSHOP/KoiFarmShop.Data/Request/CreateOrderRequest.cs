using KoiFarmShop.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Request
{
    public class CreateOrderRequest
    {
        public Order Order { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }
    }
}
