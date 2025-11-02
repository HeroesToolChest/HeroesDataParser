namespace HeroesDataParser.Infrastructure.JsonFileWriters;

public class JsonDataFileWriterService : IJsonDataFileWriterService
{
    private const string _jsonFileDirectory = "data";

    private readonly ILogger<JsonDataFileWriterService> _logger;
    private readonly RootOptions _options;
    private readonly ISerializedElementsService _serializedElementsService;
    private readonly IJsonSerializerOptionService _jsonSerializerOptionService;

    public JsonDataFileWriterService(
        ILogger<JsonDataFileWriterService> logger,
        IOptions<RootOptions> options,
        ISerializedElementsService serializedElementsService,
        IJsonSerializerOptionService jsonSerializerOptionService)
    {
        _logger = logger;
        _options = options.Value;
        _serializedElementsService = serializedElementsService;
        _jsonSerializerOptionService = jsonSerializerOptionService;
    }

    public async Task Write<TElement>(Dictionary<string, TElement> elementsById)
        where TElement : IElementObject
    {
        if (!IsSerializationRequired(elementsById.Count))
            return;

        await WriteTo(elementsById, _jsonFileDirectory);
    }

    // write to the maps sub directory
    public async Task WriteToMaps<TElement>(string mapDirectory, Dictionary<string, TElement> elementsById)
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

    private static void DisplayCreatedFilePath(string innerDirectory, string fileName)
    {
        AnsiConsoleHelpers.WriteFilePath(Path.Join(innerDirectory, fileName));
        AnsiConsole.WriteLine();
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

    private async Task WriteTo<TElement>(Dictionary<string, TElement> elementsById, string innerDirectory, string? mapName = null)
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
                DescriptionText = _options.LocalizedText == LocalizedTextOption.None ? null : new()
                {
                    Locale = _options.CurrentLocale,
                    GameStringTextType = _options.GameStringText.Type,
                    ReplaceFontStyles = _options.GameStringText.ReplaceFontStyles,
                    PreserveFontStyleConstantVars = _options.GameStringText.PreserveFont.PreserveFontStyleConstantVars,
                    PreserveFontStyleVars = _options.GameStringText.PreserveFont.PreserveFontStyleVars,
                },
                TotalItems = elementsById.Count,
            },
            Items = new SortedDictionary<string, TElement>(elementsById, StringComparer.Ordinal),
        };

        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(rootDataElement, _jsonSerializerOptionService.JsonSerializerDataOptions);

        if (string.IsNullOrWhiteSpace(mapName))
            await WriteBaseJsonFile(innerDirectory, dataName, bytes);
        else
            await WriteSubMapJsonFile(innerDirectory, mapName, dataName, bytes);

        AnsiConsole.WriteLine();
    }

    private async Task WriteBaseJsonFile(string innerDirectory, string dataName, byte[] bytes)
    {
        string fileName = GetFileName(dataName);
        string filePath = GetFilePath(innerDirectory, fileName);

        _logger.LogInformation("Writing to {FilePath}", filePath);

        await using FileStream fileStream = File.Create(filePath);
        await fileStream.WriteAsync(bytes);

        _serializedElementsService.AddSerializedElements(dataName, bytes);

        AnsiConsole.Write("Created file ");
        DisplayCreatedFilePath(innerDirectory, fileName);
    }

    private async Task WriteSubMapJsonFile(string innerDirectory, string mapName, string dataName, byte[] bytes)
    {
        if (_options.MapWriterJsonOutputType.HasFlag(MapWriterJsonOutputType.Normal))
        {
            string fileName = GetFileName(dataName);
            string filePath = GetFilePath(innerDirectory, fileName);

            _logger.LogInformation("Writing normal json file {FilePath} for map {MapName}", filePath, mapName);

            await using FileStream fileStream = File.Create(filePath);
            await fileStream.WriteAsync(bytes);

            AnsiConsole.Write("Created file ");
            DisplayCreatedFilePath(innerDirectory, fileName);
        }

        if (_options.MapWriterJsonOutputType.HasFlag(MapWriterJsonOutputType.Diff))
        {
            string fileName = $"{GetFileName(dataName)}.diff";
            string filePath = GetFilePath(innerDirectory, fileName);

            _logger.LogInformation("Writing diff json file {FilePath} for map {MapName}", filePath, mapName);

            await WriteDiffJsonFile(innerDirectory, mapName, dataName, fileName, filePath, bytes);
        }
        else
        {
            // if none, do nothing
            _logger.LogInformation("No normal/diff json file writing for map {MapName}", mapName);
        }
    }

    private async Task WriteDiffJsonFile(string innerDirectory, string mapName, string dataName, string fileName, string filePath, byte[] bytes)
    {
        JsonNode? jsonDiff = _serializedElementsService.GetJsonNodeDiff(dataName, bytes);
        JsonNode? metaNode = jsonDiff?["items"]; // null means that the items are the same

        int totalItemsChanged = metaNode?.AsObject()?.Count ?? 0;

        _logger.LogInformation("Found {TotalItems} changed items of {DataType} for map {MapName}", dataName, totalItemsChanged, mapName);

        if (totalItemsChanged < 1)
            AnsiConsole.MarkupLineInterpolated($"[yellow]{totalItemsChanged,6} changed items[/]");
        else
            AnsiConsole.WriteLine($"{totalItemsChanged,6} changed items");

        await using FileStream fileStream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(fileStream, jsonDiff, _jsonSerializerOptionService.JsonSerializerDataOptions);

        AnsiConsole.Write("Created diff file ");
        DisplayCreatedFilePath(innerDirectory, fileName);
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
