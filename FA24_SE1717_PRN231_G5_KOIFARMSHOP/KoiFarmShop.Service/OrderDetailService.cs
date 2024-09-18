using KoiFarmShop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Service
{
    public interface IOrderDetailService
    {

    }

    public class OrderDetailService : IOrderDetailService
    {
        private readonly UnitOfWork _unitOfWork;
        public OrderDetailService()
        {
            _unitOfWork ??= new UnitOfWork();
        }
    }
}
