using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Service;
using KoiFarmShop.Data.Request;
using KoiFarmShop.Service.Base;
using KoiFarmShop.Common;

namespace KoiFarmShop.APIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }


        [HttpGet]
        public async Task<ActionResult<IBusinessResult>> GetOrders()
        {
            var result = await _orderService.GetOrders();
            if (result.Status == 1)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("{OrderId}")]
        public async Task<ActionResult<IBusinessResult>> GetOrderById(string OrderId)
        {
            var result = await _orderService.GetOrderById(OrderId);
            if (result.Status == 1)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost]
        public async Task<ActionResult<IBusinessResult>> CreateOrder([FromBody] List<CreateOrderRequest> request)
        {
            var orderItems = request.Select(x => (x.KoiId, x.Quantity)).ToList();
            // Call the service to create the order
            var result = await _orderService.CreateOrderAsync(orderItems);

            // Check the result and return the appropriate response
            if (result.Status == 1)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPut("{OrderId}")]
        public async Task<ActionResult<IBusinessResult>> UpdateOrder(string OrderId, int status)
        {
            var result = await _orderService.UpdateOrderAsync(OrderId, status);
            if (result.Status == 1)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
