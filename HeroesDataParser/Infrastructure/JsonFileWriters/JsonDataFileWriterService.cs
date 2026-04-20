namespace HeroesDataParser.Infrastructure.JsonFileWriters;

public class JsonDataFileWriterService : JsonFileWriterBase, IJsonDataFileWriterService
{
    public JsonDataFileWriterService(
        ILogger<JsonDataFileWriterService> logger,
        IOptions<RootOptions> options,
        IAnsiConsole console,
        ISerializedDataStoreService serializedElementsService,
        IJsonSerializerOptionService jsonSerializerOptionService,
        IResultSummaryService resultSummaryService)
        : base(logger, options, console, serializedElementsService, jsonSerializerOptionService, resultSummaryService)
    {
    }

    public async Task Write<TElement>(SortedDictionary<string, TElement> elementsById)
        where TElement : IElementObject
    {
        if (!IsSerializationRequired(elementsById.Count))
            return;

        await WriteTo(elementsById, Constants.JsonDataDirectory);
    }

    // write to the maps sub directory
    public async Task WriteToMapSpecific<TElement>(string mapDirectory, SortedDictionary<string, TElement> elementsById)
        where TElement : IElementObject
    {
        if (Options.MapSpecificWriterJsonOutputType == MapSpecificWriterJsonOutputType.None)
        {
            Logger.LogInformation($"{nameof(MapSpecificWriterJsonOutputType)} is set to None, skipping writing json data for maps");
            return;
        }

        if (!IsSerializationRequired(elementsById.Count))
            return;

        Span<char> buffer = stackalloc char[mapDirectory.Length];
        int length = NormalizeMapDirectoryName(buffer, mapDirectory);

        await WriteTo(elementsById, Path.Join(Constants.JsonDataDirectory, Constants.MapDirectory, buffer[..length]), mapDirectory);
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

        if (!Enum.TryParse(dataName, true, out DataType dataType))
        {
            Logger.LogWarning("Unable to parse data type from data name {DataName}, defaulting to Unknown", dataName);
            dataType = DataType.Unknown;
        }

        RootDataElement<TElement> rootDataElement = new()
        {
            Meta = new()
            {
                ItemsType = ItemsType.Data,
                DataType = dataType,
                MapName = mapName,
                LocalizedText = Options.LocalizedText,
                HeroesVersion = Options.HeroesVersion.GetAsHeroesDataVersion(),
                HdpVersion = Options.AppVersion,
                GameStringTextProperties = Options.LocalizedText == LocalizedTextOption.Extract ? null : new()
                {
                    Locale = Options.CurrentLocale,
                    GameStringTextType = Options.GameStringText.Type,
                    ReplaceFontConstantVars = Options.GameStringText.ReplaceFontConstantVars,
                    PreserveFontStyleConstantVars = Options.GameStringText.PreserveFontStyleConstantVars,
                    ReplaceFontStylesVars = Options.GameStringText.ReplaceFontStylesVars,
                    PreserveFontStyleVars = Options.GameStringText.PreserveFontStyleVars,
                },
                TotalItems = elementsById.Count,
            },
            Items = elementsById,
        };

        // serializing triggers the custom TypeInfoResolver which handles the saving of the gamestrings
        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(rootDataElement, JsonSerializerOptionService.JsonSerializerDataOptions);

        // this must be done after the serialization
        if (!ShouldWrite())
            return;

        if (isMap)
            await WriteSubMapJsonFile(innerDirectory, mapName!, dataType, bytes, false);
        else
            await WriteBaseJsonFile(innerDirectory, dataType, bytes, false);
    }

    private bool ShouldWrite()
    {
        return Options.LocalizedText != LocalizedTextOption.Extract || Options.IsLocalizedExtractFirstRun;
    }
}
