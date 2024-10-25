using KoiFarmShop.Common;
using KoiFarmShop.Data;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Repository;
using KoiFarmShop.Data.Request;
using KoiFarmShop.Data.Response;
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
        Task<IBusinessResult> Create(Order order);
        Task<IBusinessResult> Update(Order order);
        Task<IBusinessResult> Save(Order order);
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
                var orders = await _unitOfWork.OrderRepository.GetAllOrderAsync();
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
                var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId);

                if (order == null)
                {
                    return new BusinessResult(Const.FAIL_READ_CODE, "Order not found");
                }

                var result = new GetOrderResponse
                {
                    OrderId = order.OrderId,
                    UserId = order.UserId,
                    Status = order.Status,
                    VoucherId = order.VoucherId,
                    ShippingAddress = order.ShippingAddress,
                    PaymentMethod = order.PaymentMethod,
                    DeliveryDate = order.DeliveryDate,
                    ReceiveDate = order.ReceiveDate,
                    Note = order.Note,
                    TotalWeight = order.TotalWeight,
                    TotalAmount = order.TotalAmount,
                    Quantity = order.Quantity,
                    User = new UserDto
                    {
                        UserId = order.User.UserId,
                        UserName = order.User.FullName,
                    },
                    Voucher = new VoucherDto
                    {
                        VoucherId = order.Voucher.VoucherId,
                        VoucherCode = order.Voucher.VoucherCode,
                    },
                    OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                    {
                        KoiId = od.KoiId,
                        Quantity = od.Quantity,
                    }).ToList(), // Chắc chắn trả về danh sách
                };

                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result); // Trả về result
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có ngoại lệ
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }


        public async Task<IBusinessResult> Create(Order order)
        {
            if (order is null)
            {
                return new BusinessResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
            }

            try
            {
                int result = await _unitOfWork.OrderRepository.CreateAsync(order);

                if (result > 0)
                {
                    return new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG);
                }
                else
                {
                    return new BusinessResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> Update(Order order)
        {
            if (order is null)
            {
                return new BusinessResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }

            try
            {
                int result = await _unitOfWork.OrderRepository.UpdateAsync(order);

                if (result > 0)
                {
                    return new BusinessResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, order);
                }
                else
                {
                    return new BusinessResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
        public async Task<IBusinessResult> Save(Order order)
        {
            try
            {
                int result = -1;

                Order orderTmp = _unitOfWork.OrderRepository.Get(o => o.OrderId == order.OrderId);

                if (orderTmp != null)
                {
                    orderTmp.UserId = order.UserId;
                    orderTmp.TotalWeight = order.TotalWeight;
                    orderTmp.PaymentMethod = order.PaymentMethod;
                    orderTmp.ShippingAddress = order.ShippingAddress;
                    orderTmp.DeliveryDate = order.DeliveryDate;
                    orderTmp.ReceiveDate = order.ReceiveDate;
                    orderTmp.Note = order.Note;
                    orderTmp.Status = order.Status;
                    orderTmp.ModifiedDate = DateTime.Now;
                    orderTmp.ModifiedBy = "User";
                    var totalAmount = 0.0;
                    var totalQuantity = 0;
                    foreach (var item in order.OrderDetails)
                    {
                        var koi = _unitOfWork.KoiFishRepository.Get(k => k.KoiId == item.KoiId);
                        var orderDetail = _unitOfWork.OrderDetailRepository.Get(od => od.OrderId == order.OrderId && od.KoiId == item.KoiId);
                        if (orderDetail == null)
                        {
                            throw new Exception("Order detail not found");
                        }
                        orderDetail.Quantity = item.Quantity;
                        orderDetail.Price = koi.Price;
                        totalAmount += koi.Price * item.Quantity;
                        totalQuantity += item.Quantity;
                        orderTmp.VoucherId = order.VoucherId;

                        await _unitOfWork.OrderDetailRepository.UpdateAsync(orderDetail);
                    }
                    var voucher = _unitOfWork.VoucherRepository.Get(v => v.VoucherId == orderTmp.VoucherId);

                    if (order.VoucherId != null && totalAmount >= voucher.MinOrderAmount)
                    {
                        totalAmount = totalAmount - (totalAmount * voucher.DiscountAmount) / 100;
                    }
                    else
                    {
                        return new BusinessResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                    }
                    orderTmp.TotalAmount = totalAmount;
                    orderTmp.Quantity = totalQuantity;
                    result = await _unitOfWork.OrderRepository.UpdateAsync(orderTmp);

                    if (result > 0)
                    {
                        return new BusinessResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, orderTmp);
                    }
                    else
                    {
                        return new BusinessResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                    }
                }
                else
                {
                    order.OrderId = $"ORDER{(await _unitOfWork.OrderRepository.Count() + 1).ToString("D4")}";
                    order.CreatedDate = DateTime.Now;
                    order.CreatedBy = "User";

                    var totalAmount = 0.0;
                    var totalQuantity = 0;
                    foreach (var item in order.OrderDetails)
                    {
                        var koi = _unitOfWork.KoiFishRepository.Get(k => k.KoiId == item.KoiId);
                        item.OrderId = order.OrderId;
                        item.KoiId = item.KoiId;
                        item.Price = koi.Price;
                        item.Quantity = item.Quantity;
                        totalAmount += item.Price * item.Quantity;
                        totalQuantity += item.Quantity;
                    }
                    var voucher = _unitOfWork.VoucherRepository.Get(v => v.VoucherId == order.VoucherId);

                    if (order.VoucherId != null && totalAmount >= voucher.MinOrderAmount)
                    {
                        totalAmount = totalAmount - (totalAmount * voucher.DiscountAmount)/100;
                    }
                    else
                    {
                        return new BusinessResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
                    }
                    order.TotalAmount = totalAmount;
                    order.Quantity = totalQuantity;

                    result = await _unitOfWork.OrderRepository.CreateAsync(order);

                    if (result > 0)
                    {
                        return new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG);
                    }
                    else
                    {
                        return new BusinessResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
                    }
                }
            }
            catch (Exception ex)
            {
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
