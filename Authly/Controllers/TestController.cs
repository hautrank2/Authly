using Authly.Services.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authly.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController(ICloudinaryService cloudinaryService) : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var result = await cloudinaryService.UploadImageAsync(file, "test");
            return Ok(result);
        }
    }
}
