using DotNet_Quick_ref_all.Models.Auth;
using DotNet_Quick_ref_all.Services.Auth;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotNet_Quick_ref_all.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;

        public AuthController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] DotNet_Quick_ref_all.Models.Auth.LoginRequest request)
        {
            // TODO: replace with real DB user lookup
            if (request.Username == "admin" && request.Password == "password")
            {
                var token = _tokenService.GenerateToken(
                    userId: "1",
                    email: "admin@example.com",
                    role: "Admin"
                );

                return Ok(new LoginResponse
                {
                    Token = token,
                    Username = request.Username
                });
            }

            return Unauthorized(new { message = "Invalid username or password" });
        }
    }
}
