using System.Text.Json.Serialization;

namespace CorePlay.Plugins.Library.SteamLibrary.Models
{
    public class SteamGameResponse
    {
        [JsonPropertyName("game_count")]
        public int GameCount { get; set; }

        [JsonPropertyName("games")]
        public List<SteamGame> Games { get; set; }
    }
}
