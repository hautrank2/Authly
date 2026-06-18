using Authly.Models.Dtos;
using Authly.Services.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authly.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] UserQueryDto request)
        {
            var result = await userService.GetAllAsync(request);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] CreateUserDto dto)
        {
            var result = await userService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] string id, [FromForm] UpdateUserDto dto)
        {
            var result = await userService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpPut("{id}/image")]
        [Authorize]
        public async Task<IActionResult> UpdateImage([FromRoute] string id, IFormFile file)
        {
            var result = await userService.UpdateImageAsync(id, file);
            return Ok(result);
        }
    }
}
