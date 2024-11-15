using Microsoft.AspNetCore.Mvc;
using Rest_Emulator.Enums;
using Rest_Emulator.Factories;

namespace Rest_Emulator.Controllers
{
    [ApiController]
    [Route("")]
    public class OAuthController(IModeFactory _factory) : ControllerBase
    {
        [HttpGet("/oauth")]
        public IActionResult OAuth()
        {
            return _factory.GetModeService(ModeType.GoogleOAuth).GetResult();
        }
    }
}
