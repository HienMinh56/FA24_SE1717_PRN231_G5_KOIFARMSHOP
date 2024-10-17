using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KoiFarmShop.Data.Models;

namespace KoiFarmShop.MVCWebApp.Views.Vouchers
{
    public class IndexModel : PageModel
    {
        private readonly KoiFarmShop.Data.Models.FA24_SE1717_PRN231_G5_KOIFARMSHOPContext _context;

        public IndexModel(KoiFarmShop.Data.Models.FA24_SE1717_PRN231_G5_KOIFARMSHOPContext context)
        {
            _context = context;
        }

        public IList<Voucher> Voucher { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Voucher = await _context.Vouchers.ToListAsync();
        }
    }
}
