using Google.Apis.Storage.v1.Data;
using KoiFarmShop.Common;
using KoiFarmShop.Data;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Request;
using KoiFarmShop.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Service
{
    public interface IConsignmentService
    {
        Task<IBusinessResult> GetAll();
        Task<IBusinessResult> GetById(string code);
        Task<IBusinessResult> Create(CreateConsignmentRequest consignment);
        Task<IBusinessResult> Update(UpdateConsignmentRequest consignment);
        Task<IBusinessResult> DeleteById(string consignmentId);
    }

    public class ConsignmentService : IConsignmentService
    {
        private readonly UnitOfWork _unitOfWork;

        public ConsignmentService()
        {
            _unitOfWork ??= new UnitOfWork();
        }

        public async Task<IBusinessResult> GetAll()
        {
            #region Business Rule

            #endregion

            try
            {
                var consignments = await _unitOfWork.ConsignmentRepository.GetAllAsync();
                if (consignments == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new List<Consignment>());
                }
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, consignments);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }


        }

        public async Task<IBusinessResult> GetById(string consignmentId)
        {
            #region Business Rule

            #endregion

            try
            {
                var consignment = await _unitOfWork.ConsignmentRepository.GetConsignmentByIdAsync(consignmentId);
                if (consignment == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new Consignment());
                }
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, consignment);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> Create(CreateConsignmentRequest consignment)
        {
            try
            {
                var consignmentTmp = await _unitOfWork.ConsignmentRepository.CreateConsignmentAsync(consignment);
                return new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, consignmentTmp);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có ngoại lệ
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> Update(UpdateConsignmentRequest consignment)
        {
            try
            {
                var consignmentTmp = _unitOfWork.ConsignmentRepository.Get(c => c.ConsignmentId == consignment.ConsignmentId);

                if (consignmentTmp == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, null);
                }
                consignmentTmp.UserId = consignment.UserId;
                consignmentTmp.KoiId = consignment.KoiId;
                consignmentTmp.Type = consignment.Type;
                consignmentTmp.DealPrice = consignment.DealPrice;
                consignmentTmp.Method = consignment.Method;
                consignmentTmp.Status = consignment.Status;
                consignmentTmp.ModifiedDate = consignment.ModifiedDate;
                consignmentTmp.ModifiedBy = consignment.ModifiedBy;

                var result = await _unitOfWork.ConsignmentRepository.UpdateAsync(consignmentTmp);

                return new BusinessResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, consignmentTmp);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có ngoại lệ
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> DeleteById(string consignmentId)
        {
            try
            {
                var consignment = _unitOfWork.ConsignmentRepository.Get(c => c.ConsignmentId == consignmentId);
                if (consignment == null)
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new Consignment());
                else
                {                    
                    var result = await _unitOfWork.ConsignmentRepository.RemoveAsync(consignment);
                    if (result)
                        return new BusinessResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG, consignment);
                    else
                        return new BusinessResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG, consignment);
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}