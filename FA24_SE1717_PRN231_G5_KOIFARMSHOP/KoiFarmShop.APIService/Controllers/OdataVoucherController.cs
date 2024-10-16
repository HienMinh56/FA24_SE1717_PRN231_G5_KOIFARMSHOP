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
        [HttpGet("id")]
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
public async Task<ActionResult<Voucher>> CreateVoucher(CreateVoucherRequest voucherRequest)
{
    // Retrieve the existing voucher based on VoucherId
    var existingVoucher = genericRepo.Get(u => u.VoucherId == voucherRequest.VoucherId);
    
    if (existingVoucher != null)
    {

                // Update the existing voucher's properties

                existingVoucher.VoucherCode = voucherRequest.VoucherCode;
                existingVoucher.ModifiedDate = DateTime.Now;
                existingVoucher.ModifiedBy = "new"; // Replace with actual user context
        // Optionally update other fields if needed

        var updateResult = await genericRepo.UpdateAsync(existingVoucher);
        
        if (updateResult > 0)
        {
            return Ok(existingVoucher); // Return 200 OK with the updated voucher
        }
        else
        {
            return StatusCode(500); // Return 500 Internal Server Error if update fails
        }
    }

    // Create a new voucher if it doesn't exist
    var newVoucher = new Voucher
    {
        VoucherId = voucherRequest.VoucherId,
        VoucherCode = voucherRequest.VoucherCode,
        DiscountAmount = voucherRequest.DiscountAmount,
        MinOrderAmount = voucherRequest.MinOrderAmount,
        ValidityStartDate = voucherRequest.ValidityStartDate,
        ValidityEndDate = voucherRequest.ValidityEndDate,
        Status = voucherRequest.Status,
        CreatedDate = DateTime.Now,
        CreatedBy = "current_user", // Replace with actual user context
        ModifiedDate = DateTime.Now,
        ModifiedBy = "current_user" // Replace with actual user context
    };

    var creationResult = await genericRepo.CreateAsync(newVoucher);
    
    if (creationResult > 0)
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
