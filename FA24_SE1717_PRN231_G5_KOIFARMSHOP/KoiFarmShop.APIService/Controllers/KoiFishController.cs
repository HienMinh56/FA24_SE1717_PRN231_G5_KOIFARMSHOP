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

namespace KoiFarmShop.APIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KoiFishController : ControllerBase
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
        public async Task<IBusinessResult> GetKoiFish(string code)
        {
            var result = await _koiFishService.GetById(code);
            return result;
        }

        [HttpPut("{id}")]
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
    }
}
