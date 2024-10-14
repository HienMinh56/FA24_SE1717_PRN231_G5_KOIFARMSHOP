using KoiFarmShop.Data.Base;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Request;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Repository
{
    public class OrderRepository : GenericRepository<Order>
    {
        public OrderRepository()
        {
        }

        public OrderRepository(FA24_SE1717_PRN231_G5_KOIFARMSHOPContext context) => _context = context;

        public async Task<Order> CreateOrderAsync(string userId, List<OrderItem> orderDetails, string? voucherCode)
        {
            try
            {
                // Tạo OrderId và PaymentId
                string orderId = $"ORDER{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
                string paymentId = $"PAY{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

                var voucher = !string.IsNullOrEmpty(voucherCode)
             ? _context.Vouchers.Where(v => v.VoucherCode == voucherCode).FirstOrDefault()
             : null;

                // Khởi tạo đối tượng Order
                var order = new Order
                {
                    OrderId = orderId,
                    UserId = userId,
                    Status = 1, // Trạng thái ban đầu của Order
                    CreatedDate = DateTime.Now,
                    CreatedBy = userId,
                    VoucherId = voucher?.VoucherId, // VoucherId có thể là null nếu không có voucher
                };

                double totalAmount = 0;
                int totalQuantity = 0;

                // Duyệt qua từng Koi được order
                foreach (var item in orderDetails)
                {
                    // Lấy thông tin sản phẩm (Koi) từ database dựa trên koiId
                    var koi = await _context.KoiFishes
                        .Where(k => k.DeletedBy == null && k.KoiId == item.KoiId)
                        .AsNoTracking()
                        .SingleOrDefaultAsync();

                    if (koi == null)
                    {
                        throw new Exception($"Koi with ID {item.KoiId} not found.");
                    }

                    // Tạo đối tượng OrderDetail
                    var orderDetail = new OrderDetail
                    {
                        KoiId = item.KoiId, // Liên kết Koi với OrderDetail
                        Quantity = item.Quantity,
                        Price = koi.Price * item.Quantity,
                        OrderId = orderId,
                    };

                    totalAmount += orderDetail.Price;
                    totalQuantity += item.Quantity;
                    order.OrderDetails.Add(orderDetail); // Thêm OrderDetail vào Order
                }

                // Cập nhật tổng số tiền và tổng số lượng vào Order
                order.TotalAmount = totalAmount;
                order.Quantity = totalQuantity;

                // Tạo đối tượng Payment và liên kết với Order
                

                // Lưu thông tin Order và Payment vào database
                await _context.Orders.AddAsync(order);

                // Lưu thay đổi vào database
                await _context.SaveChangesAsync();

                // Trả về kết quả thành công
                return order;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                throw new Exception($"An error occurred while creating the order: {ex.Message}", ex);
            }
        }

    }

}
