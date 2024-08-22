namespace CorePlay.SDK.Models.Database
{
    public class Platform
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Logo { get; set; }
        public PlatformCategory PlatformCategory { get; set; }
    }
}
