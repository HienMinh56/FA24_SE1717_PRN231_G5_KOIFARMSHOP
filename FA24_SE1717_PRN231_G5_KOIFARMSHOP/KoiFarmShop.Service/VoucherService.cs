using KoiFarmShop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Service
{
    public interface IVoucherService
    {

    }

    public class VoucherService : IVoucherService
    {
        private readonly UnitOfWork _unitOfWork;
        public VoucherService()
        {
            _unitOfWork ??= new UnitOfWork();
        }
    }
}
