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

        public async Task<List<Order>> GetAllOrderAsync()
        {
            return  _context.Orders.Include(u => u.User)
                                        .Include(od => od.OrderDetails)
                                        .Include(v => v.Voucher)
                                        .ToList();
        }

        public async Task<Order> GetOrderByIdAsync(string orderId)
        {
            return await _context.Orders.Include(u => u.User)
                                        .Include(od => od.OrderDetails)
                                        .Include(v => v.Voucher)
                                        .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
    }

    

}
