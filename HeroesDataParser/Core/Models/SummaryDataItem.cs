namespace HeroesDataParser.Core.Models;

public class SummaryDataItem : SummaryItem
{
    public required string? MapName { get; init; }

    public required StormLocale? Locale { get; init; }
}
