using CorePlay.SDK.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CorePlay.SDK.Services
{
    public class PluginLoaderService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PluginLoaderService> _logger;

        public PluginLoaderService(IServiceProvider serviceProvider, ILogger<PluginLoaderService> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void LoadPlugins(string pluginFolder, IServiceCollection services)
        {
            var assemblies = LoadAssemblies(pluginFolder);

            foreach (var assembly in assemblies)
            {
                try
                {
                    var pluginTypes = assembly.GetTypes().Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);

                    foreach (var type in pluginTypes)
                    {
                        if (Activator.CreateInstance(type) is IPlugin plugin)
                        {
                            plugin.ConfigureServices(services);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to Load Plugin");
                }
            }
        }

        private IEnumerable<Assembly> LoadAssemblies(string pluginFolder)
        {
            var assemblies = new List<Assembly>();

            foreach (var file in Directory.GetFiles(pluginFolder, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);
                    assemblies.Add(assembly);
                    _logger.LogInformation($"Loaded assembly: {file}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to load assembly: {file}");
                }
            }

            return assemblies;
        }
    }
}