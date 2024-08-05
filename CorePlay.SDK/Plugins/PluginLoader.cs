using CorePlay.SDK.Interfaces.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace CorePlay.SDK.Plugins
{
    public class PluginLoader
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PluginLoader> _logger;

        public PluginLoader(IServiceProvider serviceProvider, ILogger<PluginLoader> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void LoadPlugins(string pluginFolder, IServiceCollection services)
        {
            var assemblies = LoadAssemblies(pluginFolder);

            foreach (var assembly in assemblies)
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
