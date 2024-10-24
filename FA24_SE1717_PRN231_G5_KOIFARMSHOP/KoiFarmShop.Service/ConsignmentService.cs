using Google.Apis.Storage.v1.Data;
using KoiFarmShop.Common;
using KoiFarmShop.Data;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Request;
using KoiFarmShop.Service.Base;
using Microsoft.EntityFrameworkCore;
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
        Task<IBusinessResult> SaveConsignment(object requestModel);
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
                var consignment = _unitOfWork.ConsignmentRepository.Get(o => o.ConsignmentId == consignmentId);
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
            if (consignment is null)
            {
                return new BusinessResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
            }

            try
            {
                var totalConsignments = await _unitOfWork.ConsignmentRepository.Count();
                var ConsignmentId = $"CONSIGN{(totalConsignments + 1).ToString("D4")}";

                var consignmentTmp = new Consignment
                {
                    ConsignmentId = ConsignmentId,
                    UserId = consignment.UserId,
                    KoiId = consignment.KoiId,
                    Type = consignment.Type, // 1: Care, 2: Sale
                    Method = consignment.Method,
                    DealPrice = consignment.DealPrice,
                    Status = 1, // 1:Pending, 2:Agreed, 3: In store, 4:Sold, 5:Return, 6:Cancel
                    ConsignmentDate = consignment.ConsignmentDate,
                    CreatedDate = DateTime.Now,
                    CreatedBy = consignment.UserId
                };

                // Thêm Consignment vào context
                int result = await _unitOfWork.ConsignmentRepository.CreateAsync(consignmentTmp);

                if (result > 0)
                {
                    return new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, consignmentTmp);
                }
                else
                {
                    return new BusinessResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
                }
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có ngoại lệ
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> Update(UpdateConsignmentRequest consignment)
        {
            if (consignment is null)
            {
                return new BusinessResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }

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
                consignmentTmp.ModifiedDate = DateTime.Now;
                consignmentTmp.ModifiedBy = consignment.UserId;

                int result = await _unitOfWork.ConsignmentRepository.UpdateAsync(consignmentTmp);

                if (result > 0)
                {
                    return new BusinessResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, consignmentTmp);
                }
                else
                {
                    return new BusinessResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                }
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có ngoại lệ
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
        public async Task<IBusinessResult> SaveConsignment(object requestModel)
        {
            try
            {
                int result = -1;

                if (requestModel is UpdateConsignmentRequest updateRequest)
                {
                    // Tìm consignment dựa trên ConsignmentId
                    var consignmentTmp = _unitOfWork.ConsignmentRepository.Get(c => c.ConsignmentId == updateRequest.ConsignmentId);

                    if (consignmentTmp != null)
                    {
                        // Cập nhật các trường của consignment
                        consignmentTmp.UserId = updateRequest.UserId;
                        consignmentTmp.KoiId = updateRequest.KoiId;
                        consignmentTmp.Type = updateRequest.Type;
                        consignmentTmp.DealPrice = updateRequest.DealPrice;
                        consignmentTmp.Method = updateRequest.Method;
                        consignmentTmp.Status = updateRequest.Status;
                        consignmentTmp.Note = updateRequest.Note;
                        consignmentTmp.CustomerContact = updateRequest.CustomerContact;
                        consignmentTmp.CustomerAddress = updateRequest.CustomerAddress;
                        consignmentTmp.TotalWeight = updateRequest.TotalWeight;
                        consignmentTmp.ModifiedDate = DateTime.Now;
                        consignmentTmp.ModifiedBy = updateRequest.UserId;

                        // Lưu thay đổi
                        result = await _unitOfWork.ConsignmentRepository.UpdateAsync(consignmentTmp);

                        if (result > 0)
                        {
                            return new BusinessResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, consignmentTmp);
                        }
                        else
                        {
                            return new BusinessResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                        }
                    }
                    else
                    {
                        return new BusinessResult(Const.FAIL_UPDATE_CODE, "Consignment not found.");
                    }
                }
                else if (requestModel is CreateConsignmentRequest createRequest)
                {
                    // Đếm số lượng consignment hiện có để tạo ConsignmentId mới
                    var totalConsignments = await _unitOfWork.ConsignmentRepository.Count();
                    var ConsignmentId = $"CONSIGN{(totalConsignments + 1).ToString("D4")}";

                    // Tạo mới consignment từ createRequest
                    var newConsignment = new Consignment
                    {
                        ConsignmentId = ConsignmentId,
                        UserId = createRequest.UserId,
                        KoiId = createRequest.KoiId,
                        Type = createRequest.Type,
                        DealPrice = createRequest.DealPrice,
                        Method = createRequest.Method,
                        Note = createRequest.Note,
                        CustomerContact = createRequest.CustomerContact,
                        CustomerAddress = createRequest.CustomerAddress,
                        TotalWeight = createRequest.TotalWeight,
                        ConsignmentDate = createRequest.ConsignmentDate,
                        Status = 1,
                        CreatedDate = DateTime.Now,
                        CreatedBy = createRequest.UserId
                    };

                    // Lưu consignment mới vào database
                    result = await _unitOfWork.ConsignmentRepository.CreateAsync(newConsignment);

                    if (result > 0)
                    {
                        return new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, newConsignment);
                    }
                    else
                    {
                        return new BusinessResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
                    }
                }
                else
                {
                    return new BusinessResult(Const.ERROR_EXCEPTION, "Invalid model type.");
                }
            }
            catch (Exception ex)
            {
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