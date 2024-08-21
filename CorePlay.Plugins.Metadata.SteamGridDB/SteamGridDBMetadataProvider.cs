using CorePlay.SDK.Extensions;
using CorePlay.SDK.Models;
using CorePlay.SDK.Providers;
using craftersmine.SteamGridDBNet;
using Microsoft.Extensions.Logging;

namespace CorePlay.Plugins.Metadata.SteamGridDB
{
    public class SteamGridDBMetadataProvider(SteamGridDb client, ILogger<SteamGridDBMetadataProvider> logger) : IMetadataProvider
    {
        private readonly SteamGridDb _client = client ?? throw new ArgumentNullException(nameof(client));
        private readonly ILogger<SteamGridDBMetadataProvider> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<IEnumerable<PlatformMetadata?>> GetAllPlatformMetadataAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<GameMetadata?> GetGameMetadataByNameAsync(string name, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching Game Metadata...");
                var games = await _client.SearchForGamesAsync(name);

                if (games == null)
                {
                    _logger.LogWarning("game not found in SteamGridDB database.");
                    return null;
                }

                var game = games.FirstOrDefault();

                if (game == null)
                {
                    return null;
                }

                _logger.LogInformation("Mapping game to Game Metadata...");
                return await GetGameMetadataAsync(game, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the metadata.");
                throw;
            }
        }

        private async Task<GameMetadata?> GetGameMetadataAsync(SteamGridDbGame game, CancellationToken cancellationToken)
        {
            return new GameMetadata
            {
                Id = game.Id.ToString() ?? string.Empty,
                Name = game.Name.RemoveTrademarks() ?? string.Empty,
                Cover = (await _client.GetGridsByGameIdAsync(game.Id, limit: 1)).FirstOrDefault()?.FullImageUrl,
                Logo = (await _client.GetLogosByGameIdAsync(game.Id, limit: 1)).FirstOrDefault()?.FullImageUrl,
                Icon = (await _client.GetIconsByGameIdAsync(game.Id, limit: 1)).FirstOrDefault()?.FullImageUrl,
                Platforms = game.Platforms.Select(p => (MetadataProperty)new MetadataNameProperty(Enum.GetName(p.GetType(), p) ?? string.Empty)).ToHashSet(),
            };
        }

        public async Task<GameMetadata?> GetGameMetadataByIdAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching Game Metadata...");
                var game = await _client.GetGameByIdAsync(int.Parse(id));

                if (game == null)
                {
                    _logger.LogWarning("game not found in SteamGridDB database.");
                    return null;
                }

                _logger.LogInformation("Mapping game to Game Metadata...");
                return await GetGameMetadataAsync(game, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the metadata.");
                throw;
            }
        }

        public async Task<PlatformMetadata?> GetPlatformMetadataByNameAsync(string name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<GameMetadata?>> SearchGameMetadataByNameAsync(string name, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching Game Metadata...");
                var games = await _client.SearchForGamesAsync(name);

                if (games == null)
                {
                    _logger.LogWarning("game not found in SteamGridDB database.");
                    return null;
                }

                ICollection<GameMetadata?> gamesMetadata = [];

                foreach (var game in games)
                {
                    gamesMetadata.Add(await GetGameMetadataAsync(game, cancellationToken));
                }

                return gamesMetadata;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the metadata.");
                throw;
            }
        }
    }
}
