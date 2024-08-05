using CorePlay.Plugins.Library.SteamLibrary.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace CorePlay.Plugins.Library.SteamLibrary
{
    public class SteamClient
    {
        private readonly string _steamId;
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly ILogger<SteamClient> _logger;

        public SteamClient(string steamId, string apiKey, HttpClient httpClient, ILogger<SteamClient> logger)
        {
            _steamId = steamId ?? throw new ArgumentNullException(nameof(steamId));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves the list of owned games from the Steam API.
        /// </summary>
        /// <param name="includeFreeSub">Whether to include free games in the response.</param>
        /// <returns>A list of SteamGame objects representing the user's owned games.</returns>
        public async Task<List<SteamGame>> GetSteamLibraryAsync(bool includeFreeSub = false)
        {
            try
            {
                var url = $"https://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={_apiKey}&include_appinfo=1&include_played_free_games=1&format=json&steamid={_steamId}&skip_unvetted_apps=0&include_free_sub={(includeFreeSub ? "1" : "0")}";

                _logger.LogInformation($"Fetching Steam library from URL: {url}");
                var response = await _httpClient.GetFromJsonAsync<SteamApiResponse>(url);

                if (response?.Response?.Games == null)
                {
                    _logger.LogWarning("No games found in the Steam library response.");
                    return [];
                }

                _logger.LogInformation($"Retrieved {response.Response.Games.Count} games from Steam library.");
                return response.Response.Games;
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "An HTTP error occurred while fetching the Steam library.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the Steam library.");
                throw;
            }
        }
    }
}
