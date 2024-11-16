using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportSpot.V1.Location.Services;
using SportSpot.V1.Media.Services;
using SportSpot.V1.User.Services;

namespace SportSpot.V1.Admin
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AdminController(IWebHostEnvironment _enviornment, IAuthService _authService, IMediaService _mediaService, ILocationCacheService _locationCache) : ControllerBase
    {
        [AllowAnonymous]
        [HttpDelete("seed-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SeedData()
        {
            if (!_enviornment.IsDevelopment())
            {
                return NotFound();
            }
            _locationCache.FlushCache();
            await _authService.DeleteAllUser();
            await _mediaService.DeleteAllMedia();
            return Ok();
        }
    }
}