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
        public async Task<ActionResult<IBusinessResult>> CreateOrder([FromBody] OrderCreateRequest request)
        {
            // Call the service to create the order
            var result = await _orderService.CreateOrderAsync(request.UserId,request.OrderItems, request.VoucherCode);

            // Check the result and return the appropriate response
            if (result.Status == 1)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPut]
        public async Task<ActionResult<IBusinessResult>> UpdateOrder(UpdateOrderRequest orderRequest)
        {
            var result = await _orderService.UpdateOrderAsync(orderRequest);
            if (result.Status == 1)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        [HttpDelete("{orderId}")]
        public async Task<ActionResult<IBusinessResult>> DeleteOrder(string orderId)
        {
            var result = await _orderService.DeleteOrderAsync(orderId);
            if (result.Status == 1)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
