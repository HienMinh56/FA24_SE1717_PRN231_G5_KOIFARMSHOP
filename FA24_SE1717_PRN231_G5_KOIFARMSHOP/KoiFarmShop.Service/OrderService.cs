using KoiFarmShop.Common;
using KoiFarmShop.Data;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Repository;
using KoiFarmShop.Data.Request;
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
        Task<IBusinessResult> GetOrderById(string orderId);
        Task<IBusinessResult> CreateOrderAsync(string userId, List<OrderItem> orderDetails, string? voucherId, DateTime createTime, string createBy);
        Task<IBusinessResult> UpdateOrderAsync(UpdateOrderRequest orderRequest);
        Task<IBusinessResult> DeleteOrderAsync(string orderId);
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

        public async Task<IBusinessResult> GetOrderById(string orderId)
        {
            try
            {
                var order = _unitOfWork.OrderRepository.Get(o => o.OrderId == orderId);
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, order);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có ngoại lệ
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> CreateOrderAsync(string userId, List<OrderItem> orderDetails, string? voucherCode, DateTime createTime, string createBy)
        {
            try
            {
                var order = await _unitOfWork.OrderRepository.CreateOrderAsync(userId, orderDetails, voucherCode, createTime, createBy);
                return new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có ngoại lệ
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> UpdateOrderAsync(UpdateOrderRequest orderRequest)
        {
            try
            {
                var order =  _unitOfWork.OrderRepository.Get(o => o.OrderId == orderRequest.OrderId);
                if (order == null)
                {
                    return new BusinessResult(Const.ERROR_EXCEPTION, "Order not found");
                }

                order.Status = orderRequest.Status;
                order.ModifiedDate = orderRequest.ModifiedDate;
                order.ModifiedBy = orderRequest.ModifiedBy;

                if (!string.IsNullOrEmpty(orderRequest.VoucherId))
                {
                    var voucher = _unitOfWork.VoucherRepository.Get(v => v.VoucherId == orderRequest.VoucherId && v.ValidityStartDate <= DateTime.Now && v.ValidityEndDate >= DateTime.Now);
                    if (voucher == null)
                    {
                        throw new Exception("Invalid or expired voucher.");
                    }

                    if (voucher.MinOrderAmount <= order.TotalAmount)
                        order.TotalAmount = order.TotalAmount - (int)voucher.DiscountAmount;
                    else
                    {
                        throw new Exception("Min amount is not invalid");
                    }
                    order.VoucherId = voucher.VoucherId;
                }

                await _unitOfWork.OrderRepository.UpdateAsync(order);
                return new BusinessResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có ngoại lệ
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> DeleteOrderAsync(string orderId)
        {
            try
            {
                var order = _unitOfWork.OrderRepository.Get(o => o.OrderId == orderId);
                var orderDetails = _unitOfWork.OrderDetailRepository.GetList(od => od.OrderId == orderId);
                if (order == null)
                {
                    return new BusinessResult(Const.ERROR_EXCEPTION, "Order not found or it have done");
                }
                //order.Status = 4;
                foreach (var item in orderDetails)
                {
                    await _unitOfWork.OrderDetailRepository.RemoveAsync(item);
                }
                await _unitOfWork.OrderRepository.RemoveAsync(order);
                
                return new BusinessResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có ngoại lệ
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }


    }
}
