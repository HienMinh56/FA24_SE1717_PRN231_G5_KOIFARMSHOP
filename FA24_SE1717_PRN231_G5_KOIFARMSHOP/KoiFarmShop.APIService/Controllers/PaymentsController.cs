using KoiFarmShop.Common;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Request;
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


        // GET: api/Consignments/5
        [HttpGet("{paymentId}")]
        public async Task<IBusinessResult> GetConsignment(string paymentId)
        {
            return await _paymentService.GetById(paymentId);
        }

        // GET: api/Payments
        [HttpGet]
        public async Task<IBusinessResult> GetPayments()
        {
            return await _paymentService.GetAll();
        }

        [HttpPut]
        public async Task<IBusinessResult> PutPayment(UpdatePaymentRequest payment)
        {
            return await _paymentService.Update(payment);

        }

        [HttpDelete("{paymentId}")]
        public async Task<IBusinessResult> DeletePayment(string paymentId)
        {
            return await _paymentService.DeleteById(paymentId);
        }
    }
}
