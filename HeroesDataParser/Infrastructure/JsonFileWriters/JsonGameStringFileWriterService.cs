namespace HeroesDataParser.Infrastructure.JsonFileWriters;

public class JsonGameStringFileWriterService : JsonFileWriterBase, IJsonGameStringFileWriterService
{
    private readonly IGameStringSerializerService _gameStringSerializerService;

    public JsonGameStringFileWriterService(
        ILogger<JsonGameStringFileWriterService> logger,
        IOptions<RootOptions> options,
        IAnsiConsole console,
        IGameStringSerializerService gameStringSerializerService,
        ISerializedDataStoreService serializedDataStoreService,
        IJsonSerializerOptionService jsonSerializerOptionService,
        IResultSummaryService resultSummaryService)
        : base(logger, options, console, serializedDataStoreService, jsonSerializerOptionService, resultSummaryService)
    {
        _gameStringSerializerService = gameStringSerializerService;
    }

    public async Task WriteForMap(string mapName)
    {
        RootGameStrings rootGameStrings = GetRootGameStrings(mapName);

        byte[] bytes = _gameStringSerializerService.SerializeGameStrings(rootGameStrings.Meta, JsonSerializerOptionService.JsonSerializerGameStringOptions);

        Span<char> buffer = stackalloc char[mapName!.Length];
        int length = NormalizeMapDirectoryName(buffer, mapName);

        await WriteSubMapJsonFile(Path.Join(Constants.JsonGameStringsDirectory, Constants.MapDirectory, buffer[..length]), mapName, Constants.GameStringFilePrefix, bytes);

        // after writing, clear the extracted gamestrings
        if (Options.MapWriterJsonOutputType != MapWriterJsonOutputType.None)
            _gameStringSerializerService.ClearStoredGameStrings();
    }

    // writes from the given bytes to a json file
    public async Task Write(byte[] bytes)
    {
        await WriteBaseJsonFile(Constants.JsonGameStringsDirectory, Constants.GameStringFilePrefix, bytes);
    }

    // instead of writing to a file, just serializes (the current dictionary items) and stores the data in the serialized data store, clear the stored gamestrings after saving
    public void SerializeOnly()
    {
        RootGameStrings rootGameStrings = GetRootGameStrings();
        byte[] bytes = _gameStringSerializerService.SerializeGameStrings(rootGameStrings.Meta, JsonSerializerOptionService.JsonSerializerGameStringOptions);

        SerializedDataStoreService.AddSerializedData(Constants.GameStringFilePrefix, bytes);

        // after saved, clear the extracted gamestrings
        _gameStringSerializerService.ClearStoredGameStrings();
    }

    protected override void IncrementFilesTotal()
    {
        ResultSummaryService.GameStringFilesTotal++;
    }

    protected override void IncrementFilesWritten()
    {
        ResultSummaryService.GameStringFilesWritten++;
    }

    private RootGameStrings GetRootGameStrings(string? mapName = null)
    {
        return new()
        {
            Meta = new()
            {
                DataTypes = [.. SerializedDataStoreService.GetDataTypesFromData().Where(x => !x.Equals(Constants.GameStringFilePrefix))],
                MapName = mapName,
                HeroesVersion = Options.HeroesVersion.GetAsHeroesDataVersion(),
                HdpVersion = Options.AppVersion,
                DescriptionText = new()
                {
                    Locale = Options.CurrentLocale,
                    GameStringTextType = Options.GameStringText.Type,
                    ReplaceFontStyles = Options.GameStringText.ReplaceFontStyles,
                    PreserveFontStyleConstantVars = Options.GameStringText.PreserveFont.PreserveFontStyleConstantVars,
                    PreserveFontStyleVars = Options.GameStringText.PreserveFont.PreserveFontStyleVars,
                },
            },
        };
    }
}
