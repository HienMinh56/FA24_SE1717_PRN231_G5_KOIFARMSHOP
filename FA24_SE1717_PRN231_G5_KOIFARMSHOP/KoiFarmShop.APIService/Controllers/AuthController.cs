using KoiFarmShop.Data.Request;
using KoiFarmShop.Data.Response;
using KoiFarmShop.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KoiFarmShop.APIService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthTokensResponse>> Login([FromBody] LoginRequest request)
        {
            return Created(nameof(Login), await _authService.Login(request));
        }


        [HttpPost("register")]
        public async Task<ActionResult> RegisterAccount([FromBody] RegisterRequest request)
        {
            await _authService.RegisterAccount(request);
            return Ok();
        }
    }
}