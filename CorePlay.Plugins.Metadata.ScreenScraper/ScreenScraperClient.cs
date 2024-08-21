using System.Text.Json;

namespace CorePlay.Plugins.Metadata.ScreenScraper
{
    public class ScreenScraperClient(string devId, string devPassword, string userId, string userPassword)
    {
        private readonly HttpClient _httpClient = new();
        private readonly string _baseUrl = "https://www.screenscraper.fr/api2/";

        private readonly string _devId = devId;
        private readonly string _devPassword = devPassword;
        private readonly string _userId = userId;
        private readonly string _userPassword = userPassword;

        private async Task<T> GetApiResponseAsync<T>(string endpoint, string parameters) where T : class
        {
            var url = $"{_baseUrl}{endpoint}?devid={_devId}&devpassword={_devPassword}&ssid={_userId}&sspassword={_userPassword}&{parameters}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // Get game information
        public async Task<GameInfo> GetGameInfoAsync(string gameName)
        {
            return await GetApiResponseAsync<GameInfo>("jeuInfos.php", $"recherche={gameName}");
        }

        // Get system information
        public async Task<SystemInfo> GetSystemInfoAsync(string systemId)
        {
            return await GetApiResponseAsync<SystemInfo>("systemeInfos.php", $"systemeid={systemId}");
        }

        // Get media for a specific game
        public async Task<MediaInfo> GetGameMediaAsync(string gameId, string mediaType = "screenshot")
        {
            return await GetApiResponseAsync<MediaInfo>("media.php", $"jeuId={gameId}&media={mediaType}");
        }

        // Get list of systems
        public async Task<SystemList> GetSystemListAsync()
        {
            return await GetApiResponseAsync<SystemList>("systemesListe.php", "");
        }

        // Search for games by name
        public async Task<GameSearchResult> SearchGamesAsync(string gameName)
        {
            return await GetApiResponseAsync<GameSearchResult>("jeuxRecherche.php", $"recherche={gameName}");
        }

        // Get game region information
        public async Task<GameInfo> GetGameRegionInfoAsync(string gameId, string region)
        {
            return await GetApiResponseAsync<GameInfo>("jeuInfosRegion.php", $"jeuId={gameId}&region={region}");
        }

        // Get a list of genres
        public async Task<GenreList> GetGenreListAsync()
        {
            return await GetApiResponseAsync<GenreList>("genresListe.php", "");
        }

        // Get a list of media types available
        public async Task<MediaTypeList> GetMediaListAsync()
        {
            return await GetApiResponseAsync<MediaTypeList>("mediaTypesListe.php", "");
        }

        // Get a list of the languages supported for games
        public async Task<LanguageList> GetLanguagesListAsync()
        {
            return await GetApiResponseAsync<LanguageList>("languagesListe.php", "");
        }
    }
}
