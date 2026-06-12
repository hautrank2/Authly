using Authly.Models.Dtos;
using Authly.Services.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authly.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] UserQueryDto request)
        {
            var result = await userService.GetAllAsync(request);
            return Ok(result);
        }
    }
}
