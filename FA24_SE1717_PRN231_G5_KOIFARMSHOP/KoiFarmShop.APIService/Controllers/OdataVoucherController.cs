using KoiFarmShop.Data.Base;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Repository;
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



    }
}
