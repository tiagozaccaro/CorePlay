using CorePlay.SDK.Extensions;
using CorePlay.SDK.Models;
using CorePlay.SDK.Providers;
using Microsoft.Extensions.Logging;

namespace CorePlay.Plugins.Library.SteamLibrary
{
    public class SteamLibraryProvider : ILibraryProvider
    {
        private readonly SteamClient _client;
        private readonly ILogger<SteamLibraryProvider> _logger;

        public SteamLibraryProvider(SteamClient client, ILogger<SteamLibraryProvider> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves the list of games from the Steam library.
        /// </summary>
        /// <returns>A collection of GameMetadata objects representing the games in the Steam library.</returns>
        public async Task<IEnumerable<GameMetadata>> GetGamesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching Steam library...");
                var games = await _client.GetSteamLibraryAsync();

                if (games == null)
                {
                    _logger.LogWarning("No games found in Steam library.");
                    return [];
                }

                _logger.LogInformation("Mapping games to Game Metadata...");
                return games.Select(g => new GameMetadata
                {
                    Source = new MetadataNameProperty("Steam"),
                    Id = g.AppId.ToString(),
                    Name = g.Name.RemoveTrademarks(),
                    Platforms = new HashSet<MetadataProperty> { new MetadataSpecProperty("pc_windows") },
                    Playtime = (ulong)(g.PlaytimeForever * 60)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the Steam library.");
                throw;
            }
        }
    }
}
