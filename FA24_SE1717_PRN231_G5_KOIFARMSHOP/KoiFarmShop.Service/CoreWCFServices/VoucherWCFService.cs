using CoreWCF;
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

namespace KoiFarmShop.Service.CoreWCFServices
{
    [ServiceContract]
    public interface IVoucherWCFService
    {
        [OperationContract]
        Task<IBusinessResult> GetAll();
        
        [OperationContract]
        Task<IBusinessResult> GetById(string Id);

        [OperationContract]
        Task<IBusinessResult> Save(CreateVoucherRequest voucher);

        [OperationContract]
        Task<IBusinessResult> DeleteById(string Id);


    }

    public class VoucherWCFService : IVoucherWCFService
    {
        private readonly UnitOfWork _unitOfWork;
        public VoucherWCFService()
        {
            _unitOfWork ??= new UnitOfWork();
        }
        public async Task<IBusinessResult> GetAll()
        {
            #region Business rule

            #endregion

            var vouchers = await _unitOfWork.VoucherRepository.GetAllAsync();

            if (vouchers == null)
            {
                return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new List<Voucher>());
            }
            else
            {
                var simplifiedVouchers = vouchers
                    .Select(v => new {
                        VoucherId = v.VoucherId,
                        VoucherCode = v.VoucherCode,
                        DiscountAmount = v.DiscountAmount,
                        MinOrderAmount = v.MinOrderAmount,
                        ValidityStartDate = v.ValidityStartDate,
                        ValidityEndDate = v.ValidityEndDate,
                        Status = v.Status,
                        CreatedDate = DateTime.Now,
                        CreatedBy = v.CreatedBy,
                        ModifiedDate = DateTime.Now,
                        ModifiedBy = v.ModifiedBy,

                    })
                    .ToList();
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, simplifiedVouchers);
            }
        }

        public async Task<IBusinessResult> GetById(string voucherId)
        {
            #region Business rule

            #endregion

            var voucher = _unitOfWork.VoucherRepository.Get(u => u.VoucherId == voucherId);

            if (voucher == null)
            {
                return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new User());
            }
            else
            {
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, voucher);
            }
        }

        public async Task<IBusinessResult> Save(CreateVoucherRequest voucher)
        {
            #region Business rule

            #endregion

            try
            {
                int result = -1;

                var voucherTmp = _unitOfWork.VoucherRepository.Get(u => u.VoucherId == voucher.VoucherId);

                if (voucherTmp != null)
                {
                    #region Business Rule

                    #endregion
                    voucherTmp.VoucherCode = voucher.VoucherCode;
                    voucherTmp.VoucherId = voucher.VoucherId;

                    voucherTmp.CreatedDate = voucher.CreatedDate;
                    voucherTmp.CreatedBy = voucher.CreatedBy;
                    voucherTmp.DiscountAmount = voucher.DiscountAmount;
                    voucherTmp.MinOrderAmount = voucher.MinOrderAmount;
                    voucherTmp.ModifiedBy = voucher.ModifiedBy;
                    voucherTmp.ModifiedDate = voucher.ModifiedDate;
                    voucherTmp.Status = voucher.Status;
                    voucherTmp.ValidityEndDate = voucher.ValidityEndDate;
                    voucherTmp.ValidityStartDate = voucher.ValidityStartDate;

                    result = await _unitOfWork.VoucherRepository.UpdateAsync(voucherTmp);

                    if (result > 0)
                    {
                        return new BusinessResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG);
                    }
                    else
                    {
                        return new BusinessResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                    }
                }
                else
                {
                    result = await _unitOfWork.VoucherRepository.CreateAsync(new Voucher
                    {
                        VoucherId = voucher.VoucherId,
                        VoucherCode = voucher.VoucherCode,
                        DiscountAmount = voucher.DiscountAmount,
                        MinOrderAmount = voucher.MinOrderAmount,
                        Status = voucher.Status,
                        ValidityStartDate = voucher.ValidityStartDate,
                        ValidityEndDate = voucher.ValidityEndDate,
                        CreatedDate = voucher.CreatedDate,
                        CreatedBy = voucher.CreatedBy,
                        ModifiedDate = voucher.ModifiedDate,
                        ModifiedBy = voucher.ModifiedBy


                    });

                    if (result > 0)
                    {
                        return new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG);
                    }
                    else
                    {
                        return new BusinessResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
                    }
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> DeleteById(string voucherId)
        {
            #region Business rule

            #endregion

            try
            {
                var voucher = _unitOfWork.VoucherRepository.Get(u => u.VoucherId == voucherId);

                if (voucher == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new User());
                }
                else
                {
                    var result = await _unitOfWork.VoucherRepository.RemoveAsync(voucher);

                    if (result)
                    {
                        return new BusinessResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG, voucher);
                    }
                    else
                    {
                        return new BusinessResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG, voucher);
                    }
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
