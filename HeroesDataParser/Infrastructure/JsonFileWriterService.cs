using System.Text.Json.Nodes;

namespace HeroesDataParser.Infrastructure;

public class JsonFileWriterService : IJsonFileWriterService
{
    private const string _jsonFileDirectory = "data";

    private readonly ILogger<JsonFileWriterService> _logger;
    private readonly RootOptions _options;
    private readonly HeroesData _heroesData;
    private readonly ISerializedElementsService _serializedElementsService;
    private readonly IJsonSerializerOptionService _jsonSerializerOptionService;

    public JsonFileWriterService(
        ILogger<JsonFileWriterService> logger,
        IOptions<RootOptions> options,
        IHeroesXmlLoaderService heroesXmlLoaderService,
        ISerializedElementsService serializedElementsService,
        IJsonSerializerOptionService jsonSerializerOptionService)
    {
        _logger = logger;
        _options = options.Value;
        _heroesData = heroesXmlLoaderService.HeroesXmlLoader.HeroesData;
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
        int length = SanitizeMapDirectory(buffer, mapDirectory);

        await WriteTo(elementsById, Path.Join(_jsonFileDirectory, "maps", buffer[..length]), mapDirectory);
    }

    private static int SanitizeMapDirectory(Span<char> buffer, string mapDirectory)
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
        AnsiConsole.Write(new TextPath(Path.Join(innerDirectory, fileName))
            .SeparatorColor(Color.SpringGreen1)
            .StemColor(Color.SteelBlue1_1)
            .LeafColor(Color.Orange1));
        AnsiConsole.WriteLine();
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
        string fullOutputDirectory = Path.Join(_options.OutputDirectory, innerDirectory);

        Directory.CreateDirectory(fullOutputDirectory);

        string dataName = $"{typeof(TElement).Name}data".ToLowerInvariant();
        string fileName = $"{dataName}_{_heroesData.Build ?? 0}_{_options.CurrentLocale}.json{(!string.IsNullOrWhiteSpace(mapName) ? ".diff" : string.Empty)}".ToLowerInvariant();
        string filePath = Path.Join(fullOutputDirectory, fileName);

        _logger.LogInformation("Writing to {FilePath}", filePath);

        RootElement<TElement> rootElement = new()
        {
            Meta =
            {
                DataType = dataName,
                MapName = mapName,
                IsLocalizedText = _options.LocalizedText,
                // TODO Version
                HdpVersion = AppVersion.GetAppVersion(),
                DescriptionText = _options.LocalizedText ? null : new()
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

        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(rootElement, _jsonSerializerOptionService.HeroesJsonSerializerOptions);

        if (string.IsNullOrWhiteSpace(mapName))
            await WriteBaseJsonFile(innerDirectory, dataName, fileName, filePath, bytes);
        else
            WriteSubMapJsonFile(innerDirectory, mapName, dataName, fileName, filePath, bytes);
    }

    private async Task WriteBaseJsonFile(string innerDirectory, string dataName, string fileName, string filePath, byte[] bytes)
    {
        await using FileStream fileStream = File.Create(filePath);

        await fileStream.WriteAsync(bytes);

        _serializedElementsService.AddSerializedElements(dataName, bytes);

        AnsiConsole.Write("Created file ");
        DisplayCreatedFilePath(innerDirectory, fileName);
    }

    private void WriteSubMapJsonFile(string innerDirectory, string mapName, string dataName, string fileName, string filePath, byte[] bytes)
    {
        JsonNode? jsonDiff = _serializedElementsService.GetJsonNodeDiff(dataName, bytes);
        JsonNode? metaNode = jsonDiff?["items"];

        if (metaNode is null)
        {
            AnsiConsole.MarkupLineInterpolated($"[yellow]No changed items[/]");
            AnsiConsole.WriteLine();

            return;
        }

        int totalItems = metaNode.AsObject().Count;

        _logger.LogInformation("Found {TotalItems} changed items of {DataType} for map {MapName}", dataName, totalItems, mapName);
        AnsiConsole.WriteLine($"{totalItems,6} changed items");

        using FileStream fileStream = File.Create(filePath);
        JsonSerializer.Serialize(fileStream, jsonDiff, _jsonSerializerOptionService.HeroesJsonSerializerOptions);

        AnsiConsole.Write("Created diff file ");
        DisplayCreatedFilePath(innerDirectory, fileName);
    }
}
