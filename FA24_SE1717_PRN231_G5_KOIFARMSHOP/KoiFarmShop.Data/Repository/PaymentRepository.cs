using KoiFarmShop.Data.Base;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Request;
using KoiFarmShop.Data.Request;
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
                string userId = string.Empty;
                double amount = 0;
                string paymentId;

                if (createPaymentRequest.Type == 1) // Type 1: Order
                {
                    var order = await _context.Orders
                                              .AsNoTracking()
                                              .FirstOrDefaultAsync(o => o.OrderId == createPaymentRequest.OrderId);

                    if (order == null)
                    {
                        throw new Exception("Order not found.");
                    }

                    userId = order.UserId;
                    amount = order.TotalAmount;

                    paymentId = $"PAYMENT{(await Count() + 1).ToString("D4")}";
                    order.PaymentId = paymentId;

                    // Cập nhật order
                    _context.Orders.Update(order); // Thêm dòng này để cập nhật Order
                }
                else if (createPaymentRequest.Type == 2) // Type 2: Consignment
                {
                    if (string.IsNullOrEmpty(createPaymentRequest.ConsignmentId))
                    {
                        throw new Exception("Type 2 input ConsignmentId");
                    }

                    var consignment = await _context.Consignments
                                                    .AsNoTracking()
                                                    .FirstOrDefaultAsync(c => c.ConsignmentId == createPaymentRequest.ConsignmentId);

                    if (consignment == null)
                    {
                        throw new Exception("Consignment not found.");
                    }

                    userId = consignment.UserId;
                    amount = consignment.DealPrice ?? 0; // DealPrice null = 0

                    paymentId = $"PAYMENT{(await Count() + 1).ToString("D4")}";
                    consignment.PaymentId = paymentId;

                    // Cập nhật consignment
                    _context.Consignments.Update(consignment);
                }
                else
                {
                    throw new Exception("Type không hợp lệ.");
                }

                var payment = new Payment
                {
                    PaymentId = paymentId,
                    UserId = userId,
                    Amount = amount,
                    Type = createPaymentRequest.Type,
                    Status = 2, // Pending
                    CreatedDate = DateTime.Now
                };

                await _context.Payments.AddAsync(payment);
                await _context.SaveChangesAsync();

                return payment;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Payment?> GetPaymentByIdAsync(string paymentId)
        {
            var payment = await _context.Payments
                                        .AsNoTracking()
                                        .Include(p => p.User)  
                                        .Include(p => p.Orders)  
                                        .Include(p => p.Consignments)  
                                        .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            return payment;
        }
     
    }
}
