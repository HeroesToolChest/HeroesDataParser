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

        AnsiConsole.Write(new Panel(new Rows(renderables))
        {
            Border = BoxBorder.Ascii,
            Header = new PanelHeader("Result Summary"),
            Padding = new Padding(1, 1, 1, 1),
        });

        AnsiConsole.WriteLine();
    }

    private void RenderDataSummary(List<IRenderable> renderables)
    {
        List<SummaryDataItem> warningDataItems = [.. _summaryDataItems.Where(x => x.ParseCount.Parsed < x.ParseCount.Total)];

        if (warningDataItems.Count == 0)
        {
            renderables.Add(new Markup("[bold green]All data parsed successfully[/]"));
        }
        else
        {
            renderables.Add(new Markup($"[bold yellow]Has unsuccessful data parsing[/]"));
            renderables.Add(DataPrintSummary(warningDataItems));
        }
    }

    private void RenderImageSummary(List<IRenderable> renderables)
    {
        List<SummaryImageItem> warningImageItems = [.. _summaryImageItems.Where(x => x.ParseCount.Parsed < x.ParseCount.Total)];

        if (warningImageItems.Count == 0)
        {
            renderables.Add(new Markup("\n[bold green]All images parsed successfully[/]"));
        }
        else
        {
            renderables.Add(new Markup($"\n[bold yellow]Has unsuccessful image parsing[/]"));
            renderables.Add(ImagePrintSummary(warningImageItems));
        }
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
