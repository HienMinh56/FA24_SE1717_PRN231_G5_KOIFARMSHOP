using KoiFarmShop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Service
{
    public interface IPaymentService
    {

    }

    public class PaymentService : IPaymentService
    {
        private readonly UnitOfWork _unitOfWork;
        public PaymentService()
        {
            _unitOfWork ??= new UnitOfWork();
        }
    }
}
