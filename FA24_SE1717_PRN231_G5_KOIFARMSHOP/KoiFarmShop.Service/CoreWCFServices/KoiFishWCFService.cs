using CoreWCF;
using KoiFarmShop.Common;
using KoiFarmShop.Data;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Request;
using KoiFarmShop.Service.Base;
using System.Linq;

namespace KoiFarmShop.Service.CoreWCFServices
{
    [ServiceContract]
    public interface IKoiFishWCFService
    {
        [OperationContract]
        List<KoiFish> GetAll();

        [OperationContract]
        KoiFish GetById(string code);

        [OperationContract]
        KoiFish Create(CreateKoiFishRequest koiFish);

        //[OperationContract]
        //KoiFish Update(UpdateKoiFishRequest koiFish);

        [OperationContract]
        KoiFish Delete(string code);

        [OperationContract]
        KoiFish Save(KoiFish koiFish);

        [OperationContract]
        KoiFish DeleteById(string code);

        [OperationContract]
        IQueryable<KoiFish> GetAllOData();
    }

    public class KoiFishWCFService : IKoiFishWCFService
    {
        private readonly UnitOfWork _unitOfWork;
        public KoiFishWCFService()
        {
            _unitOfWork ??= new UnitOfWork();
        }

        public KoiFish Create(CreateKoiFishRequest createKoiFishRequest)
        {
            var koiFishs = _unitOfWork.KoiFishRepository.GetAll();
            var Id = koiFishs.Count + 1;

            var KoiId = $"{Const.KOIFISH}{Id.ToString("D4")}";
            _unitOfWork.KoiFishRepository.Create(new KoiFish
            {
                KoiId = KoiId,
                KoiName = createKoiFishRequest.KoiName,
                Origin = createKoiFishRequest.Origin,
                Gender = createKoiFishRequest.Gender,
                Age = createKoiFishRequest.Age,
                Size = createKoiFishRequest.Size,
                Breed = createKoiFishRequest.Breed,
                Type = createKoiFishRequest.Type,
                Price = createKoiFishRequest.Price,
                Quantity = createKoiFishRequest.Quantity,
                OwnerType = createKoiFishRequest.OwnerType,
                Description = createKoiFishRequest.Description,
                CreatedDate = DateTime.Now,
                CreatedBy = "Admin"
            });

            var result = _unitOfWork.KoiFishRepository.Get(k => k.KoiId == KoiId);
            if (result is null)
            {
                return new KoiFish();
            }
            else
            {
                return result;
            }
        }

        public KoiFish Delete(string code)
        {
            throw new NotImplementedException();
        }

        public KoiFish DeleteById(string id)
        {
            try
            {
                var koifish = _unitOfWork.KoiFishRepository.Get(k => k.KoiId == id);

                if (koifish == null)
                    return new KoiFish();
                else
                {
                    var result = _unitOfWork.KoiFishRepository.Remove(koifish);
                    return koifish;
                }
            }
            catch (Exception ex)
            {
                return new KoiFish();
            }
        }

        public List<KoiFish> GetAll()
        {
            var koifishes = _unitOfWork.KoiFishRepository.GetAll();
            return koifishes;
        }

        public KoiFish GetById(string code)
        {
            var koifish = _unitOfWork.KoiFishRepository.Get(k => k.KoiId == code);
            if (koifish == null)
            {
                return new KoiFish();
            }
            return koifish;
        }

        public KoiFish Save(KoiFish koiFish)
        {
            var koiFishTmp = _unitOfWork.KoiFishRepository.Get(k => k.KoiId == koiFish.KoiId);

            if (koiFishTmp != null)
            {
                _unitOfWork.KoiFishRepository.Update(koiFish);
                return koiFish;
            }
            else
            {
                _unitOfWork.KoiFishRepository.Create(koiFish);
                return koiFish;
            }
        }

        //public KoiFish Update(UpdateKoiFishRequest updateKoiFishRequest)
        //{
        //    var koiFish = _unitOfWork.KoiFishRepository.GetByIdWithDetail(updateKoiFishRequest.KoiId);
        //    if (koiFish is null)
        //    {
        //        return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new KoiFish());
        //    }
        //    else
        //    {
        //        koiFish.KoiName = updateKoiFishRequest.KoiName ?? koiFish.KoiName;
        //        koiFish.Origin = updateKoiFishRequest.Origin ?? koiFish.Origin;
        //        koiFish.Gender = updateKoiFishRequest.Gender ?? koiFish.Gender;
        //        koiFish.Age = updateKoiFishRequest.Age == 0 ? koiFish.Age : updateKoiFishRequest.Age;
        //        koiFish.Size = updateKoiFishRequest.Size == 0 ? koiFish.Size : updateKoiFishRequest.Size;
        //        koiFish.Breed = updateKoiFishRequest.Breed ?? koiFish.Breed;
        //        koiFish.Type = updateKoiFishRequest.Type ?? koiFish.Type;
        //        koiFish.Price = updateKoiFishRequest.Price == 0 ? koiFish.Price : updateKoiFishRequest.Price;
        //        koiFish.Quantity = updateKoiFishRequest.Quantity == 0 ? koiFish.Quantity : updateKoiFishRequest.Quantity;
        //        koiFish.OwnerType = updateKoiFishRequest.OwnerType ?? koiFish.OwnerType;
        //        koiFish.Description = updateKoiFishRequest.Description ?? koiFish.Description;
        //        koiFish.UpdatedDate = DateTime.Now;
        //        koiFish.UpdatedBy = "Admin";

        //        try
        //        {
        //            var result = _unitOfWork.KoiFishRepository.Update(koiFish);
        //            if (result > 0)
        //            {
        //                return new BusinessResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, koiFish);
        //            }
        //            else
        //            {
        //                return new BusinessResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG, koiFish);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
        //        }
        //    }
        //}

        public IQueryable<KoiFish> GetAllOData()
        {
            return _unitOfWork.KoiFishRepository.GetAll().AsQueryable();
        }
    }
}

