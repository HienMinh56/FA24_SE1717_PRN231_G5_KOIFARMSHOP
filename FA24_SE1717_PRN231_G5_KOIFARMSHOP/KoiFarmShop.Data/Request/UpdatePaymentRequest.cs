﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Request
{
    public class UpdatePaymentRequest
    {
        public string PaymentId { get; set; }
        public int Status { get; set; }
        public double Amount { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
