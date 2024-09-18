using KoiFarmShop.Data.Base;
using KoiFarmShop.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Repository
{
    public class VoucherRepository : GenericRepository<Voucher>
    {
        public VoucherRepository()
        {
        }

        public VoucherRepository(FA24_SE1717_PRN231_G5_KOIFARMSHOPContext context) => _context = context;
    }
}
