using KoiFarmShop.Common;
using KoiFarmShop.Data;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Request;
using KoiFarmShop.Service.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        Task<IBusinessResult> Update(UpdateKoiFishRequest koiFish);
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

        public async Task<IBusinessResult> Create(CreateKoiFishRequest createKoiFishRequest)
        {
            #region Business rule
            //1.create a new koifish entity and add it into database
            //2.check if new koifish was create. if wasn't, return failed message.
            //3.create new image entities with koiId of koifish created as foreign key
            //4.check if new image entity was created. if wasn't, remove koifish entity created from DB and return failed message.
            #endregion
            if (createKoiFishRequest.Image is null)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, "You need attached an image when create a koifish");
            }

            var koiFishs = await _unitOfWork.KoiFishRepository.GetAllOrderedByKoiId();
            var Id = koiFishs.Count + 1;
            if (_unitOfWork.KoiFishRepository.Get(k => k.KoiId == $"{Const.KOIFISH}{Id.ToString("D4")}") is not null)
            {
                Id = await _unitOfWork.KoiFishRepository.FindEmptyPositionWithBinarySearch(koiFishs, 1, Id, Const.KOIFISH, Const.KOIFISH_INDEX);
            }

            var KoiId = $"{Const.KOIFISH}{Id.ToString("D4")}";
            await _unitOfWork.KoiFishRepository.CreateAsync(new KoiFish
            {
                KoiId = KoiId,
                KoiName = createKoiFishRequest.KoiName,
                Origin = createKoiFishRequest.Origin,
                Gender = createKoiFishRequest.Gender, // Male or Female
                Age = createKoiFishRequest.Age,
                Size = createKoiFishRequest.Size,
                Breed = createKoiFishRequest.Breed,
                Type = createKoiFishRequest.Type, // 'Imported Purebred', 'F1', 'Vietnamese Purebred'
                Price = createKoiFishRequest.Price,
                Quantity = createKoiFishRequest.Quantity,
                OwnerType = createKoiFishRequest.OwnerType,
                Description = createKoiFishRequest.Description,
                CreatedDate = DateTime.Now,
                CreatedBy = "Admin" // fix cung truoc, se sua lai sau khi them authen
            });

            try
            {
                string ImageId;
                List<Image> images;
                int iId;
                foreach (var image in createKoiFishRequest.Image)
                {
                    images = await _unitOfWork.ImageRepository.GetAllOrderByImageId();
                    iId = images.Count + 1;
                    if (_unitOfWork.ImageRepository.Get(i => i.ImageId == $"{Const.IMAGE}{iId.ToString("D4")}") is not null)
                    {
                        iId = await _unitOfWork.ImageRepository.FindEmptyPositionWithBinarySearch(images, 1, iId, Const.IMAGE, Const.IMAGE_INDEX);
                    }

                    ImageId = $"{Const.IMAGE}{iId.ToString("D4")}";
                    await _unitOfWork.ImageRepository.CreateAsync(new Image
                    {
                        ImageId = ImageId,
                        KoiId = KoiId,
                        Url = await _firebaseStorageService.UploadImageAsync(image),
                        CreatedDate = DateTime.Now,
                        CreatedBy = "Admin"
                    });
                }
            }
            catch (Exception ex)
            {
                await DeleteById(KoiId);
                return new BusinessResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG, ex.Message);
            }
            

            var result = await _unitOfWork.KoiFishRepository.GetByIdWithImages(KoiId);
            if (result is null)
            {
                return new BusinessResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG, new KoiFish());
            }

            if (result.Images is not null)
            {
                return new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, result);
            }
            else
            {
                await DeleteById(KoiId);
                return new BusinessResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG, new KoiFish());
            }
        }

        public Task<IBusinessResult> Delete(string code)
        {
            throw new NotImplementedException();
        }

        public async Task<IBusinessResult> DeleteById(string id)
        {
            try
            {
                var koifish = _unitOfWork.KoiFishRepository.Get(k => k.KoiId == id);
                var images = (await _unitOfWork.ImageRepository.GetAllOrderByImageId()).Where(i => i.KoiId == id).ToList();
                if (koifish == null)
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new KoiFish());
                else
                {
                    foreach (var image in images)
                    {
                        await _firebaseStorageService.DeleteImageAsync(_firebaseStorageService.ExtractImageNameFromUrl(image.Url));
                        await _unitOfWork.ImageRepository.RemoveAsync(image);
                    }
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
                var koifishes = await _unitOfWork.KoiFishRepository.GetAllWithImages();
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
                var koifish = await _unitOfWork.KoiFishRepository.GetByIdWithImages(code);
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

                var koiFishTmp = _unitOfWork.KoiFishRepository.Get(k => k.KoiId == koiFish.KoiId);

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

        public async Task<IBusinessResult> Update(UpdateKoiFishRequest updateKoiFishRequest)
        {
            #region Business rule
            //1. check if koifish that is gonna updated is already existed in DB
            //2. if not, return failed message: the koifish doesn't exist.
            //3. else, get the koifish entity, update inputted properties, and save the changes
            //4. check if there are any images user want to delete, if there are, delete these images from DB and firebase
            //5. check if there are any images user want to add, if there are, add these images from DB and firebase.
            //6. save these changes. if success, return success message, else, return fail message
            #endregion

            var koiFish = await _unitOfWork.KoiFishRepository.GetByIdWithDetail(updateKoiFishRequest.KoiId);
            if (koiFish is null)
            {
                return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new KoiFish());
            }
            else
            {
                koiFish.KoiName = updateKoiFishRequest.KoiName is null ? koiFish.KoiName : updateKoiFishRequest.KoiName;
                koiFish.Origin = updateKoiFishRequest.Origin is null ? koiFish.Origin : updateKoiFishRequest.Origin;
                koiFish.Gender = updateKoiFishRequest.Gender is null ? koiFish.Gender : updateKoiFishRequest.Gender; // Male or Female
                koiFish.Age = updateKoiFishRequest.Age == 0 ? koiFish.Age : updateKoiFishRequest.Age;
                koiFish.Size = updateKoiFishRequest.Size == 0 ? koiFish.Size : updateKoiFishRequest.Size;
                koiFish.Breed = updateKoiFishRequest.Breed is null ? koiFish.Breed : updateKoiFishRequest.Breed;
                koiFish.Type = updateKoiFishRequest.Type is null ? koiFish.Type : updateKoiFishRequest.Type; // 'Imported Purebred', 'F1', 'Vietnamese Purebred'
                koiFish.Price = updateKoiFishRequest.Price == 0 ? koiFish.Price : updateKoiFishRequest.Price;
                koiFish.Quantity = updateKoiFishRequest.Quantity == 0 ? koiFish.Quantity : updateKoiFishRequest.Quantity;
                koiFish.OwnerType = updateKoiFishRequest.OwnerType == 0 ? koiFish.OwnerType : updateKoiFishRequest.OwnerType;
                koiFish.Description = updateKoiFishRequest.Description is null ? koiFish.Description : updateKoiFishRequest.Description;
                koiFish.ModifiedDate = DateTime.UtcNow;
                koiFish.ModifiedBy = "Admin"; // fix cung truoc, se sua lai sau khi them authen
            }

            if (updateKoiFishRequest.RemovedImage is not [])
            {
                Image? image;
                foreach (var url in updateKoiFishRequest.RemovedImage)
                {
                    if ((image = await _unitOfWork.ImageRepository.GetImageByUrl(url)) is not null)
                    {
                        await _firebaseStorageService.DeleteImageAsync(_firebaseStorageService.ExtractImageNameFromUrl(url));
                        await _unitOfWork.ImageRepository.RemoveAsync(image);
                    }
                }
            }
            if (updateKoiFishRequest.AddedImage is not null)
            {
                string ImageId;
                List<Image> images;
                int id;
                foreach (var image in updateKoiFishRequest.AddedImage)
                {
                    images = await _unitOfWork.ImageRepository.GetAllOrderByImageId();
                    id = images.Count + 1;
                    if (_unitOfWork.ImageRepository.Get(i => i.ImageId == $"{Const.IMAGE}{id.ToString("D4")}") is not null)
                    {
                        id = await _unitOfWork.ImageRepository.FindEmptyPositionWithBinarySearch(images, 1, id, Const.IMAGE, Const.IMAGE_INDEX);
                    }

                    ImageId = $"{Const.IMAGE}{id.ToString("D4")}";
                    await _unitOfWork.ImageRepository.CreateAsync(new Image
                    {
                        ImageId = ImageId,
                        KoiId = updateKoiFishRequest.KoiId,
                        Url = await _firebaseStorageService.UploadImageAsync(image),
                        CreatedDate = DateTime.Now,
                        CreatedBy = "Admin"
                    });
                }
            }

            if (await _unitOfWork.KoiFishRepository.UpdateAsync(koiFish) > 0)
            {
                return new BusinessResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, koiFish);
            }
            else
            {
                return new BusinessResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG, new KoiFish());
            }
        }
/*
        public bool CheckEmpty(string code)
        {
            if (_unitOfWork.KoiFishRepository.Get(k => k.KoiId == code) is not null)
            {
                return false;   
            }
            return true;

        }
        public async Task<int> FindEmptyPositionWithBinarySearch(List<KoiFish> koiFishs, int low, int high)
        {
//            var Id = $"KOI{current.ToString("D3")}";
            var mid = (low + high) / 2;
            var Id = $"KOIFISH{mid.ToString("D3")}";

            if (CheckEmpty(Id))
            {
                return mid;
            }
            else
            {
                var index = koiFishs.FindIndex(k => k.KoiId == Id) + 1;
                if (index < mid)
                {
                    return await FindEmptyPositionWithBinarySearch(koiFishs, low, mid);
                }
                else
                {
                    return await FindEmptyPositionWithBinarySearch(koiFishs, mid, high);
                }
            }
        }
*/
    }
}
