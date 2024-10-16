using KoiFarmShop.Data;
using KoiFarmShop.Data.Base;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Repository;
using KoiFarmShop.Data.Request;
using KoiFarmShop.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace KoiFarmShop.APIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OdataVoucherController : ODataController
    {
        GenericRepository<Voucher> genericRepo;

        public OdataVoucherController()
        {
            genericRepo = new VoucherRepository();
        }

        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<Voucher>>> GetVouchers()
        {
            return await genericRepo.GetAllAsync();
        }
        [HttpDelete]
        [EnableQuery]
        public async Task<ActionResult<Voucher>> DeleteVoucher(string id)
        {
            var voucher = genericRepo.Get(u => u.VoucherId == id);
            if (voucher == null)
            {
                return NotFound(); // Return 404 Not Found if voucher is not found
            }

            bool isRemoved = await genericRepo.RemoveAsync(voucher);
            if (isRemoved)
            {
                return Ok(voucher); // Return 200 OK with the deleted voucher
            }
            else
            {
                return StatusCode(500); // Return 500 Internal Server Error if deletion fails
            }
        }
        [HttpDelete]
        [EnableQuery]
        public async Task<ActionResult<Voucher>> GetVoucherById(string id)
        {
            var voucher = genericRepo.Get(u => u.VoucherId == id);
            if (voucher == null)
            {
                return NotFound(); // Return 404 Not Found if voucher is not found
            }
            return Ok(voucher);
        }
        [HttpPost]
        [EnableQuery]
        public async Task<ActionResult<Voucher>> CreateVoucher(CreateVoucherRequest Voucher)
        {
            var voucher = genericRepo.Get(u => u.VoucherId == Voucher.VoucherId);
            if (voucher != null)
            {
                var updateVoucher = new Voucher
                {
                    VoucherId = voucher.VoucherId,
                    VoucherCode = voucher.VoucherCode,
                    DiscountAmount = voucher.DiscountAmount,
                    MinOrderAmount = voucher.MinOrderAmount,
                    ValidityStartDate = voucher.ValidityStartDate,
                    ValidityEndDate = voucher.ValidityEndDate,
                    Status = voucher.Status,
                    CreatedDate = DateTime.Now,
                    CreatedBy = voucher.CreatedBy,
                    ModifiedDate = DateTime.Now,
                    ModifiedBy = voucher.ModifiedBy,
                };
                var t = await genericRepo.UpdateAsync(updateVoucher);
                if (t > 0)
                {
                    return Ok(updateVoucher);
                }
                else
                {
                    return StatusCode(500);
                }



            }


            var newVoucher = new Voucher
            {
                VoucherId = voucher.VoucherId,
                VoucherCode = voucher.VoucherCode,
                DiscountAmount = voucher.DiscountAmount,
                MinOrderAmount = voucher.MinOrderAmount,
                ValidityStartDate = voucher.ValidityStartDate,
                ValidityEndDate = voucher.ValidityEndDate,
                Status = voucher.Status,
                CreatedDate = DateTime.Now,
                CreatedBy = voucher.CreatedBy,
                ModifiedDate = DateTime.Now,
                ModifiedBy = voucher.ModifiedBy,
            };

            var result = await genericRepo.CreateAsync(newVoucher);
            if (result > 0)
            {
                return Ok(newVoucher); // Return 200 OK with the created voucher
            }
            else
            {
                return StatusCode(500); // Return 500 Internal Server Error if creation fails
            }
        }


    }
}
