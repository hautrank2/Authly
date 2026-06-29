using Authly.Models.Dtos;
using Authly.Services.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Authly.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] UserQueryDto request)
        {
            var result = await userService.GetAllAsync(request);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] CreateUserDto dto)
        {
            var actorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await userService.CreateAsync(dto, actorId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateUserDto dto)
        {
            var actorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && actorId != id)
                return Forbid();

            var result = await userService.UpdateAsync(id, dto, actorId);
            return Ok(result);
        }

        [HttpPut("{id}/image")]
        public async Task<IActionResult> UpdateImage([FromRoute] string id, IFormFile file)
        {
            var actorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && actorId != id)
                return Forbid();

            var result = await userService.UpdateImageAsync(id, file, actorId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] string id, [FromQuery] bool hardDelete = false)
        {
            await userService.DeleteAsync(id, hardDelete);
            var message = hardDelete ? "User permanently deleted." : "User deleted successfully.";
            return Ok(new { message });
        }

        [HttpPut("{id}/change-password")]
        public async Task<IActionResult> ChangePassword([FromRoute] string id, [FromBody] ChangePasswordDto dto)
        {
            var actorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // Chỉ chính user mới được đổi password của mình
            if (actorId != id)
                return Forbid();

            await userService.ChangePasswordAsync(id, dto);
            return Ok(new { message = "Password changed successfully." });
        }

        [HttpPut("{id}/reset-password")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetPassword([FromRoute] string id, [FromBody] ResetPasswordDto dto)
        {
            await userService.ResetPasswordAsync(id, dto);
            return Ok(new { message = "Password reset successfully." });
        }
    }
}
