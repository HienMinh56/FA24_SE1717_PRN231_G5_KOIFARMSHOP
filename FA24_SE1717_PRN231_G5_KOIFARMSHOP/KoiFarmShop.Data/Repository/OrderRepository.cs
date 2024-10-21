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

        public async Task<Order> CreateOrderAsync(OrderCreateRequest orderCreateRequest)
        {
            try
            {
                string orderId = $"ORDER{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
                string paymentId = $"PAY{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

                

                var order = new Order
                {
                    OrderId = orderId,
                    UserId = orderCreateRequest.UserId,
                    Status = 1, 
                    CreatedDate = orderCreateRequest.CreateTime,
                    CreatedBy = orderCreateRequest.CreateBy,
                };

                double totalAmount = 0;
                int totalQuantity = 0;

                foreach (var item in orderCreateRequest.OrderItems)
                {
                    var koi = await _context.KoiFishes
                        .Where(k => k.DeletedBy == null && k.KoiId == item.KoiId)
                        .AsNoTracking()
                        .SingleOrDefaultAsync();

                    if (koi == null)
                    {
                        throw new Exception($"Koi with ID {item.KoiId} not found.");
                    }

                    var orderDetail = new OrderDetail
                    {
                        KoiId = item.KoiId, 
                        Quantity = item.Quantity,
                        Price = koi.Price * item.Quantity,
                        OrderId = orderId,
                    };

                    totalAmount += orderDetail.Price;
                    totalQuantity += item.Quantity;
                    
                    order.OrderDetails.Add(orderDetail);
                    if (!string.IsNullOrEmpty(orderCreateRequest.VoucherCode))
                    {
                        var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.VoucherCode == orderCreateRequest.VoucherCode && v.ValidityStartDate <= DateTime.Now && v.ValidityEndDate >= DateTime.Now);
                        if (voucher == null)
                        {
                            throw new Exception("Invalid or expired voucher.");
                        }

                        if (voucher.MinOrderAmount <= totalAmount)
                            totalAmount = totalAmount - (int)voucher.DiscountAmount*totalAmount/100;
                        else
                        {
                            throw new Exception("Min amount is not invalid");
                        }
                        order.VoucherId = voucher.VoucherId;
                    }
                }

                

                order.TotalAmount = totalAmount;
                order.Quantity = totalQuantity;

                await _context.Orders.AddAsync(order);

                await _context.SaveChangesAsync();

                return order;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while creating the order: {ex.Message}", ex);
            }
        }


    }

}
