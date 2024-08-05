using CorePlay.SDK.Extensions;
using CorePlay.SDK.Interfaces.Providers;
using CorePlay.SDK.Models;
using IGDB;
using Microsoft.Extensions.Logging;
using IGDBModels = IGDB.Models;

namespace CorePlay.Plugins.Metadata.IGDB
{
    public class IGDBMetadataProvider : IMetadataProvider
    {
        private readonly IGDBClient _client;
        private readonly ILogger<IGDBMetadataProvider> _logger;

        public IGDBMetadataProvider(IGDBClient client, ILogger<IGDBMetadataProvider> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<GameMetadata?> GetGameMetadataAsync(string name)
        {
            try
            {
                _logger.LogInformation("Fetching Game Metadata...");
                var games = await _client.QueryAsync<IGDBModels.Game>(IGDBClient.Endpoints.Games, $"fields id,name,cover.image_id,artworks.image_id,videos.video_id; where name = \"{name}\";");

                if (games == null)
                {
                    _logger.LogWarning("game found in IGDB database.");
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
                    GameId = game.Id?.ToString() ?? string.Empty,
                    Name = game.Name?.RemoveTrademarks() ?? string.Empty,
                    Cover = game.Cover?.Value != null ? $"https:{ImageHelper.GetImageUrl(imageId: game.Cover.Value.ImageId, ImageSize.CoverSmall, retina: false)}" : null,
                    Videos = game.Videos?.Values != null ? game.Videos.Values.Select(v => $"https://www.youtube.com/watch?v={v.VideoId}").ToList() : []
                    //Platforms = game.platforms.Select(p => new MetadataIdProperty(p)),
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
