using KoiFarmShop.Common;
using KoiFarmShop.Data;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Request;
using KoiFarmShop.Data.Request.KoiFarmShop.Data.Request;
using KoiFarmShop.Service.Base;
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
        Task<IBusinessResult> GetById(string paymentId);
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

        public async Task<IBusinessResult> GetById(string paymentId)
        {
            try
            {
                var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(paymentId);
                if (payment == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, "Không tìm thấy Payment", null);
                }
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, payment);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
