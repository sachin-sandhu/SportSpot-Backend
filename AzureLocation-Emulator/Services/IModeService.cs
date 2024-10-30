using Microsoft.AspNetCore.Mvc;

namespace AzureLocation_Emulator.Services
{
    public interface IModeService
    {
        bool Success { get; set; }
        string Response { get; set; }
        IActionResult GetResult(); 
    }
}
