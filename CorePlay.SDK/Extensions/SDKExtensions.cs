using CorePlay.SDK.Database;
using CorePlay.SDK.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CorePlay.SDK.Extensions
{
    public static class SDKExtensions
    {
        public static void AddSDKServices(this IServiceCollection services)
        {            
            // Register common services
            services.AddHttpClient();
            services.AddSingleton<PluginLoaderService>();
            services.AddSingleton<ILogger<PluginLoaderService>, Logger<PluginLoaderService>>();
            services.AddSingleton<IGamepadService, GamepadService>();
            
            services.AddSingleton(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<CorePlayDatabaseContext>>();
                return new CorePlayDatabaseContext("D:/Documents/CorePlay/deploy/plugins/coreplay.db", logger);
            });

            // Build initial service provider
            var serviceProvider = services.BuildServiceProvider();

            // Configure PluginLoader and load plugins
            var pluginLoader = serviceProvider.GetRequiredService<PluginLoaderService>();
            pluginLoader.LoadPlugins("D:/Documents/CorePlay/deploy/plugins", services);
        }

        public static void UseSDKServices(this IServiceProvider provider)
        {            
            var gamepadService = provider.GetService<IGamepadService>();
            gamepadService?.Start();
        }
    }
}
