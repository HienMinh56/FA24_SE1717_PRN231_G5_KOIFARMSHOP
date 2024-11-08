using KoiFarmShop.Common;
using KoiFarmShop.Data;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Request;
using KoiFarmShop.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Service
{
    public interface IVoucherService
    {
        Task<IBusinessResult> GetAll();
        Task<IBusinessResult> GetById(string Id);
        Task<IBusinessResult> Save(CreateVoucherRequest voucher);
        Task<IBusinessResult> DeleteById(string Id);
        Task<IBusinessResult> GetByCode(string code);

    }

    public class VoucherService : IVoucherService
    {
        private readonly UnitOfWork _unitOfWork;
        public VoucherService()
        {
            _unitOfWork ??= new UnitOfWork();
        }


        public async Task<IBusinessResult> GetByCode(string code)
        {
            #region Business rule

            #endregion

            var voucher = _unitOfWork.VoucherRepository.Get(u => u.VoucherCode == code);

            if (voucher == null)
            {
                return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new User());
            }
            else
            {
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, voucher);
            }
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
                        VoucherName = v.VoucherName,
                        ApplyMethod = v.ApplyMethod,
                        DiscountAmount = v.DiscountAmount,
                        MinOrderAmount = v.MinOrderAmount,
                        ValidityStartDate = v.ValidityStartDate,
                        ValidityEndDate = v.ValidityEndDate,
                        Status = v.Status,
                        Quanlity = v.Quantity,

                        CreatedDate = DateTime.Now,
                        CreatedBy = v.CreatedBy,
                        ModifiedDate = DateTime.Now,
                        ModifiedBy = v.ModifiedBy,
                        Note = v.Note
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

            //var accountId = claims.FindFirst("aid")?.Value;
            //var account = _unitOfWork.UserRepository.Get(u => u.Id == int.Parse(accountId));
            try
            {
                int result = -1;

                var voucherTmp = _unitOfWork.VoucherRepository.Get(u => u.VoucherId == voucher.VoucherId);
                var toalVoucher = await _unitOfWork.VoucherRepository.Count();
                var voucherNewId = $"Voucher{Guid.NewGuid().ToString("N").Substring(0, 6)}";
                if (voucherTmp != null)
                {
                    #region Business Rule

                    #endregion


                    if (voucherTmp.VoucherCode != null)
                    {
                        voucherTmp.VoucherCode = voucher.VoucherCode;
                    }
                    if (voucher.VoucherName != null)
                    {
                        voucherTmp.VoucherName = voucher.VoucherName;
                    }
                    if (voucher.ApplyMethod != null)
                    {
                        voucherTmp.ApplyMethod = voucher.ApplyMethod;
                    }
                    


                    if (voucher.DiscountAmount != null && voucher.DiscountAmount != 0)
                    {
                        voucherTmp.DiscountAmount = voucher.DiscountAmount;
                    }
                    if (voucher.MinOrderAmount != null && voucher.MinOrderAmount != 0)
                    {
                        voucherTmp.MinOrderAmount = voucher.MinOrderAmount;
                    }

                    voucherTmp.ModifiedDate = DateTime.Now;

                    

                    if (voucher.Status != null)
                    {
                        voucherTmp.Status = voucher.Status;
                    }
                    if (voucher.ValidityEndDate != null)
                    {
                        voucherTmp.ValidityEndDate = voucher.ValidityEndDate;
                    }
                    if (voucher.ValidityStartDate != null)
                    {
                        voucherTmp.ValidityStartDate = voucher.ValidityStartDate;
                    }
                    if (voucher.Quantity != null)
                    {
                        voucherTmp.Quantity = voucher.Quantity;
                    }
                    if (voucher.Note == null)
                    {
                        voucherTmp.Note = "";
                    }else 
                    {
                        voucherTmp.Note = voucher.Note;
                    }


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
                        VoucherId = voucherNewId,
                        VoucherCode = voucher.VoucherCode,
                        VoucherName = voucher.VoucherName ?? "",
                        DiscountAmount = voucher.DiscountAmount,
                        MinOrderAmount = voucher.MinOrderAmount,
                        Status = voucher.Status,
                        ValidityStartDate = voucher.ValidityStartDate,
                        ValidityEndDate = voucher.ValidityEndDate,
                        Quantity = voucher.Quantity,
                        
                        Note = voucher.Note ?? "",
                        ApplyMethod = voucher.ApplyMethod,

                        CreatedDate = DateTime.Now,
                        CreatedBy = "User",
                        ModifiedDate = DateTime.Now,
                        ModifiedBy = "User"


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