namespace HeroesDataParser.Infrastructure.JsonFileWriters;

public abstract class JsonFileWriterBase
{
    public JsonFileWriterBase(
        ILogger logger,
        IOptions<RootOptions> options,
        ISerializedDataStoreService serializedDataStoreService,
        IJsonSerializerOptionService jsonSerializerOptionService,
        IResultSummaryService resultSummaryService)
    {
        Logger = logger;
        Options = options.Value;
        SerializedDataStoreService = serializedDataStoreService;
        JsonSerializerOptionService = jsonSerializerOptionService;
        ResultSummaryService = resultSummaryService;
    }

    protected ILogger Logger { get; }

    protected RootOptions Options { get; }

    protected ISerializedDataStoreService SerializedDataStoreService { get; }

    protected IJsonSerializerOptionService JsonSerializerOptionService { get; }

    protected IResultSummaryService ResultSummaryService { get; }

    protected static int NormalizeMapDirectoryName(Span<char> buffer, string mapDirectory)
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

    protected abstract void IncrementFilesTotal();

    protected abstract void IncrementFilesWritten();

    protected string GetFileName(string dataName, bool isDiff)
    {
        string diffSuffix = isDiff ? ".diff" : string.Empty;
        return $"{dataName}_{Options.BuildNumber ?? 0}_{Options.CurrentLocale}{diffSuffix}.json".ToLowerInvariant();
    }

    protected string GetFilePath(string innerDirectory, string fileName)
    {
        string directory = Path.Combine(Options.OutputDirectory, innerDirectory);
        Directory.CreateDirectory(directory);

        return Path.Combine(directory, fileName);
    }

    protected async Task WriteSubMapJsonFile(string innerDirectory, string mapName, string dataName, byte[] bytes)
    {
        if (Options.MapWriterJsonOutputType.HasFlag(MapWriterJsonOutputType.Normal))
            await WriteNormalMap(innerDirectory, mapName, dataName, bytes);

        if (Options.MapWriterJsonOutputType.HasFlag(MapWriterJsonOutputType.Diff))
            await WriteMapDiff(innerDirectory, mapName, dataName, bytes);
    }

    // the normal json file without a map loaded
    protected async Task WriteBaseJsonFile(string innerDirectory, string dataName, byte[] bytes)
    {
        string fileName = GetFileName(dataName, false);
        string filePath = GetFilePath(innerDirectory, fileName);

        Logger.LogInformation("Writing to {FilePath}", filePath);

        try
        {
            IncrementFilesTotal();

            await using FileStream fileStream = File.Create(filePath);
            await fileStream.WriteAsync(bytes);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error writing json file to {FilePath}", filePath);
            return;
        }

        IncrementFilesWritten();
        SerializedDataStoreService.AddSerializedData(dataName, bytes);

        AnsiConsole.Write("Created file ");
        AnsiConsoleHelpers.WriteFilePath(Path.Join(innerDirectory, fileName));
        AnsiConsole.WriteLine();
    }

    protected async Task WriteMapDiff(string innerDirectory, string mapName, string dataName, byte[] bytes)
    {
        string fileName = GetFileName(dataName, true);
        string filePath = GetFilePath(innerDirectory, fileName);

        Logger.LogInformation("Writing diff json file {FilePath} for map {MapName}", filePath, mapName);

        await WriteDiffJsonFile(innerDirectory, mapName, dataName, fileName, filePath, bytes);
    }

    protected async Task WriteNormalMap(string innerDirectory, string mapName, string dataName, byte[] bytes)
    {
        string fileName = GetFileName(dataName, false);
        string filePath = GetFilePath(innerDirectory, fileName);

        Logger.LogInformation("Writing normal json file {FilePath} for map {MapName}", filePath, mapName);

        try
        {
            IncrementFilesTotal();

            await using FileStream fileStream = File.Create(filePath);
            await fileStream.WriteAsync(bytes);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error writing json file {FilePath} for map {MapName}", filePath, mapName);
            return;
        }

        IncrementFilesWritten();

        AnsiConsole.Write("Created file ");
        AnsiConsoleHelpers.WriteFilePath(Path.Join(innerDirectory, fileName));
        AnsiConsole.WriteLine();
    }

    private async Task WriteDiffJsonFile(string innerDirectory, string mapName, string dataName, string fileName, string filePath, byte[] bytes)
    {
        JsonNode? jsonDiff = SerializedDataStoreService.GetJsonDataDiff(dataName, bytes);
        JsonNode? metaNode = jsonDiff?[ElementConstants.ItemsPropertyName]; // null means that the items are the same

        int totalItemsChanged = metaNode?.AsObject()?.Count ?? 0;

        Logger.LogInformation("Found {TotalItems} changed items of {DataType} for map {MapName}", dataName, totalItemsChanged, mapName);

        if (totalItemsChanged > 0 || Options.AllowEmptyDiffFiles)
        {
            try
            {
                IncrementFilesTotal();

                await using FileStream fileStream = File.Create(filePath);
                await JsonSerializer.SerializeAsync(fileStream, jsonDiff, JsonSerializerOptionService.JsonSerializerDataOptions);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error writing diff json file {FilePath} for map {MapName}", filePath, mapName);
                return;
            }

            IncrementFilesWritten();

            AnsiConsole.Write("Created diff file ");
            AnsiConsoleHelpers.WriteFilePath(Path.Join(innerDirectory, fileName));

            if (totalItemsChanged < 1)
                AnsiConsole.Write($" [0]");
            else
                AnsiConsole.Write($" [+{totalItemsChanged}]");

            AnsiConsole.WriteLine();
        }
        else
        {
            AnsiConsole.WriteLine($"No changes found for {dataName}");
        }
    }
}
