using KoiFarmShop.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Request
{
    public class OrderItem
    {
        public string KoiId { get; set; }
        public int Quantity { get; set; }
    }
}
