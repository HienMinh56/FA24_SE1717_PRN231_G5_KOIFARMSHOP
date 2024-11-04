using KoiFarmShop.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Response
{
    public class GetOrderResponse
    {
        public string OrderId { get; set; }
        public string UserId { get; set; }
        public int Status { get; set; }
        public double TotalAmount { get; set; }
        public int Quantity { get; set; }
        public string VoucherId { get; set; }
        public string ShippingAddress { get; set; }
        public string Phone { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public string? Note { get; set; }
        public decimal? TotalWeight { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; }
        public VoucherDto Voucher { get; set; }
        public UserDto User { get; set; }
    }
    public class OrderDetailDto
    {
        public string KoiId { get; set; }
        public string KoiName { get; set; }
        public int Quantity { get; set; }
    }
    public class UserDto
    {
        public string UserId { get; set; }
        public string UserName{ get; set; }
    }
    public class VoucherDto
    {
        public string VoucherId { get; set; }
        public string VoucherCode{ get; set; }
    }
}
