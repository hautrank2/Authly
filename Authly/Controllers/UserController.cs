using Authly.Models.Dtos;
using Authly.Services.Dtos;

using Microsoft.AspNetCore.Mvc;

namespace Authly.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] UserQueryDto request)
        {
            var result = await userService.GetAllAsync(request);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateUserDto dto)
        {
            var result = await userService.CreateAsync(dto);
            return Ok(result);
        }
    }
}
