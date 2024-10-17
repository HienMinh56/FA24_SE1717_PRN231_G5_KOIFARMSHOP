using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KoiFarmShop.Data.Models;
using KoiFarmShop.APIService.Controllers;
using KoiFarmShop.Service;
using KoiFarmShop.Common;
using Newtonsoft.Json;
using KoiFarmShop.Service.Base;
using System.Net.Http;

namespace KoiFarmShop.MVCWebApp.Controllers
{
    public class ConsignmentsController : Controller
    {
        // Get: Consignment
        public IActionResult Index()
        {
            return View();
        }
    }
}