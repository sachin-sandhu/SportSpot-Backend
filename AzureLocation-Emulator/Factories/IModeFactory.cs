using AzureLocation_Emulator.Enums;
using AzureLocation_Emulator.Services;

namespace AzureLocation_Emulator.Factories
{
    public interface IModeFactory
    {
        IModeService GetModeService(ModeType modeType);
    }
}
