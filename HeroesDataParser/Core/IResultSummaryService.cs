namespace HeroesDataParser.Core;

public interface IResultSummaryService
{
    int JsonDataFilesWritten { get; set; }

    int JsonDataFilesTotal { get; set; }

    int ImageFilesWritten { get; set; }

    int ImageFilesTotal { get; set; }

    int GameStringFilesWritten { get; set; }

    int GameStringFilesTotal { get; set; }

    void AddSummaryDataItem(string dataType, int parsedCount, int totalCount, StormLocale stormLocale, string? mapName = null);

    void AddSummaryImageItem(string dataType, int parsedCount, int totalCount);

    void PrintSummary();
}
