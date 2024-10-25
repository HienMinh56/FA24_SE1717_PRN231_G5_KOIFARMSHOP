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
    }
}