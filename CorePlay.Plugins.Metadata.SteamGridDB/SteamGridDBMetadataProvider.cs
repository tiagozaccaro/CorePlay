using CorePlay.SDK.Extensions;
using CorePlay.SDK.Interfaces.Providers;
using CorePlay.SDK.Models;
using craftersmine.SteamGridDBNet;
using Microsoft.Extensions.Logging;

namespace CorePlay.Plugins.Metadata.SteamGridDB
{
    public class SteamGridDBMetadataProvider : IMetadataProvider
    {
        private readonly SteamGridDb _client;
        private readonly ILogger<SteamGridDBMetadataProvider> _logger;

        public SteamGridDBMetadataProvider(SteamGridDb client, ILogger<SteamGridDBMetadataProvider> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<GameMetadata?> GetGameMetadataAsync(string name)
        {
            try
            {
                _logger.LogInformation("Fetching Game Metadata...");
                var games = await _client.SearchForGamesAsync(name);

                if (games == null)
                {
                    _logger.LogWarning("game found in SteamGridDB database.");
                    return null;
                }

                var game = games.FirstOrDefault();

                if (game == null)
                {
                    return null;
                }

                _logger.LogInformation("Mapping game to Game Metadata...");
                return new GameMetadata
                {
                    GameId = game.Id.ToString() ?? string.Empty,
                    Name = game.Name.RemoveTrademarks() ?? string.Empty,
                    Cover = (await _client.GetGridsByGameIdAsync(game.Id, limit: 1)).FirstOrDefault()?.FullImageUrl,
                    Logo = (await _client.GetLogosByGameIdAsync(game.Id, limit: 1)).FirstOrDefault()?.FullImageUrl,
                    Icon = (await _client.GetIconsByGameIdAsync(game.Id, limit: 1)).FirstOrDefault()?.FullImageUrl,
                    Platforms = game.Platforms.Select(p => (MetadataProperty)new MetadataNameProperty(Enum.GetName(p.GetType(), p) ?? string.Empty)).ToHashSet(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the Steam library.");
                throw;
            }
        }
    }
}
