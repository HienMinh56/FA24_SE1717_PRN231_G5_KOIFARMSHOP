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
    public class OrderDetailRepository : GenericRepository<OrderDetail>
    {
        public OrderDetailRepository()
        {
        }

        public OrderDetailRepository(FA24_SE1717_PRN231_G5_KOIFARMSHOPContext context) => _context = context;
        public List<OrderDetail> GetOrderDetailsByOrderId(string orderId)
        {
            return _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .Include(o => o.Koi)
                .ToList();
        }

    }
}
