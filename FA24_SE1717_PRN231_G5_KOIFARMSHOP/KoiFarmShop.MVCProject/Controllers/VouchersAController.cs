using Microsoft.AspNetCore.Mvc;

namespace KoiFarmShop.MVCProject.Controllers
{
    public class VouchersAController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
