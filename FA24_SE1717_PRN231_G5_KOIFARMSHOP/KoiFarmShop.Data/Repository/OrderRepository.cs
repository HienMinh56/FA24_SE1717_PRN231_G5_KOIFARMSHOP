using KoiFarmShop.Data.Base;
using KoiFarmShop.Data.Models;
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

        public async Task<Order> CreateOrderAsync(string userId, List<(string koiId, int quantity)> orderDetails, string voucherId)
        {
            try
            {
                // Tạo OrderId và PaymentId
                string orderId = $"ORDER{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
                string paymentId = $"PAY{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

                // Khởi tạo đối tượng Order
                var order = new Order
                {
                    OrderId = orderId,
                    UserId = userId,
                    PaymentId = paymentId, // Liên kết với Payment
                    Status = 1, // Trạng thái ban đầu của Order
                    CreatedDate = DateTime.Now,
                    CreatedBy = userId,
                    VoucherId = voucherId,
                };

                double totalAmount = 0;
                int totalQuantity = 0;

                // Duyệt qua từng Koi được order
                foreach (var item in orderDetails)
                {
                    // Lấy thông tin sản phẩm (Koi) từ database dựa trên koiId
                    var koi = await _context.KoiFishes
                        .Where(k => k.DeletedBy == null && k.KoiId == item.koiId)
                        .AsNoTracking()
                        .SingleOrDefaultAsync();

                    if (koi == null)
                    {
                        throw new Exception($"Koi with ID {item.koiId} not found.");
                    }

                    // Tạo đối tượng OrderDetail
                    var orderDetail = new OrderDetail
                    {
                        KoiId = item.koiId, // Liên kết Koi với OrderDetail
                        Quantity = item.quantity,
                        Price = koi.Price * item.quantity,
                        OrderId = orderId,
                    };

                    totalAmount += orderDetail.Price;
                    totalQuantity += item.quantity;
                    order.OrderDetails.Add(orderDetail); // Thêm OrderDetail vào Order
                }

                // Cập nhật tổng số tiền và tổng số lượng vào Order
                order.TotalAmount = totalAmount;
                order.Quantity = totalQuantity;

                // Tạo đối tượng Payment và liên kết với Order
                var payment = new Payment
                {
                    PaymentId = paymentId,
                    UserId = userId,
                    Amount = totalAmount,
                    Type = 1, // Loại thanh toán
                    Status = 0, // Trạng thái chưa thanh toán
                    CreatedDate = DateTime.Now,
                };

                // Lưu thông tin Order và Payment vào database
                await _context.Orders.AddAsync(order);
                await _context.Payments.AddAsync(payment);

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
