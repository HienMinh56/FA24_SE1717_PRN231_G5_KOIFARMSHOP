using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Service;
using KoiFarmShop.Service.Base;

namespace KoiFarmShop.APIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VouchersController : ControllerBase
    {
        private readonly VoucherService _voucherService;

        public VouchersController()
        {
            _voucherService ??= new VoucherService();
        }

        // GET: api/Vouchers
        [HttpGet]
        public async Task<IBusinessResult> GetVouchers()
        {
            return await _voucherService.GetAll();
        }

        // GET: api/Vouchers/5
        [HttpGet("{voucherId}")]
        public async Task<IBusinessResult> GetVoucher(string voucherId)
        {
            return await _voucherService.GetById(voucherId);
        }

        // PUT: api/Vouchers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IBusinessResult> PutVoucher( Voucher voucher)
        {
            return await _voucherService.Save(voucher);
        }

        // POST: api/Vouchers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IBusinessResult> PostVoucher(Voucher voucher)
        {
            return await _voucherService.Save(voucher);
        }

        // DELETE: api/Vouchers/5
        [HttpDelete]
        public async Task<IBusinessResult> DeleteVoucher(string voucherId)
        {
            return await _voucherService.DeleteById(voucherId);
        }


    }
}
