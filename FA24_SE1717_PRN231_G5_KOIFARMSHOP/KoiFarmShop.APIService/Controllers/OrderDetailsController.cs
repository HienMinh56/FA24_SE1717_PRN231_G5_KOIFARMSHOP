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
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderDetailService _detailService;

        public OrderDetailsController(IOrderDetailService detailService)
        {
            _detailService = detailService;
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<IBusinessResult>> GetOrderDetailsByOrderId(string orderId)
        {
            var result = await _detailService.GetOrderDetailsByOrderId(orderId);
            if (result.Status == 1)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
