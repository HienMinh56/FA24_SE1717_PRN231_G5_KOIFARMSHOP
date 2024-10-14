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
        // Các phương thức khác nếu cần
    }

    public class PaymentService : IPaymentService
    {
        private readonly UnitOfWork _unitOfWork;

        public PaymentService()
        {
            _unitOfWork = new UnitOfWork();
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
    }
}
