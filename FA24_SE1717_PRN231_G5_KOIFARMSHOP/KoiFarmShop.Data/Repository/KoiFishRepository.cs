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

        public async Task<List<KoiFish>> SearchKoiFish(QueryKoiFishRequest request)
        {
            var query = _context.KoiFishes
                .AsNoTracking();

            query = request.KoiName is null ? query : query.Where(q => q.KoiName.Contains(request.KoiName));
            query = request.Origin is null ? query : query.Where(q => q.Origin.Contains(request.Origin));
            query = request.Gender is null ? query : query.Where(q => q.Gender.Equals(request.Gender));
            query = request.Age == 0 ? query : query.Where(q => q.Age == request.Age);
            query = request.Size == 0 ? query : query.Where(q => q.Size == request.Size);
            query = request.Breed is null ? query : query.Where(q => q.Breed.Contains(request.Breed));
            query = request.Type is null ? query : query.Where(q => q.Type.Equals(request.Type));
            query = request.Price == 0 ? query : query.Where(q => q.Price == request.Price);
            query = request.Quantity == 0 ? query : query.Where(q => q.Quantity == request.Quantity);

            var result = await query.ToListAsync();
            return result;
        }
    }
}
