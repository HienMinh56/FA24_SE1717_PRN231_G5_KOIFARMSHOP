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
        Task<IBusinessResult> Save(CreateOrderRequest order);
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
                    VoucherId = order.VoucherId ?? "",
                    ShippingAddress = order.ShippingAddress,
                    PaymentMethod = order.PaymentMethod,
                    DeliveryDate = order.DeliveryDate,
                    ReceiveDate = order.ReceiveDate,
                    Note = order.Note ?? "",
                    TotalWeight = order.TotalWeight,
                    TotalAmount = order.TotalAmount,
                    Quantity = order.Quantity,
                    User = new UserDto
                    {
                        UserId = order.User.UserId,
                        UserName = order.User.FullName,
                    },
                    //Voucher = new VoucherDto
                    //{
                    //    VoucherId = order.Voucher.VoucherId,
                    //    VoucherCode = order.Voucher.VoucherCode,
                    //},
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
        public async Task<IBusinessResult> Save(CreateOrderRequest order)
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
                    if (order.OrderDetails.Count == 0)
                    {
                        var totalAmount = 0.0;
                        var totalQuantity = 0;
                        var orderDetail = _unitOfWork.OrderDetailRepository.GetOrderDetailsByOrderId(orderTmp.OrderId);
                        foreach (var item in orderDetail)
                        {
                            var koi = _unitOfWork.KoiFishRepository.Get(k => k.KoiId == item.KoiId);
                            var orderDetail1 = _unitOfWork.OrderDetailRepository.Get(od => od.OrderId == order.OrderId && od.KoiId == item.KoiId);
                            if (orderDetail == null)
                            {

                                throw new Exception("Order detail not found");
                            }
                            orderDetail1.Quantity = item.Quantity;
                            orderDetail1.Price = koi.Price;
                            totalAmount += koi.Price * item.Quantity;
                            totalQuantity += item.Quantity; 

                            if (koi.Quantity < 0)
                            {
                                return new BusinessResult(Const.FAIL_UPDATE_CODE, "Koi fish out of stock");
                            }

                            _unitOfWork.OrderDetailRepository.Update(orderDetail1);
                            _unitOfWork.KoiFishRepository.Update(koi);

                            var voucher = _unitOfWork.VoucherRepository.Get(v => v.VoucherCode == order.VoucherCode);
                            var existVoucher = _unitOfWork.VoucherRepository.Get(v => v.VoucherId == orderTmp.VoucherId);
                            if (voucher == null && existVoucher == null)
                            {
                                orderTmp.VoucherId = null;
                                orderTmp.TotalAmount = totalAmount;
                                orderTmp.Quantity = totalQuantity;
                            }
                            else if(voucher == null && existVoucher != null)
                            {
                                orderTmp.VoucherId = null;
                                orderTmp.TotalAmount = totalAmount;
                                orderTmp.Quantity = totalQuantity;
                                existVoucher.Quantity += 1;
                                _unitOfWork.VoucherRepository.Update(existVoucher);
                            }
                            else if (voucher != null && existVoucher ==null && totalAmount >= voucher.MinOrderAmount && voucher.Status == 1 &&
                        voucher.ValidityEndDate >= DateTime.Now && voucher.ValidityStartDate <= DateTime.Now)
                            {
                                totalAmount = totalAmount - (totalAmount * voucher.DiscountAmount) / 100;
                                voucher.Quantity = voucher.Quantity - 1;
                                orderTmp.VoucherId = voucher.VoucherId;
                                orderTmp.TotalAmount = totalAmount;
                                orderTmp.Quantity = totalQuantity;
                                if (voucher.Quantity < 0)
                                {
                                    return new BusinessResult(Const.FAIL_UPDATE_CODE, "Voucher out of stock");
                                }

                                _unitOfWork.VoucherRepository.Update(voucher);
                            }
                            else if ( voucher != null && existVoucher!= null && totalAmount >= voucher.MinOrderAmount && voucher.Status == 1 &&
                        voucher.ValidityEndDate >= DateTime.Now && voucher.ValidityStartDate <= DateTime.Now)
                            {
                                totalAmount = totalAmount - (totalAmount * voucher.DiscountAmount) / 100;
                                existVoucher.Quantity += 1;
                                voucher.Quantity = voucher.Quantity - 1;
                                orderTmp.VoucherId = voucher.VoucherId;
                                orderTmp.TotalAmount = totalAmount;
                                orderTmp.Quantity = totalQuantity;
                                if (voucher.Quantity < 0)
                                {
                                    return new BusinessResult(Const.FAIL_UPDATE_CODE, "Voucher out of stock");
                                }

                                _unitOfWork.VoucherRepository.Update(voucher);
                                _unitOfWork.VoucherRepository.Update(existVoucher);
                            }
                            else
                            {
                                return new BusinessResult(Const.FAIL_UPDATE_CODE, "Cannot apply voucher!!!");
                            }


                        }

                    }
                    #region if have order details
                    //else
                    //{
                    //    var totalAmount = 0.0;
                    //    var totalQuantity = 0;
                    //    foreach (var item in orderTmp.OrderDetails)
                    //    {
                    //        var koi = _unitOfWork.KoiFishRepository.Get(k => k.KoiId == item.KoiId);
                    //        var orderDetail = _unitOfWork.OrderDetailRepository.Get(od => od.OrderId == order.OrderId && od.KoiId == item.KoiId);
                    //        if (orderDetail == null)
                    //        {


                    //            throw new Exception("Order detail not found");
                    //        }
                    //        orderDetail.Quantity = item.Quantity;
                    //        orderDetail.Price = koi.Price;
                    //        totalAmount += koi.Price * item.Quantity;
                    //        totalQuantity += item.Quantity;
                    //        koi.Quantity -= item.Quantity; // reduce koifish quantity according to amount of orderdetail quantity

                    //        await _unitOfWork.OrderDetailRepository.UpdateAsync(orderDetail);
                    //        var voucher = _unitOfWork.VoucherRepository.Get(v => v.VoucherCode == order.VoucherCode);
                    //        var existVoucher = _unitOfWork.VoucherRepository.Get(v => v.VoucherId == orderTmp.VoucherId);
                    //        if (voucher != null && totalAmount >= voucher.MinOrderAmount && voucher.Status == 1 &&
                    //    voucher.ValidityEndDate >= DateTime.Now && voucher.ValidityStartDate <= DateTime.Now)
                    //        {
                    //            totalAmount = totalAmount - (totalAmount * voucher.DiscountAmount) / 100;
                    //            existVoucher.Quantity += 1;
                    //            voucher.Quantity = voucher.Quantity - 1;
                    //            orderTmp.VoucherId = voucher.VoucherId;
                    //        }
                    //        else
                    //        {

                    //            return new BusinessResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                    //        }
                    //        orderTmp.TotalAmount = totalAmount;
                    //        orderTmp.Quantity = totalQuantity;
                    //    }
                    //}
                    #endregion
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
                    var newOrder = new Order();
                    newOrder.OrderId = $"ORDER{(await _unitOfWork.OrderRepository.Count() + 1).ToString("D4")}";
                    newOrder.CreatedDate = DateTime.Now;
                    newOrder.CreatedBy = "User";
                    newOrder.UserId = order.UserId;
                    newOrder.TotalWeight = order.TotalWeight;
                    newOrder.PaymentMethod = order.PaymentMethod;
                    newOrder.ShippingAddress = order.ShippingAddress;
                    newOrder.DeliveryDate = order.DeliveryDate;
                    newOrder.ReceiveDate = order.ReceiveDate;
                    newOrder.Note = order.Note;
                    newOrder.Status = order.Status;
                    var totalAmount = 0.0;
                    var totalQuantity = 0;
                    newOrder.OrderDetails = order.OrderDetails;
                    foreach (var item in newOrder.OrderDetails)
                    {
                        var koi = _unitOfWork.KoiFishRepository.Get(k => k.KoiId == item.KoiId);
                        item.OrderId = newOrder.OrderId;
                        item.KoiId = item.KoiId;
                        item.Price = koi.Price;
                        item.Quantity = item.Quantity;
                        totalAmount += item.Price * item.Quantity;
                        totalQuantity += item.Quantity;
                        koi.Quantity -= item.Quantity; // reduce koifish quantity according to amount of orderdetail quantity

                        if (koi.Quantity < 0)
                        {
                            return new BusinessResult(Const.FAIL_CREATE_CODE, "Koi fish out of stock");
                        }
                        _unitOfWork.KoiFishRepository.Update(koi);
                    }
                    var voucher = _unitOfWork.VoucherRepository.Get(v => v.VoucherCode == order.VoucherCode);
                    if (voucher == null)
                    {
                        newOrder.TotalAmount = totalAmount;
                        newOrder.Quantity = totalQuantity;
                    }
                    if (voucher != null && totalAmount >= voucher.MinOrderAmount && voucher.Status == 1 &&
                        voucher.ValidityEndDate >= DateTime.Now && voucher.ValidityStartDate <= DateTime.Now && voucher.Quantity >= 1)
                    {
                        totalAmount = totalAmount - (totalAmount * voucher.DiscountAmount) / 100;
                        newOrder.VoucherId = voucher.VoucherId;
                        voucher.Quantity = voucher.Quantity - 1;
                        newOrder.TotalAmount = totalAmount;
                        newOrder.Quantity = totalQuantity;

                        if (voucher.Quantity < 0)
                        {
                            return new BusinessResult(Const.FAIL_CREATE_CODE, "Voucher out of stock");
                        }
                        _unitOfWork.VoucherRepository.Update(voucher);
                    }
                    

                    result = await _unitOfWork.OrderRepository.CreateAsync(newOrder);

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
