using Rest_Emulator.Enums;
using Rest_Emulator.Services;

namespace Rest_Emulator.Factories
{
    public class DefaultModeFactory(IServiceProvider _serviceProvider) : IModeFactory
    {
        public IModeService GetModeService(ModeType modeType)
        {
            return modeType switch
            {
                ModeType.SearchLocation => _serviceProvider.GetRequiredService<SearchModeService>(),
                ModeType.ReverseLocation => _serviceProvider.GetRequiredService<ReverseModeService>(),
                ModeType.GoogleOAuth => _serviceProvider.GetRequiredService<GoogleOAuthModeService>(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
