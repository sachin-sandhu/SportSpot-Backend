using Rest_Emulator.Dtos;
using Rest_Emulator.Factories;
using Microsoft.AspNetCore.Mvc;
using Rest_Emulator.Services;

namespace Rest_Emulator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModeController(IModeFactory _factory) : ControllerBase
    {
        [HttpPost]
        public IActionResult SetMode([FromBody] SetModeDto request)
        {
            IModeService modeService = _factory.GetModeService(request.Mode);
            modeService.Success = request.Success;
            modeService.Response = request.Response;
            return Ok();
        }
    }
}
