using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CorePlay.SDK.Database;
using CorePlay.SDK.Interfaces.Providers;
using CorePlay.SDK.Plugins;
using CorePlay.ViewModels;
using CorePlay.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CorePlay
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var services = new ServiceCollection();
            // Register common services
            services.AddHttpClient();
            services.AddSingleton<ILogger<PluginLoader>, Logger<PluginLoader>>();
            services.AddSingleton<PluginLoader>();
            services.AddSingleton<ILogger<PluginLoader>, Logger<PluginLoader>>();
            services.AddSingleton(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<CorePlayDatabaseContext>>();
                return new CorePlayDatabaseContext("D:\\Documents\\CorePlay\\deploy\\plugins\\coreplay.db", logger);
            });

            // Build initial service provider
            var serviceProvider = services.BuildServiceProvider();

            // Configure PluginLoader and load plugins
            var pluginLoader = serviceProvider.GetRequiredService<PluginLoader>();
            pluginLoader.LoadPlugins("D:\\Documents\\CorePlay\\deploy\\plugins", services);

            // Build the final service provider
            serviceProvider = services.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainViewModel(serviceProvider.GetService<CorePlayDatabaseContext>(), serviceProvider.GetServices<ILibraryProvider>(), serviceProvider.GetServices<IMetadataProvider>())
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = new MainViewModel(serviceProvider.GetService<CorePlayDatabaseContext>(), serviceProvider.GetServices<ILibraryProvider>(), serviceProvider.GetServices<IMetadataProvider>())
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}