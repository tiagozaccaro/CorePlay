using CorePlay.SDK.Models;

namespace CorePlay.SDK.Providers
{
    public interface ILibraryProvider
    {
        Task<IEnumerable<GameMetadata>> GetGamesAsync();
    }
}
