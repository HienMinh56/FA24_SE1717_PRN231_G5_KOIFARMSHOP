using KoiFarmShop.Common;
using KoiFarmShop.Data;
using KoiFarmShop.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Service
{
    public interface IOrderDetailService
    {
        Task<IBusinessResult> GetOrderDetailsByOrderId(string orderId);
    }

    public class OrderDetailService : IOrderDetailService
    {
        private readonly UnitOfWork _unitOfWork;
        public OrderDetailService()
        {
            _unitOfWork ??= new UnitOfWork();
        }

        public async Task<IBusinessResult> GetOrderDetailsByOrderId(string orderId)
        {
            try
            {
                var orderDetails = _unitOfWork.OrderDetailRepository.GetList(o => o.OrderId == orderId);
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, orderDetails);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có ngoại lệ
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
