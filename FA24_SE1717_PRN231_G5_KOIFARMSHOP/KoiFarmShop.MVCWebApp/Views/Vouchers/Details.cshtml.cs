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
    public class DetailsModel : PageModel
    {
        private readonly KoiFarmShop.Data.Models.FA24_SE1717_PRN231_G5_KOIFARMSHOPContext _context;

        public DetailsModel(KoiFarmShop.Data.Models.FA24_SE1717_PRN231_G5_KOIFARMSHOPContext context)
        {
            _context = context;
        }

        public Voucher Voucher { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voucher = await _context.Vouchers.FirstOrDefaultAsync(m => m.Id == id);
            if (voucher == null)
            {
                return NotFound();
            }
            else
            {
                Voucher = voucher;
            }
            return Page();
        }
    }
}
