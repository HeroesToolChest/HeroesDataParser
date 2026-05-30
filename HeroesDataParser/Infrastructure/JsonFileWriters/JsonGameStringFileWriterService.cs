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

    // writes the map specific gamestrings to a json file in the maps sub directory
    public async Task WriteMapSpecific(Map map)
    {
        SortedSet<DataType> dataTypes = [.. SerializedDataStoreService.GetDataTypesFromData().Where(x => !x.Equals(DataType.GameStrings))];
        MetaGameStringProperties meta = GetMeta(dataTypes, map.Id);

        // get the current extracted gamestrings
        byte[] bytes = _gameStringSerializerService.SerializeGameStrings(meta, JsonSerializerOptionService.JsonSerializerGameStringOptions);

        await WriteSubMapJsonFile(Path.Join(Constants.JsonGameStringsDirectory, Constants.MapDirectory, map.NormalizedId), map.Id, DataType.GameStrings, bytes, true);

        // after writing, clear the extracted gamestrings
        if (Options.MapSpecificWriterJsonOutputType != MapSpecificWriterJsonOutputType.None)
            _gameStringSerializerService.ClearStoredGameStrings();
    }

    // writes the base gamestring file (no map data)
    public async Task WriteBase()
    {
        SortedSet<DataType> dataTypes = [.. SerializedDataStoreService.GetDataTypesFromData().Where(x => !x.Equals(DataType.GameStrings) && !x.Equals(DataType.MapData))];
        MetaGameStringProperties meta = GetMeta(dataTypes);

        // get the current extracted gamestrings
        byte[] bytes = _gameStringSerializerService.SerializeGameStrings(meta, JsonSerializerOptionService.JsonSerializerGameStringOptions);

        // add to store service, so the map specific writer can create a patch with the base gamestrings if needed
        SerializedDataStoreService.AddSerializedData(DataType.GameStrings, bytes);

        await WriteBaseJsonFile(Constants.JsonGameStringsDirectory, DataType.GameStrings, bytes, true);

        // after writing, clear the extracted gamestrings
        _gameStringSerializerService.ClearStoredGameStrings();
    }

    // write the base gamestring file for the map data
    public async Task WriteMap()
    {
        SortedSet<DataType> dataTypes = [DataType.MapData];
        MetaGameStringProperties meta = GetMeta(dataTypes);

        // get the current extracted gamestrings (should only be for mapdata)
        byte[] bytes = _gameStringSerializerService.SerializeGameStrings(meta, JsonSerializerOptionService.JsonSerializerGameStringOptions);

        string fileName = GetFileName($"{DataType.GameStrings}_{DataType.MapData}", false, true);
        string filePath = GetFilePath(Constants.JsonGameStringsDirectory, fileName);

        await WriteBaseJsonFile(Constants.JsonGameStringsDirectory, filePath, fileName, DataType.GameStrings, bytes);
    }

    protected override void IncrementFilesTotal()
    {
        ResultSummaryService.GameStringFilesTotal++;
    }

    protected override void IncrementFilesWritten()
    {
        ResultSummaryService.GameStringFilesWritten++;
    }

    private MetaGameStringProperties GetMeta(SortedSet<DataType> dataTypes, string? mapName = null)
    {
        return new()
        {
            ItemsType = ItemsType.GameStrings,
            DataTypes = dataTypes,
            MapName = mapName,
            HeroesVersion = Options.HeroesVersion.GetAsHeroesDataVersion(),
            HdpVersion = Options.AppVersion,
            GameStringTextProperties = new()
            {
                Locale = Options.CurrentLocale,
                GameStringTextType = Options.GameStringText.Type,
                ConstantVars =
                {
                    Replaced = Options.GameStringText.ReplaceFontConstantVars,
                    Preserved = Options.GameStringText.PreserveFontConstantVars,
                },
                StyleVars =
                {
                    Replaced = Options.GameStringText.ReplaceFontStylesVars,
                    Preserved = Options.GameStringText.PreserveFontStyleVars,
                },
            },
        };
    }
}
