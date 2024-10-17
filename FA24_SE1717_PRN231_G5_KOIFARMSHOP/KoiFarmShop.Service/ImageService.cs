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
    public interface IImageService
    {
        Task<IBusinessResult> GetImages();
        Task<IBusinessResult> GetImageById(string ImageId);
        Task<IBusinessResult> Save(ImageRequest image);
        Task<IBusinessResult> Delete(string imageId);

    }

    public class ImageService : IImageService
    {
        private readonly UnitOfWork _unitOfWork;
        public ImageService()
        {
            _unitOfWork ??= new UnitOfWork();
        }


        public async Task<IBusinessResult> GetImages()
        {
            try
            {
                var Images = await _unitOfWork.ImageRepository.GetAllAsync();
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, Images);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có ngoại lệ
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> GetImageById(string ImageId)
        {
            try
            {
                var Image = _unitOfWork.ImageRepository.Get(o => o.ImageId == ImageId);
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, Image);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có ngoại lệ
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
        public async Task<IBusinessResult> Save(ImageRequest Image)
        {
            try
            {
                int result = -1;

                var ImageTmp = Image.ImageId is null 
                    ? null
                    : _unitOfWork.ImageRepository.Get(k => k.ImageId == Image.ImageId);

                if (ImageTmp != null)
                {
                    #region Business Rule
                    #endregion
                    ImageTmp.KoiId = Image.KoiId;
                    ImageTmp.Url = Image.ImageURL;
                    ImageTmp.CreatedDate = Image.CreatedDate;
                    ImageTmp.ModifiedBy = Image.ModifiedBy;
                    ImageTmp.CreatedBy = Image.CreatedBy;
                    ImageTmp.DeletedBy = Image.DeletedBy;

                    result = await _unitOfWork.ImageRepository.UpdateAsync(ImageTmp);

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

                    result = await _unitOfWork.ImageRepository.CreateAsync(new Image
                    {
                        ImageId = Guid.NewGuid().ToString().Substring(2, 5),
                        KoiId = Image.KoiId,
                        Url = Image.ImageURL,
                        CreatedDate = Image.CreatedDate,
                        ModifiedBy = Image.ModifiedBy,
                        CreatedBy = Image.CreatedBy,
                        DeletedBy = Image.DeletedBy,
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

        public async Task<IBusinessResult> Delete(string imageId)
        {
            try
            {
                var Image = _unitOfWork.ImageRepository.Get(k => k.ImageId == imageId);
                if (Image == null)
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new Image());
                else
                {
                    var result = await _unitOfWork.ImageRepository.RemoveAsync(Image);
                    if (result)
                        return new BusinessResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG, Image);
                    else
                        return new BusinessResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG, Image);
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
