using KoiFarmShop.Common;
using KoiFarmShop.Data;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Repository;
using KoiFarmShop.Service.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Service
{
    public interface IOrderService
    {
        Task<IBusinessResult> GetOrders();
        Task<IBusinessResult> GetOrderById(string OrderId);
        Task<IBusinessResult> CreateOrderAsync(List<(string koiId, int quantity)> orderDetails, string? voucherId);
        Task<IBusinessResult> UpdateOrderAsync(string OrderId, int status);
    }

    public class OrderService : IOrderService
    {
        private readonly UnitOfWork _unitOfWork;
        public OrderService()
        {
            _unitOfWork ??= new UnitOfWork();
        }


        public async Task<IBusinessResult> GetOrders()
        {
            try
            {
                var orders = await _unitOfWork.OrderRepository.GetAllAsync();
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, orders);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có ngoại lệ
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> GetOrderById(string OrderId)
        {
            try
            {
                var order = _unitOfWork.OrderRepository.Get(o => o.OrderId == OrderId);
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, order);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có ngoại lệ
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> CreateOrderAsync(List<(string koiId, int quantity)> orderDetails, string? voucherId)
        {
            try
            {
                var userId = "S1";
                var order = await _unitOfWork.OrderRepository.CreateOrderAsync(userId, orderDetails, voucherId);
                return new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có ngoại lệ
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> UpdateOrderAsync(string OrderId, int status)
        {
            try
            {
                var order = _unitOfWork.OrderRepository.Get(o => o.OrderId == OrderId);
                order.Status = status;
                await _unitOfWork.OrderRepository.UpdateAsync(order);
                return new BusinessResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có ngoại lệ
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }



    }
}
