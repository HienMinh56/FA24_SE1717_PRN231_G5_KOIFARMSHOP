using KoiFarmShop.Data.Models;
using KoiFarmShop.Data.Request;
using KoiFarmShop.Service;
using KoiFarmShop.Service.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace KoiFarmShop.APIService.Controllers
{   
    
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController()
        {
            _userService ??= new UserService();
        }

        //public UsersController(UserService userService)
        //{
        //    _userService = userService;
        //}

        // GET: api/Users
        [Authorize]
        [HttpGet]
       
        public async Task<IBusinessResult> GetUsers()
        {
            return await _userService.GetAll();
        }

        // GET: api/Users/5
        [HttpGet("{userId}")]
        public async Task<IBusinessResult> GetUser(string userId)
        {
            return await _userService.GetById(userId);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IBusinessResult> PutUser(User user)
        {
            return await _userService.Save(user);
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IBusinessResult> PostUser(User user)
        {
            return await _userService.Save(user);
        }

        // DELETE: api/Users/5
        [HttpDelete]
        public async Task<IBusinessResult> DeleteUser(string userId)
        {
            return await _userService.DeleteById(userId);
        }


    }
}