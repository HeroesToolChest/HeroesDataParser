namespace HeroesDataParser.Infrastructure.JsonFileWriters;

public class JsonDataFileWriterService : JsonFileWriterBase, IJsonDataFileWriterService
{
    private const string _jsonFileDirectory = "data";

    public JsonDataFileWriterService(
        ILogger<JsonDataFileWriterService> logger,
        IOptions<RootOptions> options,
        ISerializedDataStoreService serializedElementsService,
        IJsonSerializerOptionService jsonSerializerOptionService,
        IResultSummaryService resultSummaryService)
        : base(logger, options, serializedElementsService, jsonSerializerOptionService, resultSummaryService)
    {
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
        if (Options.MapWriterJsonOutputType == MapWriterJsonOutputType.None)
        {
            Logger.LogInformation($"{nameof(MapWriterJsonOutputType)} is set to None, skipping writing json data for maps");
            return;
        }

        if (!IsSerializationRequired(elementsById.Count))
            return;

        Span<char> buffer = stackalloc char[mapDirectory.Length];
        int length = NormalizeMapDirectoryName(buffer, mapDirectory);

        await WriteTo(elementsById, Path.Join(_jsonFileDirectory, "maps", buffer[..length]), mapDirectory);
    }

    protected override void IncrementFilesTotal()
    {
        ResultSummaryService.JsonDataFilesTotal++;
    }

    protected override void IncrementFilesWritten()
    {
        ResultSummaryService.JsonDataFilesWritten++;
    }

    private bool IsSerializationRequired(int count)
    {
        if (count > 0)
        {
            Logger.LogInformation("{Count} items to serialize", count);
            return true;
        }
        else
        {
            Logger.LogInformation("No items to serialize");
            return false;
        }
    }

    private async Task WriteTo<TElement>(SortedDictionary<string, TElement> elementsById, string innerDirectory, string? mapName = null)
        where TElement : IElementObject
    {
        bool isMap = !string.IsNullOrWhiteSpace(mapName);

        string dataName = $"{typeof(TElement).Name}{Constants.ElementDataSuffix}".ToLowerInvariant();

        RootDataElement<TElement> rootDataElement = new()
        {
            Meta = new()
            {
                DataType = dataName,
                MapName = mapName,
                LocalizedText = Options.LocalizedText,
                HeroesVersion = Options.HeroesVersion.GetAsHeroesDataVersion(),
                HdpVersion = Options.AppVersion,
                DescriptionText = Options.LocalizedText == LocalizedTextOption.Extract ? null : new()
                {
                    Locale = Options.CurrentLocale,
                    GameStringTextType = Options.GameStringText.Type,
                    ReplaceFontStyles = Options.GameStringText.ReplaceFontStyles,
                    PreserveFontStyleConstantVars = Options.GameStringText.PreserveFont.PreserveFontStyleConstantVars,
                    PreserveFontStyleVars = Options.GameStringText.PreserveFont.PreserveFontStyleVars,
                },
                TotalItems = elementsById.Count,
            },
            Items = elementsById,
        };

        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(rootDataElement, JsonSerializerOptionService.JsonSerializerDataOptions);

        if (isMap)
            await WriteSubMapJsonFile(innerDirectory, mapName!, dataName, bytes);
        else
            await WriteBaseJsonFile(innerDirectory, dataName, bytes);
    }
}
