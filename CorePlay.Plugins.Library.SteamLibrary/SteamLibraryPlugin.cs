using CorePlay.SDK.Interfaces.Plugins;
using CorePlay.SDK.Interfaces.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CorePlay.Plugins.Library.SteamLibrary
{
    public class SteamLibraryPlugin : IPlugin
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILogger<SteamClient>, Logger<SteamClient>>();
            services.AddSingleton(provider =>
            {
                var httpClient = provider.GetRequiredService<HttpClient>();
                var logger = provider.GetRequiredService<ILogger<SteamClient>>();
                return new SteamClient("76561198039108376", "FA249E19F2F06407F49840BF69260635", httpClient, logger);
            });
            services.AddSingleton<ILibraryProvider, SteamLibraryProvider>();
            services.AddSingleton<ILogger<SteamLibraryProvider>, Logger<SteamLibraryProvider>>();
        }
    }
}
