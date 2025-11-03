namespace HeroesDataParser.Core;

public interface IResultSummaryService
{
    void AddSummaryDataItem(string dataType, int parsedCount, int totalCount, StormLocale stormLocale, string? mapName = null);

    void AddSummaryImageItem(string dataType, int parsedCount, int totalCount);

    void PrintSummary();
}
