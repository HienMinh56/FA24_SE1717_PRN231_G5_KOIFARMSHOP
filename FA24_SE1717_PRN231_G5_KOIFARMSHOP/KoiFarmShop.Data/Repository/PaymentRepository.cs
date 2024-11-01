using KoiFarmShop.Data.Base;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Request;
using Microsoft.EntityFrameworkCore;

namespace KoiFarmShop.Data.Repository
{
    public class PaymentRepository : GenericRepository<Payment>
    {
        public PaymentRepository()
        {
        }
        public PaymentRepository(FA24_SE1717_PRN231_G5_KOIFARMSHOPContext context) => _context = context;

        public async Task<Payment> GetPaymentByIdAsync(string paymentId)
        {
            return await _context.Payments.Include(o => o.Orders)
                .Include(c => c.Consignments)
                .Include(u => u.User)
                .FirstOrDefaultAsync(c => c.PaymentId == paymentId);
        }
    }
}
