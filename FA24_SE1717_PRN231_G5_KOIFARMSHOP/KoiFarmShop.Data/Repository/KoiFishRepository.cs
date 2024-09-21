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
    public class KoiFishRepository : GenericRepository<KoiFish>
    {
        public KoiFishRepository()
        {
        }

        public KoiFishRepository(FA24_SE1717_PRN231_G5_KOIFARMSHOPContext context) => _context = context;

        public async Task<KoiFish?> GetByIdWithImages(string code)
        {
            return await _context.KoiFishes
                .Where(k => k.DeletedBy == null)
                .Where(k => k.KoiId == code)
                .Include(k => k.Images)
                .SingleOrDefaultAsync();
        }
        public async Task<KoiFish?> GetByIdWithDetail(string code)
        {
            return await _context.KoiFishes
                .Where(k => k.DeletedBy == null)
                .Where(k => k.KoiId == code)
                .Include(k => k.Images)
                .Include(k => k.OrderDetails)
                .Include(k => k.Consignments)
                .SingleOrDefaultAsync();
        }

        public async Task<List<KoiFish>> GetAllWithImages()
        {
            return await _context.KoiFishes
                .Where(k => k.DeletedBy == null)
                .OrderBy(k => k.KoiId)
                .AsNoTracking()
                .Include(k => k.Images)
                .ToListAsync();
        }

        public async Task<List<KoiFish>> GetAllOrderedByKoiId()
        {
            return await _context.KoiFishes
                .Where(k => k.DeletedBy == null)
                .OrderBy(k => k.KoiId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
