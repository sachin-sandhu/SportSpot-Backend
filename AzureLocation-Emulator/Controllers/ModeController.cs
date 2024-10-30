using AzureLocation_Emulator.Dtos;
using AzureLocation_Emulator.Factories;
using AzureLocation_Emulator.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureLocation_Emulator.Controllers
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
