using Spectre.Console.Rendering;

namespace HeroesDataParser.Infrastructure;

public class ResultSummaryService : IResultSummaryService
{
    private readonly ILogger<ResultSummaryService> _logger;
    private readonly RootOptions _options;

    private readonly List<SummaryDataItem> _summaryDataItems = [];
    private readonly List<SummaryImageItem> _summaryImageItems = [];

    public ResultSummaryService(ILogger<ResultSummaryService> logger, IOptions<RootOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public int JsonDataFilesWritten { get; set; }

    public int JsonDataFilesTotal { get; set; }

    public int ImageFilesWritten { get; set; }

    public int ImageFilesTotal { get; set; }

    public int GameStringFilesWritten { get; set; }

    public int GameStringFilesTotal { get; set; }

    public void AddSummaryDataItem(string dataType, int parsedCount, int totalCount, StormLocale stormLocale, string? mapName = null)
    {
        SummaryDataItem item = new()
        {
            Name = dataType,
            MapName = mapName,
            Locale = stormLocale,
            ParseCount = (parsedCount, totalCount),
        };

        _logger.LogDebug("Adding summary data item: {@SummaryItem}", item);

        _summaryDataItems.Add(item);
    }

    public void AddSummaryImageItem(string dataType, int parsedCount, int totalCount)
    {
        SummaryImageItem item = new()
        {
            Name = dataType,
            ParseCount = (parsedCount, totalCount),
        };

        _logger.LogDebug("Adding summary image item: {@SummaryItem}", item);

        _summaryImageItems.Add(item);
    }

    public void PrintSummary()
    {
        List<IRenderable> renderables = [];

        RenderDataSummary(renderables);
        RenderImageSummary(renderables);
        FileCountWrittenSummary(renderables);

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Panel(new Rows(renderables))
        {
            Border = BoxBorder.Square,
            Header = new PanelHeader("Result Summary"),
            Padding = new Padding(1, 1, 1, 1),
        });

        AnsiConsole.WriteLine();
    }

    private void RenderDataSummary(List<IRenderable> renderables)
    {
        if (_summaryDataItems.Count == 0)
            return;

        List<SummaryDataItem> warningDataItems = [.. _summaryDataItems.Where(x => x.ParseCount.Parsed < x.ParseCount.Total)];

        if (warningDataItems.Count == 0)
        {
            renderables.Add(new Markup("[bold green]All data parsed successfully[/]"));
        }
        else
        {
            renderables.Add(new Markup($"[bold yellow]:warning:  Has unsuccessful data parsing[/]"));
            renderables.Add(DataPrintSummary(warningDataItems));
        }
    }

    private void RenderImageSummary(List<IRenderable> renderables)
    {
        if (_summaryImageItems.Count == 0)
            return;

        List<SummaryImageItem> warningImageItems = [.. _summaryImageItems.Where(x => x.ParseCount.Parsed < x.ParseCount.Total)];

        if (warningImageItems.Count == 0)
        {
            renderables.Add(new Markup($"{Environment.NewLine}[bold green]All images parsed successfully[/]"));
        }
        else
        {
            renderables.Add(new Markup($"{Environment.NewLine}[bold yellow]Has unsuccessful image parsing[/]"));
            renderables.Add(ImagePrintSummary(warningImageItems));
        }
    }

    private void FileCountWrittenSummary(List<IRenderable> renderables)
    {
        renderables.Add(new Markup($"{Environment.NewLine}Files Written:"));

        if (JsonDataFilesWritten < JsonDataFilesTotal)
            renderables.Add(new Markup($"  [SteelBlue1_1]data[/]: [[[yellow]:warning:  {JsonDataFilesWritten}/{JsonDataFilesTotal}[/]]]"));
        else
            renderables.Add(new Markup($"  [SteelBlue1_1]data[/]: [[[green]{JsonDataFilesWritten}/{JsonDataFilesTotal}[/]]]"));

        if (GameStringFilesWritten < GameStringFilesTotal)
            renderables.Add(new Markup($"  [SteelBlue1_1]gamestrings[/]: [[[yellow]:warning:  {GameStringFilesWritten}/{GameStringFilesTotal}[/]]]"));
        else
            renderables.Add(new Markup($"  [SteelBlue1_1]gamestrings[/]: [[[green]{GameStringFilesWritten}/{GameStringFilesTotal}[/]]]"));

        if (ImageFilesWritten < ImageFilesTotal)
            renderables.Add(new Markup($"  [SteelBlue1_1]images[/]: [[[yellow]:warning:  {ImageFilesWritten}/{ImageFilesTotal}[/]]]"));
        else
            renderables.Add(new Markup($"  [SteelBlue1_1]images[/]: [[[green]{ImageFilesWritten}/{ImageFilesTotal}[/]]]"));
    }

    private Grid DataPrintSummary(IEnumerable<SummaryDataItem> warningDataItems)
    {
        _logger.LogDebug("Preparing data summary grid for items: {@WarningDataItems}", warningDataItems);

        Grid grid = new();
        grid.AddColumns(5);

        foreach (SummaryDataItem item in warningDataItems)
        {
            Text[] columns = new Text[grid.Columns.Count];

            if (item.Locale is not null)
                columns[0] = new Text($"{item.Locale}");
            else
                columns[0] = new Text("NOLG");

            if (!string.IsNullOrEmpty(item.MapName))
                columns[1] = new Text($"{item.MapName}");
            else
                columns[1] = new Text("(core)");

            columns[2] = new Text($"{item.Name}");
            columns[3] = new Text($"{item.ParseCount.Parsed,6} / {item.ParseCount.Total}");
            columns[4] = new Text($"({item.ParseCount.Total - item.ParseCount.Parsed})").Centered();

            grid.AddRow(columns);
        }

        return grid;
    }

    private Grid ImagePrintSummary(IEnumerable<SummaryImageItem> warningImageItems)
    {
        _logger.LogDebug("Preparing image summary grid for items: {@WarningImageItems}", warningImageItems);

        Grid grid = new();
        grid.AddColumns(3);

        foreach (SummaryImageItem item in warningImageItems)
        {
            Text[] columns = new Text[grid.Columns.Count];

            columns[0] = new Text($"{item.Name}");
            columns[1] = new Text($"{item.ParseCount.Parsed,6} / {item.ParseCount.Total}");
            columns[2] = new Text($"({item.ParseCount.Total - item.ParseCount.Parsed})").Centered();

            grid.AddRow(columns);
        }

        return grid;
    }
}
