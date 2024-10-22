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
        public async Task<IBusinessResult> GetOrders()
        {
            return await _orderService.GetOrders();
        }

        [HttpGet("{OrderId}")]
        public async Task<IBusinessResult> GetOrderById(string OrderId)
        {
            return await _orderService.GetOrderById(OrderId);
        }

        [HttpPost]
        public async Task<IBusinessResult> CreateOrder([FromBody] Order order)
        {
            // Call the service to create the order
            return await _orderService.Save(order);
        }

        [HttpPut]
        public async Task<IBusinessResult> UpdateOrder([FromBody] Order order)
        {
            return await _orderService.Save(order);
        }
        [HttpDelete("{orderId}")]
        public async Task<IBusinessResult> DeleteOrder(string orderId)
        {
            return await _orderService.DeleteOrderAsync(orderId);
        }
    }
}
