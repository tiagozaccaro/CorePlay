using CorePlay.SDK.Database;
using CorePlay.SDK.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CorePlay.SDK.Extensions
{
    public static class SDKExtensions
    {
        public static string GetSDKVersion()
        {
            return typeof(SDKExtensions).Assembly.GetName().Version?.ToString() ?? "Unknown";
        }

        public static string GetBaseDirectory()
        {
            //return AppContext.BaseDirectory;
            return "C:\\Games\\CorePlay";
        }

        public static void AddSDKServices(this IServiceCollection services)
        {
            //create required directories
            Directory.CreateDirectory(Path.Combine(GetBaseDirectory(), "data"));
            Directory.CreateDirectory(Path.Combine(GetBaseDirectory(), "plugins"));

            // Register common services
            services.AddHttpClient();
            services.AddSingleton<PluginLoaderService>();
            services.AddSingleton<ILogger<PluginLoaderService>, Logger<PluginLoaderService>>();
            services.AddSingleton<IGamepadService, GamepadService>();
            
            services.AddSingleton(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<CorePlayDatabaseContext>>();
                return new CorePlayDatabaseContext($"{GetBaseDirectory()}/data/coreplay.db", logger);
            });

            // Build initial service provider
            var serviceProvider = services.BuildServiceProvider();

            // Configure PluginLoader and load plugins
            var pluginLoader = serviceProvider.GetRequiredService<PluginLoaderService>();
            pluginLoader.LoadPlugins($"{GetBaseDirectory()}/plugins", services);
        }

        public static void UseSDKServices(this IServiceProvider provider)
        {            
            var gamepadService = provider.GetService<IGamepadService>();
            gamepadService?.Start();
        }
    }
}
