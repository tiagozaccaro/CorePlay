using CorePlay.SDK.Interfaces.Plugins;
using CorePlay.SDK.Interfaces.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CorePlay.Plugins.Metadata.ScreenScraper
{
    public class ScreenScraperMetadataPlugin : IPlugin
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                return new ScreenScraperClient("", "", "27718535", "L0newolf");
            });

            services.AddSingleton<IMetadataProvider, ScreenScraperMetadataProvider>();
            services.AddSingleton<ILogger<ScreenScraperMetadataProvider>, Logger<ScreenScraperMetadataProvider>>();
        }
    }
}
