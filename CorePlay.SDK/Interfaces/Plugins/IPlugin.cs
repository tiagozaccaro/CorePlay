using Microsoft.Extensions.DependencyInjection;

namespace CorePlay.SDK.Interfaces.Plugins
{
    public interface IPlugin
    {
        void ConfigureServices(IServiceCollection services);
    }
}
