using CorePlay.SDK.Extensions;
using CorePlay.SDK.Models;
using CorePlay.SDK.Providers;
using Microsoft.Extensions.Logging;

namespace CorePlay.Plugins.Metadata.ScreenScraper
{
    public class ScreenScraperMetadataProvider(ScreenScraperClient client, ILogger<ScreenScraperMetadataProvider> logger) : IMetadataProvider
    {
        private readonly ScreenScraperClient _client = client ?? throw new ArgumentNullException(nameof(client));
        private readonly ILogger<ScreenScraperMetadataProvider> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<IEnumerable<PlatformMetadata?>> GetAllPlatformMetadataAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching Platform Metadata...");
                var results = await _client.GetSystemListAsync();

                if (results == null)
                {
                    _logger.LogWarning("platform not found in ScreenScraper database.");
                    return null;
                }

                ICollection<PlatformMetadata?> platformsMetadata = [];

                foreach (var platform in results.Systems)
                {
                    platformsMetadata.Add(await GetPlatformMetadataAsync(platform, cancellationToken));
                }

                return platformsMetadata;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the metadata.");
                throw;
            }
        }

        public async Task<GameMetadata?> GetGameMetadataByNameAsync(string name, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching Game Metadata...");
                var game = await _client.GetGameInfoAsync(name);

                if (game == null)
                {
                    _logger.LogWarning("game not found in ScreenScraper database.");
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

        private async Task<GameMetadata?> GetGameMetadataAsync(GameInfo game, CancellationToken cancellationToken)
        {
            return new GameMetadata
            {
                Id = game.Id.ToString() ?? string.Empty,
                Name = game.Title.RemoveTrademarks() ?? string.Empty,
                Description = game.Description,
                Cover = (await _client.GetGameMediaAsync(game.Id, "box-2D"))?.MediaId,
                Logo = (await _client.GetGameMediaAsync(game.Id, "Wheel"))?.MediaId,
                Icon = (await _client.GetGameMediaAsync(game.Id, "box-2D"))?.MediaId,
            };
        }

        private async Task<PlatformMetadata?> GetPlatformMetadataAsync(SystemInfo platform, CancellationToken cancellationToken)
        {
            return new PlatformMetadata
            {
                Id = platform.SystemId.ToString() ?? string.Empty,
                Name = platform.Name.RemoveTrademarks() ?? string.Empty,
                Description = platform.Description,
            };
        }

        public async Task<GameMetadata?> GetGameMetadataByIdAsync(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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
                var results = await _client.SearchGamesAsync(name);

                if (results == null)
                {
                    _logger.LogWarning("game not found in ScreenScraper database.");
                    return null;
                }

                ICollection<GameMetadata?> gamesMetadata = [];

                foreach (var game in results.Games)
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
