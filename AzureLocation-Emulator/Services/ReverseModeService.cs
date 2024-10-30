using Microsoft.AspNetCore.Mvc;

namespace AzureLocation_Emulator.Services
{
    public class ReverseModeService : IModeService
    {
        public bool Success { get; set; }
        private string? _response;
        public string Response { get => _response ?? throw new InvalidDataException("Response can not be null!"); set => _response = value; }

        public IActionResult GetResult()
        {
            return Success ? new OkObjectResult(Response) : new NotFoundObjectResult(Response);
        }
    }
}
