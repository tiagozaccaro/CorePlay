using CorePlay.SDK.Plugins;
using CorePlay.SDK.Providers;
using craftersmine.SteamGridDBNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CorePlay.Plugins.Metadata.SteamGridDB
{
    public class SteamGridDBMetadataPlugin : IPlugin
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                return new SteamGridDb("7d90173642216861f936526f66904822");
            });
            services.AddSingleton<IMetadataProvider, SteamGridDBMetadataProvider>();
            services.AddSingleton<ILogger<SteamGridDBMetadataProvider>, Logger<SteamGridDBMetadataProvider>>();
        }
    }
}
