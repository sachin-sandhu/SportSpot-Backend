using Rest_Emulator.Enums;
using System.ComponentModel.DataAnnotations;

namespace Rest_Emulator.Dtos
{
    public record SetModeDto
    {
        [Required]
        public required ModeType Mode { get; init; }
        [Required]
        public required string Response { get; init; }
        public bool Success { get; init; } = true;
    }
}
