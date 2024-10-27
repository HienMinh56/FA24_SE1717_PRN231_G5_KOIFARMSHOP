using KoiFarmShop.Data.Base;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Request;
using Microsoft.EntityFrameworkCore;

namespace KoiFarmShop.Data.Repository
{
    public class ConsignmentRepository : GenericRepository<Consignment>
    {
        public ConsignmentRepository()
        {
        }

        public ConsignmentRepository(FA24_SE1717_PRN231_G5_KOIFARMSHOPContext context)
        {
            _context = context;
        }
        
        public async Task<List<Consignment>> GetAllConsignmentAsync()
        {
            return  _context.Consignments.Include(u => u.User)
                .Include(k => k.Koi)
                .Include(p => p.Payment)
                .ToList();
        }

        public async Task<Consignment> GetConsignmentByIdAsync(string consignmentId)
        {
            return await _context.Consignments.Include(u => u.User)
                .Include(k => k.Koi)
                .Include(p => p.Payment)
                .FirstOrDefaultAsync(c => c.ConsignmentId == consignmentId);
        }
    }
}