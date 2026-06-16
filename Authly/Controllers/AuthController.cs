using Authly.Models.Dtos;
using Authly.Services.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Authly.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await authService.LoginAsync(request);
            return Ok(result);
        }
    }
}
