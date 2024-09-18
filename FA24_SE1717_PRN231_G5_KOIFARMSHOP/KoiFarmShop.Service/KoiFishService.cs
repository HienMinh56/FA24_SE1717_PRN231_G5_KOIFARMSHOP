using KoiFarmShop.Common;
using KoiFarmShop.Data;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiFarmShop.Service
{
    public interface IKoiFishService
    {
        Task<IBusinessResult> GetAll();
        Task<IBusinessResult> GetById(string code);
        Task<IBusinessResult> Create(KoiFish koiFish);
        Task<IBusinessResult> Update(KoiFish koiFish);
        Task<IBusinessResult> Delete(string code);
        Task<IBusinessResult> Save(KoiFish koiFish);
        Task<IBusinessResult> DeleteById(string code);
        
    }
    public class KoiFishService : IKoiFishService
    {
        private readonly UnitOfWork _unitOfWork;
        public KoiFishService()
        {
            _unitOfWork ??= new UnitOfWork();
        }

        public Task<IBusinessResult> Create(KoiFish koiFish)
        {
            throw new NotImplementedException();
        }

        public Task<IBusinessResult> Delete(string code)
        {
            throw new NotImplementedException();
        }

        public async Task<IBusinessResult> DeleteById(string code)
        {
            try
            {
                var koifish =  _unitOfWork.KoiFishRepository.Get(k => k.KoiId == code);
                if (koifish == null)
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new KoiFish());
                else
                {
                    var result = await _unitOfWork.KoiFishRepository.RemoveAsync(koifish);
                    if (result)
                        return new BusinessResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG, koifish);
                    else
                        return new BusinessResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG, koifish);
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> GetAll()
        {
            #region Business Rule

            #endregion
            try
            {
                var koifishes = await _unitOfWork.KoiFishRepository.GetAllAsync();
                if (koifishes == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new List<KoiFish>());
                }
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, koifishes);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }


        }

        public async Task<IBusinessResult> GetById(string code)
        {
            try
            {
                var koifish = await _unitOfWork.KoiFishRepository.GetByIdAsync(code);
                if (koifish == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new KoiFish());
                }
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, koifish);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> Save(KoiFish koiFish)
        {
            try
            {
                int result = -1;

                var koiFishTmp = _unitOfWork.KoiFishRepository.GetById(koiFish.KoiId);

                if (koiFishTmp != null)
                {
                    #region Business Rule
                    #endregion
                    result = await _unitOfWork.KoiFishRepository.UpdateAsync(koiFish);

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
                    #region Business Rule
                    #endregion

                    result = await _unitOfWork.KoiFishRepository.CreateAsync(koiFish);

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

        public Task<IBusinessResult> Update(KoiFish koiFish)
        {
            throw new NotImplementedException();
        }
    }
}
