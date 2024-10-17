using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Request;
using KoiFarmShop.Service;
using KoiFarmShop.Service.Base;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KoiFarmShop.APIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageService _imageService;
        public ImagesController(IImageService imageService)
        {
            _imageService = imageService;
        }

        // GET: api/<ImagesController>
        [HttpGet]
        public async Task<IBusinessResult> GetAll()
        {
            var result = await _imageService.GetImages();
            return result;
        }

        // GET api/<ImagesController>/5
        [HttpGet("{id}")]
        public async Task<IBusinessResult> GetById(string id)
        {
            return await _imageService.GetImageById(id);
        }

        [HttpPost]
        public async Task<IBusinessResult> Create([FromBody] ImageRequest image)
        {
            var result = await _imageService.Save(image);
            return result;
        }

        [HttpPut]
        public async Task<IBusinessResult> Update([FromBody] ImageRequest image)
        {
            var result = await _imageService.Save(image);
            return result;
        }

        [HttpDelete("{imageId}")]
        public async Task<IBusinessResult> Delete(string imageId)
        {
            var result = await _imageService.Delete(imageId);
            return result;
        }
    }
}
