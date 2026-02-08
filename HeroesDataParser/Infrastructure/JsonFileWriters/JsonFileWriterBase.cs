namespace HeroesDataParser.Infrastructure.JsonFileWriters;

public abstract class JsonFileWriterBase
{
    public JsonFileWriterBase(
        ILogger logger,
        IOptions<RootOptions> options,
        IAnsiConsole console,
        ISerializedDataStoreService serializedDataStoreService,
        IJsonSerializerOptionService jsonSerializerOptionService,
        IResultSummaryService resultSummaryService)
    {
        Logger = logger;
        Options = options.Value;
        Console = console;
        SerializedDataStoreService = serializedDataStoreService;
        JsonSerializerOptionService = jsonSerializerOptionService;
        ResultSummaryService = resultSummaryService;
    }

    protected ILogger Logger { get; }

    protected RootOptions Options { get; }

    protected IAnsiConsole Console { get; }

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
        if (Options.MapSpecificWriterJsonOutputType.HasFlag(MapSpecificWriterJsonOutputType.Normal))
            await WriteNormalMap(innerDirectory, mapName, dataName, bytes);

        if (Options.MapSpecificWriterJsonOutputType.HasFlag(MapSpecificWriterJsonOutputType.Diff))
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

        Console.Write("Created file ");
        WriteFilePath(Path.Join(innerDirectory, fileName));
        Console.WriteLine();
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

        Console.Write("Created file ");
        WriteFilePath(Path.Join(innerDirectory, fileName));
        Console.WriteLine();
    }

    private static void WriteFilePath(string filePath)
    {
        AnsiConsole.Write(GetFilePath(filePath));
    }

    private static TextPath GetFilePath(string filePath)
    {
        return new TextPath(filePath)
            .SeparatorColor(Color.SpringGreen1)
            .StemColor(Color.SteelBlue1_1)
            .LeafColor(Color.Orange1);
    }

    private async Task WriteDiffJsonFile(string innerDirectory, string mapName, string dataName, string fileName, string filePath, byte[] bytes)
    {
        JsonNode? jsonDiff = SerializedDataStoreService.GetJsonDataDiff(dataName, bytes);
        JsonNode? metaNode = jsonDiff?[ElementConstants.ItemsPropertyName]; // null means that the items are the same

        int totalItemsChanged = metaNode?.AsObject()?.Count ?? 0;

        Logger.LogInformation("Found {TotalItems} changed items of {DataType} for map {MapName}", dataName, totalItemsChanged, mapName);

        if (totalItemsChanged > 0 || Options.AllowEmptyMapSpecificDiffFiles)
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

            Console.Write("Created diff file ");
            WriteFilePath(Path.Join(innerDirectory, fileName));

            if (totalItemsChanged < 1)
                Console.Write($" [0]");
            else
                Console.Write($" [+{totalItemsChanged}]");

            Console.WriteLine();
        }
        else
        {
            Console.WriteLine($"No changes found for {dataName}");
        }
    }
}
