using KoiFarmShop.Common;
using KoiFarmShop.Data;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Service.Base;

namespace KoiFarmShop.Service
{
    public interface IUserService
    {
        Task<IBusinessResult> GetAll();
        Task<IBusinessResult> GetById(string Id);
        Task<IBusinessResult> Save(User user);
        Task<IBusinessResult> DeleteById(string Id);
        //Task<IBusinessResult> Create(User user);
        //Task<IBusinessResult> Update(User user);
    }

    public class UserService : IUserService
    {
        private readonly UnitOfWork _unitOfWork;

        public UserService()
        {
            _unitOfWork ??= new UnitOfWork();
        }

        public async Task<IBusinessResult> GetAll()
        {
            #region Business rule

            #endregion

            var users = await _unitOfWork.UserRepository.GetAllAsync();

            if (users == null)
            {
                return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new List<User>());
            }
            else
            {
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, users);
            }
        }

        public async Task<IBusinessResult> GetById(string userId)
        {
            #region Business rule

            #endregion

            var user =  _unitOfWork.UserRepository.Get(u => u.UserId == userId);

            if (user == null)
            {
                return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new User());
            }
            else
            {
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, user);
            }
        }

        public async Task<IBusinessResult> Save(User user)
        {
            #region Business rule

            #endregion

            try
            {
                int result = -1;

                var userTmp = _unitOfWork.UserRepository.Get(u => u.UserId == user.UserId);

                if (userTmp != null)
                {
                    #region Business Rule

                    #endregion

                    result = await _unitOfWork.UserRepository.UpdateAsync(user);

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
                    result = await _unitOfWork.UserRepository.CreateAsync(user);

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

        public async Task<IBusinessResult> DeleteById(string userId)
        {
            #region Business rule

            #endregion

            try
            {
                var user =  _unitOfWork.UserRepository.Get(u => u.UserId == userId);

                if (user == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG, new User());
                }
                else
                {
                    var result = await _unitOfWork.UserRepository.RemoveAsync(user);

                    if (result)
                    {
                        return new BusinessResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG, user);
                    }
                    else
                    {
                        return new BusinessResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG, user);
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
