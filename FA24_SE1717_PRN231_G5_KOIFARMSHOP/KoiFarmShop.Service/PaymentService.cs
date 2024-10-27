using KoiFarmShop.Common;
using KoiFarmShop.Data;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Request;
using KoiFarmShop.Service.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KoiFarmShop.Service
{
    public interface IPaymentService
    {
        Task<IBusinessResult> Create(CreatePaymentRequest paymentRequest);
        Task<IBusinessResult> GetAll();
        Task<IBusinessResult> Update(UpdatePaymentRequest payment);
        Task<IBusinessResult> DeleteById(string paymentId);

    }

    public class PaymentService : IPaymentService
    {
        private readonly UnitOfWork _unitOfWork;

        public PaymentService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IBusinessResult> Create(CreatePaymentRequest createPaymentRequest)
        {
            try
            {
                string userId = string.Empty;
                double amount = 0;
                string paymentId;
                string orderId = null;
                string consignmentId = null;

                if (createPaymentRequest.Type == 1) // Type 1: Order
                {
                    var order =  _unitOfWork.OrderRepository.Get(c => c.OrderId == createPaymentRequest.OrderId);
                    if (order == null)
                    {
                        return new BusinessResult(Const.WARNING_NO_DATA_CODE, "Order not found.");
                    }
                    paymentId = $"PAYMENT{(await _unitOfWork.PaymentRepository.Count() + 1).ToString("D4")}";
                    userId = order.UserId;
                    amount = order.TotalAmount;
                    orderId = order.OrderId;
                    var payment = new Payment
                    {
                        PaymentId = paymentId,
                        UserId = userId,
                        Amount = amount,
                        ConsignmentId = null,
                        OrderId = orderId,
                        Type = createPaymentRequest.Type,
                        Status = createPaymentRequest.Status,
                        Currency = createPaymentRequest.Currency,
                        PaymentMethod = createPaymentRequest.PaymentMethod,
                        Refundable = createPaymentRequest.Refundable,
                        Note = createPaymentRequest.Note,
                        CreatedDate = createPaymentRequest.CreatedDate
                    };
                    _unitOfWork.PaymentRepository.PrepareCreate(payment); 
                    await _unitOfWork.PaymentRepository.SaveAsync();

                    order.PaymentId = paymentId;
                    await _unitOfWork.OrderRepository.UpdateAsync(order);
                    return new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, payment);
                }
                else if (createPaymentRequest.Type == 2) // Type 2: Consignment
                {
                    if (string.IsNullOrEmpty(createPaymentRequest.ConsignmentId))
                    {
                        return new BusinessResult(Const.FAIL_CREATE_CODE, "Type 2 requires ConsignmentId.");
                    }

                    var consignment = _unitOfWork.ConsignmentRepository.Get(c => c.ConsignmentId == createPaymentRequest.ConsignmentId);

                    if (consignment == null)
                    {
                        return new BusinessResult(Const.WARNING_NO_DATA_CODE, "Consignment not found.");
                    }

                    userId = consignment.UserId;
                    amount = consignment.DealPrice ?? 0;
                    consignmentId = consignment.ConsignmentId;
                    paymentId = $"PAYMENT{(await _unitOfWork.PaymentRepository.Count() + 1).ToString("D4")}";
                   
                    var payment = new Payment
                    {
                        PaymentId = paymentId,
                        UserId = userId,
                        Amount = amount,
                        ConsignmentId = consignmentId,
                        OrderId = null,
                        Type = createPaymentRequest.Type,
                        Status = createPaymentRequest.Status,
                        Currency = createPaymentRequest.Currency,
                        PaymentMethod = createPaymentRequest.PaymentMethod,
                        Refundable = createPaymentRequest.Refundable,
                        Note = createPaymentRequest.Note,
                        CreatedDate = createPaymentRequest.CreatedDate
                    };
                    _unitOfWork.PaymentRepository.PrepareCreate(payment);
                    await _unitOfWork.PaymentRepository.UpdateAsync(payment);

                    consignment.PaymentId = paymentId;
                    await _unitOfWork.ConsignmentRepository.UpdateAsync(consignment);
                    return new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, payment);
                }
                else
                {
                    throw new Exception("Type không hợp lệ.");
                }       
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có ngoại lệ
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> GetAll()
        {
            try
            {
                var payments = await _unitOfWork.PaymentRepository.GetAllAsync();
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, payments);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> Update(UpdatePaymentRequest payment)
        {
            try
            {
                var paymentEntity =  _unitOfWork.PaymentRepository.Get(p => p.PaymentId == payment.PaymentId);

                if (paymentEntity == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, null);
                }

                paymentEntity.Status = payment.Status;
                paymentEntity.Amount = payment.Amount;
                paymentEntity.PaymentMethod = payment.PaymentMethod;
                paymentEntity.Currency = payment.Currency;
                paymentEntity.Note = payment.Note;
                paymentEntity.Refundable = payment.Refundable;
                paymentEntity.CreatedDate = payment.CreatedDate;

                var result = await _unitOfWork.PaymentRepository.UpdateAsync(paymentEntity);

                return new BusinessResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, paymentEntity);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> DeleteById(string paymentId)
        {
            try
            {
                var payment =  _unitOfWork.PaymentRepository.Get(c => c.PaymentId == paymentId);
                if (payment == null)
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new Consignment());
                else
                {
                    var result = await _unitOfWork.PaymentRepository.RemoveAsync(payment);
                    if (result)
                        return new BusinessResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG, payment);
                    else
                        return new BusinessResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG, payment);
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

    }
}
