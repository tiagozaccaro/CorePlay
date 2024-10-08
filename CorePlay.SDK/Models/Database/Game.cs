﻿namespace CorePlay.SDK.Models.Database
{
    public class Game
    {
        public string GameId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ulong Playtime { get; set; }
        public DateTime? LastActivity { get; set; }
        public string? Icon { get; set; }
        public string? Cover { get; set; }
        public string? Logo { get; set; }
        public ICollection<string> Artworks { get; set; } = [];
        public ICollection<string> Videos { get; set; } = [];
        public HashSet<Platform> Platforms { get; set; } = [];
        public Source? Source { get; set; }
    }
}
