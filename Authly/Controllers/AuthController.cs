using Authly.Models.Dtos;
using Authly.Services.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti)
                ?? throw new UnauthorizedAccessException("Invalid token.");

            var expClaim = User.FindFirstValue(JwtRegisteredClaimNames.Exp)
                ?? throw new UnauthorizedAccessException("Invalid token.");

            var expiresAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime;

            await authService.LogoutAsync(jti, expiresAt);
            return Ok(new { message = "Logged out successfully." });
        }
    }
}
