namespace CorePlay.SDK.Models.Metadata
{
    public class GameMetadata
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ulong Playtime { get; set; }
        public DateTime? LastActivity { get; set; }
        public string? Icon { get; set; }
        public string? Cover { get; set; }
        public string? Logo { get; set; }
        public ICollection<string> Artworks { get; set; } = [];
        public ICollection<string> Videos { get; set; } = [];
        public HashSet<MetadataProperty> Platforms { get; set; } = [];
        public MetadataProperty? Source { get; set; }
    }
}
