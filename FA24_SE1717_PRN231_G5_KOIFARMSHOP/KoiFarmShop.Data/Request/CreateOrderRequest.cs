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
        public string? OrderId { get; set; }

        public string UserId { get; set; }

        public int Status { get; set; }

        public string? VoucherCode { get; set; }

        public string ShippingAddress { get; set; }

        public string PaymentMethod { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public DateTime? ReceiveDate { get; set; }
        public string? Phone { get; set; }

        public string? Note { get; set; }

        public decimal? TotalWeight { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
