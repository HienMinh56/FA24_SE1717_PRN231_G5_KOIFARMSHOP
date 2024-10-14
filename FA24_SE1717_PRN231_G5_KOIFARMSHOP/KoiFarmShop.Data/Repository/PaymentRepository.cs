using KoiFarmShop.Data.Base;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Request;
using KoiFarmShop.Data.Request.KoiFarmShop.Data.Request;
using Microsoft.EntityFrameworkCore;

namespace KoiFarmShop.Data.Repository
{
    public class PaymentRepository : GenericRepository<Payment>
    {
        public PaymentRepository(FA24_SE1717_PRN231_G5_KOIFARMSHOPContext context)
        {
            _context = context;
        }

        public async Task<Payment> CreatePaymentAsync(CreatePaymentRequest createPaymentRequest)
        {
            try
            {
                // Tạo PaymentId
                var paymentCount = await _context.Payments.CountAsync();
                var paymentId = $"PAYMENT{(paymentCount + 1).ToString("D3")}";

                // Lấy UserId từ Order hoặc Consignment
                string userId;
                double amount;

                if (!string.IsNullOrEmpty(createPaymentRequest.OrderId))
                {
                    var order = await _context.Orders.FindAsync(createPaymentRequest.OrderId);
                    if (order == null)
                        throw new Exception("Order not found.");

                    userId = order.UserId;
                    amount = order.TotalAmount; 
                }
                else if (!string.IsNullOrEmpty(createPaymentRequest.ConsignmentId))
                {
                    var consignment = await _context.Consignments.FindAsync(createPaymentRequest.ConsignmentId);
                    if (consignment == null)
                        throw new Exception("Consignment not found.");

                    userId = consignment.UserId;
                    amount = consignment.DealPrice ?? 0; 
                }
                else
                {
                    throw new Exception("Either OrderId or ConsignmentId must be provided.");
                }

                // Khởi tạo Payment
                var payment = new Payment
                {
                    PaymentId = paymentId,
                    UserId = userId,
                    Amount = amount,
                    Type = !string.IsNullOrEmpty(createPaymentRequest.OrderId) ? 1 : 2, // 1: Order, 2: Consignment
                    Status = 2, // Pending
                    CreatedDate = DateTime.Now,
                };

                // Thêm Payment vào context
                await _context.Payments.AddAsync(payment);
                await _context.SaveChangesAsync();

                return payment;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                throw new Exception($"An error occurred while creating the payment: {ex.Message}", ex);
            }
        }
    }
}
