namespace CorePlay.Plugins.Metadata.ScreenScraper;

// Model for game information
public class GameInfo
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ReleaseDate { get; set; }
    public string Developer { get; set; }
    public string Publisher { get; set; }
    public string Genre { get; set; }
    public string SystemId { get; set; }
    public string Region { get; set; }
    public List<MediaInfo> Media { get; set; }
    // Add other properties as per the actual API response
}

// Model for system information
public class SystemInfo
{
    public string SystemId { get; set; }
    public string Name { get; set; }
    public string Manufacturer { get; set; }
    public string ReleaseDate { get; set; }
    public string Description { get; set; }
    public List<string> SupportedMedia { get; set; }
    // Add other properties as per the actual API response
}

// Model for media information
public class MediaInfo
{
    public string MediaId { get; set; }
    public string Type { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
    // Add other properties as per the actual API response
}

// Model for a list of systems
public class SystemList
{
    public List<SystemInfo> Systems { get; set; }
}

// Model for game search results
public class GameSearchResult
{
    public List<GameInfo> Games { get; set; }
}

// Model for genre information
public class Genre
{
    public string GenreId { get; set; }
    public string Name { get; set; }
    // Add other properties as per the actual API response
}

// Model for a list of genres
public class GenreList
{
    public List<Genre> Genres { get; set; }
}

// Model for media type information
public class MediaType
{
    public string MediaTypeId { get; set; }
    public string Name { get; set; }
    // Add other properties as per the actual API response
}

// Model for a list of media types
public class MediaTypeList
{
    public List<MediaType> MediaTypes { get; set; }
}

// Model for language information
public class Language
{
    public string LanguageId { get; set; }
    public string Name { get; set; }
    // Add other properties as per the actual API response
}

// Model for a list of languages
public class LanguageList
{
    public List<Language> Languages { get; set; }
}