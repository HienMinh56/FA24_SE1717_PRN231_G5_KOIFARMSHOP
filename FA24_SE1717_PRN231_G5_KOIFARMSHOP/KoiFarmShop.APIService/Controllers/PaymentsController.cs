using KoiFarmShop.Common;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Request;
using KoiFarmShop.Data.Request.KoiFarmShop.Data.Request;
using KoiFarmShop.Service;
using KoiFarmShop.Service.Base;
using Microsoft.AspNetCore.Mvc;

namespace KoiFarmShop.APIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // POST: api/Payments
        [HttpPost]
        public async Task<IBusinessResult> PostPayment(CreatePaymentRequest paymentRequest)
        {
            return await _paymentService.Create(paymentRequest);

        }

        // GET: api/Payments
        [HttpGet]
        public async Task<IBusinessResult> GetPayments()
        {
            return await _paymentService.GetAll();
        }

        // GET: api/Payments/{paymentId}
        [HttpGet("{paymentId}")]
        public async Task<IBusinessResult> GetPaymentById(string paymentId)
        {
            return await _paymentService.GetPaymentById(paymentId);
        }

        [HttpPut("{paymentId}")]
        public async Task<IBusinessResult> UpdateStatus(string paymentId, int status)
        {
            return await _paymentService.UpdateStatusForPayment(paymentId, status);
        }
    }
}
