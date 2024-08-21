using CorePlay.SDK.Extensions;
using CorePlay.SDK.Models;
using CorePlay.SDK.Providers;
using IGDB;
using Microsoft.Extensions.Logging;
using IGDBModels = IGDB.Models;

namespace CorePlay.Plugins.Metadata.IGDB
{
    public class IGDBMetadataProvider(IGDBClient client, ILogger<IGDBMetadataProvider> logger) : IMetadataProvider
    {
        private readonly IGDBClient _client = client ?? throw new ArgumentNullException(nameof(client));
        private readonly ILogger<IGDBMetadataProvider> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<GameMetadata?> GetGameMetadataByIdAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching Game Metadata...");
                var games = await _client.QueryAsync<IGDBModels.Game>(IGDBClient.Endpoints.Games, $"fields id,name,cover.image_id,artworks.image_id,videos.video_id; where id = \"{id}\";");

                if (games == null)
                {
                    _logger.LogWarning("game not found in IGDB database.");
                    return null;
                }

                var game = games.FirstOrDefault();

                if (game == null)
                {
                    return null;
                }

                return await GetGameMetadataAsync(game, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the the metadata.");
                throw;
            }
        }

        public async Task<IEnumerable<GameMetadata?>> SearchGameMetadataByNameAsync(string name, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching Game Metadata...");
                var games = await _client.QueryAsync<IGDBModels.Game>(IGDBClient.Endpoints.Games, $"fields id,name,summary,cover.image_id,artworks.image_id,videos.video_id,platforms.name; where name = \"{name}\";");

                if (games == null)
                {
                    _logger.LogWarning("game not found in IGDB database.");
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
                _logger.LogError(ex, "An error occurred while fetching the the metadata.");
                throw;
            }
        }

        public async Task<GameMetadata?> GetGameMetadataByNameAsync(string name, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching Game Metadata...");
                var games = await _client.QueryAsync<IGDBModels.Game>(IGDBClient.Endpoints.Games, $"fields id,name,summary,cover.image_id,artworks.image_id,videos.video_id,platforms.name; where name = \"{name}\";");

                if (games == null)
                {
                    _logger.LogWarning("game not found in IGDB database.");
                    return null;
                }

                var game = games.FirstOrDefault();

                if (game == null)
                {
                    return null;
                }

                return await GetGameMetadataAsync(game, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the the metadata.");
                throw;
            }
        }

        private async Task<GameMetadata?> GetGameMetadataAsync(IGDBModels.Game game, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Mapping game to Game Metadata...");
            return new GameMetadata
            {
                Id = game.Id?.ToString() ?? string.Empty,
                Name = game.Name?.RemoveTrademarks() ?? string.Empty,
                Description = game.Summary?.ToString() ?? string.Empty,
                Cover = game.Cover?.Value != null ? $"https:{ImageHelper.GetImageUrl(imageId: game.Cover.Value.ImageId, ImageSize.CoverBig, retina: true)}" : null,
                Videos = game.Videos?.Values != null ? game.Videos.Values.Select(v => $"https://www.youtube.com/watch?v={v.VideoId}").ToList() : [],
                Platforms = game.Platforms?.Values.Select(p => (MetadataProperty)new MetadataNameProperty(p.Name)).ToHashSet(),
            };
        }

        public async Task<PlatformMetadata?> GetPlatformMetadataByNameAsync(string name, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching Platform Metadata...");
                var platforms = await _client.QueryAsync<IGDBModels.Platform>(IGDBClient.Endpoints.Platforms, $"fields abbreviation,alternative_name,category,checksum,created_at,generation,name,platform_family,platform_logo.image_id,slug,summary,updated_at,url,versions,websites; where name = \"{name}\";");

                if (platforms == null)
                {
                    _logger.LogWarning("platform found in IGDB database.");
                    return null;
                }

                var platform = platforms.FirstOrDefault();

                if (platform == null)
                {
                    return null;
                }

                return await GetPlatformMetadataAsync(platform, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the metadata.");
                throw;
            }
        }

        private async Task<PlatformMetadata?> GetPlatformMetadataAsync(IGDBModels.Platform platform, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Mapping platform to Platform Metadata...");
            return new PlatformMetadata
            {
                Id = platform.Id?.ToString() ?? string.Empty,
                Name = platform.Name?.RemoveTrademarks() ?? string.Empty,
                Description = platform.Summary?.ToString() ?? string.Empty,
                Logo = platform.PlatformLogo?.Value != null ? $"https:{ImageHelper.GetImageUrl(imageId: platform.PlatformLogo.Value.ImageId, ImageSize.CoverBig, retina: true)}" : null,
            };
        }

        public async Task<IEnumerable<PlatformMetadata?>> GetAllPlatformMetadataAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching Platform Metadata...");

                ICollection<PlatformMetadata?> platformsMetadata = [];

                var platformsCount = await _client.CountAsync(IGDBClient.Endpoints.Platforms, $"fields id;");

                for (var i = 0; platformsCount.Count > i; i += 500)
                {
                    var platforms = await _client.QueryAsync<IGDBModels.Platform>(IGDBClient.Endpoints.Platforms, $"fields abbreviation,alternative_name,category,checksum,created_at,generation,name,platform_family,platform_logo.image_id,slug,summary,updated_at,url,versions,websites; limit 500;");

                    if (platforms == null)
                    {
                        _logger.LogWarning("platform found in IGDB database.");
                        return null;
                    }

                    foreach (var platform in platforms)
                    {
                        platformsMetadata.Add(await GetPlatformMetadataAsync(platform, cancellationToken));
                    }
                }

                return platformsMetadata;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the metadata.");
                throw;
            }
        }
    }
}
