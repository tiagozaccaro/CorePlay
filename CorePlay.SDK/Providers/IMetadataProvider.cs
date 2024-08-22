using CorePlay.SDK.Models.Metadata;

namespace CorePlay.SDK.Providers
{
    public interface IMetadataProvider
    {
        Task<IEnumerable<GameMetadata?>> SearchGameMetadataByNameAsync(string name, CancellationToken cancellationToken);
        Task<GameMetadata?> GetGameMetadataByNameAsync(string name, CancellationToken cancellationToken);
        Task<GameMetadata?> GetGameMetadataByIdAsync(string id, CancellationToken cancellationToken);
        Task<IEnumerable<PlatformMetadata?>> GetAllPlatformMetadataAsync(CancellationToken cancellationToken);
        Task<PlatformMetadata?> GetPlatformMetadataByNameAsync(string name, CancellationToken cancellationToken);
    }
}
