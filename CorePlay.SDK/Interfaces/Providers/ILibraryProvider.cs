using CorePlay.SDK.Models;

namespace CorePlay.SDK.Interfaces.Providers
{
    public interface ILibraryProvider
    {
        Task<IEnumerable<GameMetadata>> GetGamesAsync();
    }
}
