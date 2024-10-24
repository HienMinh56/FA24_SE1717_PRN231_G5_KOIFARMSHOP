using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Request
{
     public class CreatePaymentRequest
     {
         public int Type { get; set; } // 1: Order, 2: Consignment

         public string? OrderId { get; set; }

         public string? ConsignmentId { get; set; }

         public int Status { get; set; }

         public string Currency { get; set; }

         public string PaymentMethod { get; set; }

         public int? Refundable { get; set; }

         public string Note { get; set; }
         public DateTime CreatedDate { get; set; }

    }
}
