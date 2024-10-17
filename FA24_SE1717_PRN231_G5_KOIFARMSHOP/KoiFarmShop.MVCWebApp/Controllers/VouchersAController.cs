using Microsoft.AspNetCore.Mvc;

namespace KoiFarmShop.MVCWebApp.Controllers
{
    public class VouchersAController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
