namespace HeroesDataParser.Infrastructure.JsonFileWriters;

public class JsonDataFileWriterService : IJsonDataFileWriterService
{
    private const string _jsonFileDirectory = "data";

    private readonly ILogger<JsonDataFileWriterService> _logger;
    private readonly RootOptions _options;
    private readonly ISerializedElementsService _serializedElementsService;
    private readonly IJsonSerializerOptionService _jsonSerializerOptionService;
    private readonly IResultSummaryService _resultSummaryService;

    public JsonDataFileWriterService(
        ILogger<JsonDataFileWriterService> logger,
        IOptions<RootOptions> options,
        ISerializedElementsService serializedElementsService,
        IJsonSerializerOptionService jsonSerializerOptionService,
        IResultSummaryService resultSummaryService)
    {
        _logger = logger;
        _options = options.Value;
        _serializedElementsService = serializedElementsService;
        _jsonSerializerOptionService = jsonSerializerOptionService;
        _resultSummaryService = resultSummaryService;
    }

    public async Task Write<TElement>(SortedDictionary<string, TElement> elementsById)
        where TElement : IElementObject
    {
        if (!IsSerializationRequired(elementsById.Count))
            return;

        await WriteTo(elementsById, _jsonFileDirectory);
    }

    // write to the maps sub directory
    public async Task WriteToMaps<TElement>(string mapDirectory, SortedDictionary<string, TElement> elementsById)
        where TElement : IElementObject
    {
        if (!IsSerializationRequired(elementsById.Count))
            return;

        Span<char> buffer = stackalloc char[mapDirectory.Length];
        int length = NormalizeMapDirectoryName(buffer, mapDirectory);

        await WriteTo(elementsById, Path.Join(_jsonFileDirectory, "maps", buffer[..length]), mapDirectory);
    }

    private static int NormalizeMapDirectoryName(Span<char> buffer, string mapDirectory)
    {
        int index = 0;

        foreach (char c in mapDirectory)
        {
            if (char.IsWhiteSpace(c))
                buffer[index++] = '_';
            else if (!char.IsPunctuation(c))
                buffer[index++] = char.ToLowerInvariant(c);
        }

        return index;
    }

    private bool IsSerializationRequired(int count)
    {
        if (count > 0)
        {
            _logger.LogInformation("{Count} items to serialize", count);
            return true;
        }
        else
        {
            _logger.LogInformation("No items to serialize");
            return false;
        }
    }

    private async Task WriteTo<TElement>(SortedDictionary<string, TElement> elementsById, string innerDirectory, string? mapName = null)
        where TElement : IElementObject
    {
        string dataName = $"{typeof(TElement).Name}data".ToLowerInvariant();

        RootDataElement<TElement> rootDataElement = new()
        {
            Meta = new()
            {
                DataType = dataName,
                MapName = mapName,
                LocalizedText = _options.LocalizedText,
                HeroesVersion = _options.HeroesVersion.GetAsHeroesDataVersion(),
                HdpVersion = _options.AppVersion,
                DescriptionText = _options.LocalizedText == LocalizedTextOption.Extract ? null : new()
                {
                    Locale = _options.CurrentLocale,
                    GameStringTextType = _options.GameStringText.Type,
                    ReplaceFontStyles = _options.GameStringText.ReplaceFontStyles,
                    PreserveFontStyleConstantVars = _options.GameStringText.PreserveFont.PreserveFontStyleConstantVars,
                    PreserveFontStyleVars = _options.GameStringText.PreserveFont.PreserveFontStyleVars,
                },
                TotalItems = elementsById.Count,
            },
            Items = elementsById,
        };

        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(rootDataElement, _jsonSerializerOptionService.JsonSerializerDataOptions);

        if (string.IsNullOrWhiteSpace(mapName))
            await WriteBaseJsonFile(innerDirectory, dataName, bytes);
        else
            await WriteSubMapJsonFile(innerDirectory, mapName, dataName, bytes);
    }

    private async Task WriteBaseJsonFile(string innerDirectory, string dataName, byte[] bytes)
    {
        string fileName = GetFileName(dataName);
        string filePath = GetFilePath(innerDirectory, fileName);

        _logger.LogInformation("Writing to {FilePath}", filePath);

        try
        {
            _resultSummaryService.JsonDataFilesTotal++;

            await using FileStream fileStream = File.Create(filePath);
            await fileStream.WriteAsync(bytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing json file {FilePath}", filePath);
            return;
        }

        _resultSummaryService.JsonDataFilesWritten++;
        _serializedElementsService.AddSerializedElements(dataName, bytes);

        AnsiConsole.Write("Created file ");
        AnsiConsoleHelpers.WriteFilePath(Path.Join(innerDirectory, fileName));
        AnsiConsole.WriteLine();
    }

    private async Task WriteSubMapJsonFile(string innerDirectory, string mapName, string dataName, byte[] bytes)
    {
        await WriteNormalMap(innerDirectory, mapName, dataName, bytes);
        await WriteMapDiff(innerDirectory, mapName, dataName, bytes);
    }

    private async Task WriteNormalMap(string innerDirectory, string mapName, string dataName, byte[] bytes)
    {
        if (!_options.MapWriterJsonOutputType.HasFlag(MapWriterJsonOutputType.Normal))
            return;

        string fileName = GetFileName(dataName);
        string filePath = GetFilePath(innerDirectory, fileName);

        _logger.LogInformation("Writing normal json file {FilePath} for map {MapName}", filePath, mapName);

        try
        {
            _resultSummaryService.JsonDataFilesTotal++;

            await using FileStream fileStream = File.Create(filePath);
            await fileStream.WriteAsync(bytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing json file {FilePath} for map {MapName}", filePath, mapName);
            return;
        }

        _resultSummaryService.JsonDataFilesWritten++;

        AnsiConsole.Write("Created file ");
        AnsiConsoleHelpers.WriteFilePath(Path.Join(innerDirectory, fileName));
        AnsiConsole.WriteLine();
    }

    private async Task WriteMapDiff(string innerDirectory, string mapName, string dataName, byte[] bytes)
    {
        if (!_options.MapWriterJsonOutputType.HasFlag(MapWriterJsonOutputType.Diff))
            return;

        string fileName = $"{GetFileName(dataName)}.diff";
        string filePath = GetFilePath(innerDirectory, fileName);

        _logger.LogInformation("Writing diff json file {FilePath} for map {MapName}", filePath, mapName);

        await WriteDiffJsonFile(innerDirectory, mapName, dataName, fileName, filePath, bytes);
    }

    private async Task WriteDiffJsonFile(string innerDirectory, string mapName, string dataName, string fileName, string filePath, byte[] bytes)
    {
        JsonNode? jsonDiff = _serializedElementsService.GetJsonNodeDiff(dataName, bytes);
        JsonNode? metaNode = jsonDiff?["items"]; // null means that the items are the same

        int totalItemsChanged = metaNode?.AsObject()?.Count ?? 0;

        _logger.LogInformation("Found {TotalItems} changed items of {DataType} for map {MapName}", dataName, totalItemsChanged, mapName);

        try
        {
            _resultSummaryService.JsonDataFilesTotal++;

            await using FileStream fileStream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(fileStream, jsonDiff, _jsonSerializerOptionService.JsonSerializerDataOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing diff json file {FilePath} for map {MapName}", filePath, mapName);
            return;
        }

        _resultSummaryService.JsonDataFilesWritten++;

        AnsiConsole.Write("Created diff file ");
        AnsiConsoleHelpers.WriteFilePath(Path.Join(innerDirectory, fileName));

        if (totalItemsChanged < 1)
            AnsiConsole.Write($" [0]");
        else
            AnsiConsole.Markup($" [[[yellow]{totalItemsChanged}[/]]]");

        AnsiConsole.WriteLine();
    }

    private string GetFileName(string dataName)
    {
        return $"{dataName}_{_options.BuildNumber ?? 0}_{_options.CurrentLocale}.json".ToLowerInvariant();
    }

    private string GetFilePath(string innerDirectory, string fileName)
    {
        string directory = Path.Combine(_options.OutputDirectory, innerDirectory);
        Directory.CreateDirectory(directory);

        return Path.Combine(directory, fileName);
    }

    private string GetDiffExtension(string? mapName)
    {
        if (!string.IsNullOrWhiteSpace(mapName) && _options.MapWriterJsonOutputType.HasFlag(MapWriterJsonOutputType.Diff))
            return ".diff";

        return string.Empty;
    }
}
