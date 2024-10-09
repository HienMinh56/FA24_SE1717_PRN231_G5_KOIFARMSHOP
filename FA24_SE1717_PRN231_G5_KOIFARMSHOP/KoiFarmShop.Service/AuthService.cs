using KoiFarmShop.Common;
using KoiFarmShop.Common.Exceptions;
using KoiFarmShop.Data;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Request;
using KoiFarmShop.Data.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;

namespace KoiFarmShop.Service
{
    public interface IAuthService
    {
        Task<AuthTokensResponse> Login(LoginRequest request);
        Task RegisterAccount(RegisterRequest request);
    }
    public class AuthService : IAuthService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly TokenService _tokenService ;

        public AuthService(UnitOfWork unitOfWork, TokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<AuthTokensResponse> Login(LoginRequest request)
        {
            var account = _unitOfWork.UserRepository.Get(a => a.Email == request.Email
            && a.Password == HashPassword(request.Password));

            if (account is null)
            {
                throw new UnauthorizedException("Wrong email or password");
            }

            string accessToken = _tokenService.GenerateAccessToken(account.Id.ToString(), account.Role.ToString());
            string refreshToken = _tokenService.GenerateRefreshToken();

            return new AuthTokensResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            // Convert the password string to bytes
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Compute the hash
            byte[] hashBytes = sha256.ComputeHash(passwordBytes);

            // Convert the hash to a hexadecimal string
            string hashedPassword = string.Concat(hashBytes.Select(b => $"{b:x2}"));

            return hashedPassword;
        }

        public async Task RegisterAccount(RegisterRequest request)
        {
            var account = await _unitOfWork.UserRepository.GetByIdAsync(request.Email);

            if (account is not null)
            {
                throw new BadRequestException("Account with this email already exists");
            }

            var users = await _unitOfWork.UserRepository.GetAllAsync();
            var Id = users.Count + 1;
            if (_unitOfWork.UserRepository.Get(k => k.UserId == $"{Const.USER}{Id.ToString("D4")}") is not null)
            {
                Id = await _unitOfWork.UserRepository.FindEmptyPositionWithBinarySearch(users, 1, Id, Const.USER, Const.USER_INDEX);
            }

            var userId = $"{Const.USER}{Id.ToString("D4")}";

            var newAccount = new User();
            newAccount.UserId = userId;
            newAccount.FullName = "new user";
            newAccount.UserName = "new user";
            newAccount.Email = request.Email;
            newAccount.Password = HashPassword(request.Password);
            newAccount.Phone = "0123456789";
            newAccount.Role = 1;
            newAccount.Status = 1;
            await _unitOfWork.UserRepository.CreateAsync(newAccount);
            await _unitOfWork.UserRepository.SaveAsync();
        }
    }
}
