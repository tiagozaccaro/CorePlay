using CorePlay.SDK.Models.Database;

namespace CorePlay.SDK.Services;

public interface IGameServices
{
    Game GetGameByIdAsync(string gameId);
    ICollection<Game> GetGamesAsync();
}
