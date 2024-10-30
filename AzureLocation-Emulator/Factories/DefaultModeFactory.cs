using AzureLocation_Emulator.Enums;
using AzureLocation_Emulator.Services;

namespace AzureLocation_Emulator.Factories
{
    public class DefaultModeFactory(IServiceProvider _serviceProvider) : IModeFactory
    {
        public IModeService GetModeService(ModeType modeType)
        {
            return modeType switch
            {
                ModeType.Search => _serviceProvider.GetRequiredService<SearchModeService>(),
                ModeType.Reverse => _serviceProvider.GetRequiredService<ReverseModeService>(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
