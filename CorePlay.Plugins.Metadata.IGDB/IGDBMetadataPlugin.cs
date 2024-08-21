using CorePlay.SDK.Plugins;
using CorePlay.SDK.Providers;
using IGDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CorePlay.Plugins.Metadata.IGDB
{
    public class IGDBMetadataPlugin : IPlugin
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                return new IGDBClient("v6hebjs5clm23zbidgpfepwi61zu25", "sr49mdbkxov6jifm2f2xf54zyteubk");
            });
            services.AddSingleton<IMetadataProvider, IGDBMetadataProvider>();
            services.AddSingleton<ILogger<IGDBMetadataProvider>, Logger<IGDBMetadataProvider>>();
        }
    }
}
