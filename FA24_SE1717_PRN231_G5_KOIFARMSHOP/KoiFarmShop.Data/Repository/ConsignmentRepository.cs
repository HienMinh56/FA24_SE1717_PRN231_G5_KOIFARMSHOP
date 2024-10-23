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
            var consignments = await _context.Consignments
                                             .AsNoTracking()
                                             .Include(k => k.Koi)
                                             .Include(u => u.User)
                                             .Include(p => p.Payment)
                                                 .ThenInclude(o => o.Orders)
                                                 .DefaultIfEmpty()
                                             .ToListAsync();
            return consignments ?? new List<Consignment>();
        }

        public async Task<Consignment> GetConsignmentByIdAsync(string ConsignmentId)
        {
            var consignment = await _context.Consignments
                                            .AsNoTracking()
                                            .Where(c => c.ConsignmentId == ConsignmentId)
                                            .Include(k => k.Koi)
                                             .Include(u => u.User)
                                             .Include(p => p.Payment)
                                                 .ThenInclude(o => o.Orders)
                                                 .DefaultIfEmpty()
                                            .FirstOrDefaultAsync();

            return consignment ?? new Consignment();
        }
    }
}