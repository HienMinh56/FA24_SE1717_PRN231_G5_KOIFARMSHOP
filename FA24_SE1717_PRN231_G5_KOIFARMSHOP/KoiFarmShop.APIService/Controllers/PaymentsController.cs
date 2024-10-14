using KoiFarmShop.Common;
using KoiFarmShop.Data.Request;
using KoiFarmShop.Data.Request.KoiFarmShop.Data.Request;
using KoiFarmShop.Service;
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
        public async Task<IActionResult> PostPayment(CreatePaymentRequest paymentRequest)
        {
            var result = await _paymentService.Create(paymentRequest);
            if (result.Status == Const.SUCCESS_CREATE_CODE)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // GET: api/Payments
        [HttpGet]
        public async Task<IActionResult> GetPayments()
        {
            var result = await _paymentService.GetAll();
            return Ok(result);
        }

        // GET: api/Payments/{paymentId}
        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPaymentById(string paymentId)
        {
            var result = await _paymentService.GetById(paymentId);
            if (result.Status == Const.SUCCESS_READ_CODE)
            {
                return Ok(result);
            }
            return NotFound(result);
        }
    }
}
