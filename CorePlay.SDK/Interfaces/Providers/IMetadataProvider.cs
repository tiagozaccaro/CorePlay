using CorePlay.SDK.Models;

namespace CorePlay.SDK.Interfaces.Providers
{
    public interface IMetadataProvider
    {
        Task<GameMetadata?> GetGameMetadataAsync(string name);
    }
}
