using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Common;
using System.Net.Http;
using KoiFarmShop.Service.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KoiFarmShop.MVCWebApp.Controllers
{
    public class OrdersController : Controller
    {
        // GET: Orders
        public IActionResult Index()
        {
            return View();
        }

    }
}
