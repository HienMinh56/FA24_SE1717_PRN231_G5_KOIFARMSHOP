using Grpc.Core;
using KoiFarmShop.Data;
using KoiFarmShop.Data.Models;
using KoiFarmShop.GRPC.Protos;
using Microsoft.AspNetCore.Http.HttpResults;


namespace KoiFarmShop.GRPC.Services
{
    public class KoiFishGRPCServices : KoiFishGRPCService.KoiFishGRPCServiceBase
    {
        private readonly ILogger<KoiFishGRPCServices> _logger;
        private readonly UnitOfWork _unitOfWork;
        public KoiFishGRPCServices(ILogger<KoiFishGRPCServices> logger, UnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }


        #region Basic
        public override Task<KoiFishResponse> GetKoiFish(GetKoiFishByIdRequest request, ServerCallContext context)
        {
            var koifish = _unitOfWork.KoiFishRepository.Get(k => k.KoiId == request.KoiId);
            if (koifish == null)
            {
                return Task.FromResult(new KoiFishResponse());
            }

            return Task.FromResult(new KoiFishResponse
            {
                KoiId = koifish.KoiId,
                KoiName = koifish.KoiName,
                Origin = koifish.Origin,
                Gender = koifish.Gender,
                Age = koifish.Age,
                Size = koifish.Size,
                Breed = koifish.Breed,
                Type = koifish.Type,
                Price = koifish.Price,
                Quantity = koifish.Quantity,
                OwnerType = koifish.OwnerType,
                Description = koifish.Description
            });
        }
        #endregion


        public override async Task GetKoiFishes(EmptyRequest request, IServerStreamWriter<KoiFishResponse> responseStream, ServerCallContext context)
        {
            // Gửi danh sách nhân viên
            foreach (var koifish in (await _unitOfWork.KoiFishRepository.GetAllAsync()))
            {
                await responseStream.WriteAsync(new KoiFishResponse
                {
                    KoiId = koifish.KoiId,
                    KoiName = koifish.KoiName,
                    Origin = koifish.Origin,
                    Gender = koifish.Gender,
                    Age = koifish.Age,
                    Size = koifish.Size,
                    Breed = koifish.Breed,
                    Type = koifish.Type,
                    Price = koifish.Price,
                    Quantity = koifish.Quantity,
                    OwnerType = koifish.OwnerType,
                    Description = koifish.Description
                });
                await Task.Delay(1000); // Giả lập độ trễ giữa các phản hồi
            }
        }

        public override async Task<KoiFishReply> AddKoiFish(IAsyncStreamReader<CreateKoiFishRequest> requestStream, ServerCallContext context)
        {
            int totalKoiFishes = 0;
            string addedList = "";
            // Đọc tất cả nhân viên từ client gửi đến
            await foreach (var koifish in requestStream.ReadAllAsync())
            {
                var createdId = Guid.NewGuid().ToString().Substring(2, 5);
                await _unitOfWork.KoiFishRepository.CreateAsync(new KoiFish
                {
                    KoiId = createdId,
                    KoiName = koifish.KoiName,
                    Origin = koifish.Origin,
                    Gender = koifish.Gender,
                    Age = koifish.Age,
                    Size = koifish.Size,
                    Breed = koifish.Breed,
                    Type = koifish.Type,
                    Price = koifish.Price,
                    Quantity = koifish.Quantity,
                    OwnerType = koifish.OwnerType,
                    Description = koifish.Description
                }); // Thêm nhân viên vào danh sách
                totalKoiFishes++;
                addedList = string.Join(", ", $"ID: {createdId}, " +
                    $"Name: {koifish.KoiName}, " +
                    $"Gender: {koifish.Gender}" +
                    $"Age: {koifish.Age}" +
                    $"Size: {koifish.Size}" +
                    $"Breed: {koifish.Breed}" +
                    $"Type: {koifish.Type}" +
                    $"Price: {koifish.Price}" +
                    $"Quantity: {koifish.Quantity}" +
                    $"OwnerType: {koifish.OwnerType}" +
                    $"Description: {koifish.Description}");
            }

            return new KoiFishReply
            {
                Message = $"Total koifishes added: {totalKoiFishes} {addedList}"
            };
        }
    }
}
