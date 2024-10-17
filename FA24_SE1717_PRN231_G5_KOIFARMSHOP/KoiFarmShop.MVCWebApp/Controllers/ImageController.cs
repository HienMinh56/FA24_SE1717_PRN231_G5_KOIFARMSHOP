using KoiFarmShop.Common;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Service.Base;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KoiFarmShop.MVC.Controllers
{
    public class ImageController : Controller
    {
        private readonly FA24_SE1717_PRN231_G5_KOIFARMSHOPContext _context;

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<List<Image>> GetAll()
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Images"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);
                        if (result is not null && result.Data is not null)
                        {
                            var data = JsonConvert.DeserializeObject<List<Image>>
                                (result.Data.ToString());

                            return data;
                        }
                    }
                    return null;
                }
            }
        }
    }
}
