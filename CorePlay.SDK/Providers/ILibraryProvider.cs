using CorePlay.SDK.Models.Metadata;

namespace CorePlay.SDK.Providers
{
    public interface ILibraryProvider
    {
        Task<IEnumerable<GameMetadata>> GetGamesAsync();
    }
}
