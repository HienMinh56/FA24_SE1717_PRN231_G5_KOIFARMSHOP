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
    public interface IKoiFishService
    {
        Task<IBusinessResult> GetAll();
        Task<IBusinessResult> GetById(string code);
        Task<IBusinessResult> Create(CreateKoiFishRequest koiFish);
        Task<IBusinessResult> Update(KoiFish koiFish);
        Task<IBusinessResult> Delete(string code);
        Task<IBusinessResult> Save(KoiFish koiFish);
        Task<IBusinessResult> DeleteById(string code);
        
    }
    public class KoiFishService : IKoiFishService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IFirebaseStorageService _firebaseStorageService;
        public KoiFishService(IFirebaseStorageService firebaseStorageService)
        {
            _unitOfWork ??= new UnitOfWork();
            _firebaseStorageService = firebaseStorageService;
        }

        public async Task<IBusinessResult> Create(CreateKoiFishRequest KoiFish)
        {
            #region Business rule

            #endregion
            if (KoiFish.Image is null)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, "You need attached an image when create a koifish");
            }

//            var KoiId = (await _unitOfWork.KoiFishRepository.Count() + 1).ToString("D3");
            var KoiId = $"K{(await _unitOfWork.KoiFishRepository.Count() + 1).ToString("D3")}";
            await _unitOfWork.KoiFishRepository.CreateAsync(new KoiFish
            {
                KoiId = KoiId,
                KoiName = KoiFish.KoiName,
                Origin = KoiFish.Origin,
                Gender = KoiFish.Gender, // Male or Female
                Age = KoiFish.Age,
                Size = KoiFish.Size,
                Breed = KoiFish.Breed,
                Type = KoiFish.Type, // 'Imported Purebred', 'F1', 'Vietnamese Purebred'
                Price = KoiFish.Price,
                Quantity = KoiFish.Quantity,
                OwnerType = KoiFish.OwnerType,
                Description = KoiFish.Description,
                CreatedDate = DateTime.Now,
                CreatedBy = "Admin" // fix cung truoc, sua lai sau khi them authen
            });

            var result = _unitOfWork.KoiFishRepository.Get(k => k.KoiId == KoiId);
            string ImageId;
            foreach (var image in KoiFish.Image)
            {
                ImageId = $"I{(await _unitOfWork.ImageRepository.Count() + 1).ToString("D3")}";
                await _unitOfWork.ImageRepository.CreateAsync(new Image
                {
                    ImageId = ImageId,
                    KoiId = KoiId,
                    Url = await _firebaseStorageService.UploadImageAsync(image),
                    CreatedDate = DateTime.Now,
                    CreatedBy = "Admin"
                });
                result.Images.Add(_unitOfWork.ImageRepository.Get(i => i.ImageId == ImageId));
            }

            if (result is not null && result.Images is not null)
            {
                return new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, result);
            }
            else
            {
                return new BusinessResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG, new KoiFish());
            }
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

        public async Task<IBusinessResult> Update(KoiFish KoiFish)
        {
            #region Business rule

            #endregion
            var koiFishs = _unitOfWork.KoiFishRepository.Get(k => k.KoiId == KoiFish.KoiId);

            if (koiFishs == null)
            {
                return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new KoiFish());
            }
            else
            {
                var result = await _unitOfWork.KoiFishRepository.UpdateAsync(KoiFish);
                if (result > 0)
                {
                    return new BusinessResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, KoiFish);
                }
                else
                {
                    return new BusinessResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG, new KoiFish());
                }
            }
        }
    }
}
