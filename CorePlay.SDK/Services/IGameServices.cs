using System;
using CorePlay.SDK.Models;

namespace CorePlay.SDK.Services;

public interface IGameServices
{
    Game GetGameByIdAsync(string gameId);
    ICollection<Game> GetGamesAsync();
}
