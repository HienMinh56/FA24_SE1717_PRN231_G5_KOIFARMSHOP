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
    public class ImageRepository : GenericRepository<Image>
    {
        public ImageRepository()
        {
        }

        public ImageRepository(FA24_SE1717_PRN231_G5_KOIFARMSHOPContext context) => _context = context;

        public async Task<List<Image>> GetAllOrderByImageId()
        {
            return await _context.Images
                .Where(i => i.DeletedBy == null)
                .OrderBy(i => i.ImageId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Image?> GetImageByUrl(string url)
        {
            return await _context.Images
                .Where(i => i.DeletedBy == null)
                .SingleOrDefaultAsync(i => i.Url == url);
        }
    }
}
