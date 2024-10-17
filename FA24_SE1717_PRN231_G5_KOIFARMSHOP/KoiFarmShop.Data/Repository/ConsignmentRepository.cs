using KoiFarmShop.Data.Base;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Request;
using Microsoft.EntityFrameworkCore;

namespace KoiFarmShop.Data.Repository
{
    public class ConsignmentRepository : GenericRepository<Consignment>
    {
        public ConsignmentRepository()
        {
        }

        public ConsignmentRepository(FA24_SE1717_PRN231_G5_KOIFARMSHOPContext context)
        {
            _context = context;
        }

        public async Task<List<Consignment>> GetAllConsignmentAsync()
        {
            var consignments = await _context.Consignments
                                .AsNoTracking()
                                .Include(k => k.Koi)
                                .Include(u => u.User)
                                .Include(p => p.Payment)
                                .ToListAsync();
            return consignments;
        }

        public async Task<Consignment> GetConsignmentByIdAsync(string ConsignmentId)
        {
            var consignment = await _context.Consignments
                                            .AsNoTracking()
                                            .Where(c => c.ConsignmentId == ConsignmentId)
                                            .Include(k => k.Koi)
                                            .Include(u => u.User)
                                            .Include(p => p.Payment)
                                            .FirstOrDefaultAsync();

            return consignment ?? new Consignment();
        }

        public async Task<Consignment> CreateConsignmentAsync(CreateConsignmentRequest createConsignmentRequest)
        {
            try
            {
                // Tạo ConsignmentId và PaymentId
                var ConsignmentId = $"CONSIGN{(await Count() + 1).ToString("D3")}";
                string paymentId = $"PAY{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

                // Khởi tạo đối tượng Consignment
                var consignmentTmp = new Consignment
                {
                    ConsignmentId = ConsignmentId,
                    UserId = createConsignmentRequest.UserId,
                    KoiId = createConsignmentRequest.KoiId,
                    Type = createConsignmentRequest.Type, // 1: Care, 2: Sale
                    Method = createConsignmentRequest.Method,
                    DealPrice = createConsignmentRequest.DealPrice,
                    PaymentId = paymentId, // Liên kết với Payment
                    Status = 1, // 1:Pending, 2:Agreed, 3: In store, 4:Sold, 5:Return, 6:Cancel
                    ConsignmentDate = DateOnly.FromDateTime(DateTime.Now),
                    CreatedDate = DateTime.Now,
                    CreatedBy = createConsignmentRequest.UserId
                };

                // Thêm Consignment vào context
                await _context.Consignments.AddAsync(consignmentTmp);

                // Tạo đối tượng Payment và liên kết với Order
                var payment = new Payment
                {
                    PaymentId = paymentId,
                    UserId = createConsignmentRequest.UserId,
                    Amount = createConsignmentRequest.DealPrice ?? 0,
                    Type = 1, // Loại thanh toán
                    Status = 0, // Trạng thái chưa thanh toán
                    CreatedDate = DateTime.Now,
                };

                // Thêm Payment vào context
                await _context.Payments.AddAsync(payment);

                // Lưu thông tin vào database
                await _context.SaveChangesAsync();

                // Trả về Consignment vừa tạo
                return consignmentTmp;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                throw new Exception($"An error occurred while creating the consignment: {ex.Message}", ex);
            }
        }
    }
}