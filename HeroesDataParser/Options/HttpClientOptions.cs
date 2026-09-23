namespace HeroesDataParser.Options;

public class HttpClientOptions
{
    // zero means infinite timeout
    public int TimeoutSeconds { get; set; } = 30;
}
