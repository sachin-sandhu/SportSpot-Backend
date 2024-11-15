using Rest_Emulator.Enums;
using Rest_Emulator.Services;

namespace Rest_Emulator.Factories
{
    public interface IModeFactory
    {
        IModeService GetModeService(ModeType modeType);
    }
}
