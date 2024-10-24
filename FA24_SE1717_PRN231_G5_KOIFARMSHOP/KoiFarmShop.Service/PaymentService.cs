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
        Task<IBusinessResult> GetPaymentById(string paymentId);
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

        public async Task<IBusinessResult> Create(CreatePaymentRequest paymentRequest)
        {
            try
            {
                var payment = await _unitOfWork.PaymentRepository.CreatePaymentAsync(paymentRequest);
                return new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, payment);
            }
            catch (Exception ex)
            {
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

        public async Task<IBusinessResult> GetPaymentById(string paymentId)
        {
            try
            {
                var payment = await _unitOfWork.PaymentRepository.GetPaymentByIdAsync(paymentId);
                if (payment == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new Payment());
                }
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, payment);
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
                var paymentEntity = await _unitOfWork.PaymentRepository.GetPaymentByIdAsync(payment.PaymentId);

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
                var payment = await _unitOfWork.PaymentRepository.GetPaymentByIdAsync(paymentId);
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
