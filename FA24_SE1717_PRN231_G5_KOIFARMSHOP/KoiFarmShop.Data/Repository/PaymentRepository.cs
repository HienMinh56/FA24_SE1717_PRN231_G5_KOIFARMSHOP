﻿using KoiFarmShop.Data.Base;
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

        // Hàm tạo Payment với thông tin từ Order hoặc Consignment
        public async Task<Payment> CreatePaymentAsync(CreatePaymentRequest createPaymentRequest)
        {
            try
            {
                string userId = string.Empty;
                double amount = 0;

                if (createPaymentRequest.Type == 1) // Type 1: Order
                {
                    var order = await _context.Orders
                                              .AsNoTracking()
                                              .FirstOrDefaultAsync(o => o.OrderId == createPaymentRequest.OrderId);

                    if (order == null)
                    {
                        throw new Exception("Order không tồn tại.");
                    }

                    userId = order.UserId;
                    amount = order.TotalAmount;

                    // Gán PaymentId
                    order.PaymentId = $"PAYMENT{(await Count() + 1).ToString("D4")}";
                }
                else if (createPaymentRequest.Type == 2) // Type 2: Consignment
                {
                    if (string.IsNullOrEmpty(createPaymentRequest.ConsignmentId))
                    {
                        throw new Exception("ConsignmentId không thể để trống khi Type là 2.");
                    }

                    var consignment = await _context.Consignments
                                                    .AsNoTracking()
                                                    .FirstOrDefaultAsync(c => c.ConsignmentId == createPaymentRequest.ConsignmentId);

                    if (consignment == null)
                    {
                        throw new Exception("Consignment không tồn tại.");
                    }

                    userId = consignment.UserId;
                    amount = consignment.DealPrice ?? 0; // DealPrice null = 0

                    // Gán PaymentId
                    consignment.PaymentId = $"PAYMENT{(await Count() + 1).ToString("D4")}";
                }
                else
                {
                    throw new Exception("Type không hợp lệ.");
                }

                // Tạo PaymentId
                string paymentId = $"PAYMENT{(await Count() + 1).ToString("D4")}";

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
                throw new Exception($"Đã xảy ra lỗi khi tạo Payment: {ex.Message}", ex);
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
