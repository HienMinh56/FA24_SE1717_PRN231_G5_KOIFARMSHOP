using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Service.Base;
using KoiFarmShop.Service;
using KoiFarmShop.Data.Request;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace KoiFarmShop.APIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KoiFishController : ODataController
    {
        private readonly IKoiFishService _koiFishService;

        public KoiFishController(IKoiFishService koiFishService)
        {
            _koiFishService = koiFishService;
        }

        [HttpGet]
        public async Task<IBusinessResult> GetKoiFishes()
        {
            return await _koiFishService.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<IBusinessResult> GetKoiFish(string id)
        {
            var result = await _koiFishService.GetById(id);
            return result;
        }

        [HttpPut]
        public async Task<IBusinessResult> PutKoiFish(UpdateKoiFishRequest updateKoiFishRequest)
        {
            var result = await _koiFishService.Update(updateKoiFishRequest);
            return result;
        }

        [HttpPost]
        public async Task<IBusinessResult> PostKoiFish(CreateKoiFishRequest koiFish)
        {
            var result = await _koiFishService.Create(koiFish);
            return result;
        }

        [HttpDelete("{id}")]
        public async Task<IBusinessResult> DeleteKoiFish(string id)
        {
            var result = await _koiFishService.DeleteById(id);
            return result;
        }

        [EnableQuery]
        [HttpGet("odata")]
        public async Task<IQueryable<KoiFish>> GetKoiFishesOData()
        {
            var koiFishes = await _koiFishService.GetAllOData();
            if (koiFishes == null)
            {
                return new List<KoiFish>().AsQueryable();
            }
            return koiFishes;
        }
    }
}
