using Microsoft.Extensions.DependencyInjection;

namespace CorePlay.SDK.Plugins
{
    public interface IPlugin
    {
        void ConfigureServices(IServiceCollection services);
    }
}
