using KoiFarmShop.Data.Request;
using KoiFarmShop.Data.Request.KoiFarmShop.Data.Request;
using KoiFarmShop.Service;
using KoiFarmShop.Service.Base;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
    }
}
