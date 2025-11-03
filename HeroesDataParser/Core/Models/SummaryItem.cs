namespace HeroesDataParser.Core.Models;

public class SummaryItem
{
    public required string Name { get; init; }

    public required (int Parsed, int Total) ParseCount { get; init; }
}
