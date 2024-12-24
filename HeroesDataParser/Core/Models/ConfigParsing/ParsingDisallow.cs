namespace HeroesDataParser.Core.Models.ConfigParsing;

public class ParsingDisallow
{
    public HashSet<string> Exact { get; set; } = [];

    public HashSet<string> Regex { get; set; } = [];
}
