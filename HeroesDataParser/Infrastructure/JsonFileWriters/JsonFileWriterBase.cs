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

    protected string GetFileName(string fileSuffixName, bool isPatch)
    {
        string patchSuffix = isPatch ? ".patch" : string.Empty;
        return $"{fileSuffixName}_{Options.BuildNumber ?? 0}_{Options.CurrentLocale}{patchSuffix}.json".ToLowerInvariant();
    }

    protected string GetFilePath(string innerDirectory, string fileName)
    {
        string directory = Path.Combine(Options.OutputDirectory, innerDirectory);
        Directory.CreateDirectory(directory);

        return Path.Combine(directory, fileName);
    }

    protected async Task WriteSubMapJsonFile(string innerDirectory, string mapName, DataType dataType, byte[] bytes)
    {
        if (Options.MapSpecificWriterJsonOutputType.HasFlag(MapSpecificWriterJsonOutputType.Normal))
            await WriteNormalMap(innerDirectory, mapName, dataType, bytes);

        if (Options.MapSpecificWriterJsonOutputType.HasFlag(MapSpecificWriterJsonOutputType.Patch))
            await WriteMapPatch(innerDirectory, mapName, dataType, bytes);
    }

    // the normal json file without a map loaded
    protected async Task WriteBaseJsonFile(string innerDirectory, DataType dataType, byte[] bytes)
    {
        string fileName = GetFileName(dataType.ToString(), false);
        string filePath = GetFilePath(innerDirectory, fileName);

        await WriteBaseJsonFile(innerDirectory, filePath, fileName, dataType, bytes);
    }

    protected async Task WriteBaseJsonFile(string innerDirectory, string filePath, string fileName, DataType dataType, byte[] bytes)
    {
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
        SerializedDataStoreService.AddSerializedData(dataType, bytes);

        Console.Write("Created file ");
        WriteFilePath(Path.Join(innerDirectory, fileName));
        Console.WriteLine();
    }

    protected async Task WriteMapPatch(string innerDirectory, string mapName, DataType dataType, byte[] bytes)
    {
        string fileName = GetFileName(dataType.ToString(), true);
        string filePath = GetFilePath(innerDirectory, fileName);

        Logger.LogInformation("Writing json patch file {FilePath} for map {MapName}", filePath, mapName);

        await WritePatchJsonFile(innerDirectory, mapName, dataType, fileName, filePath, bytes);
    }

    protected async Task WriteNormalMap(string innerDirectory, string mapName, DataType dataType, byte[] bytes)
    {
        string fileName = GetFileName(dataType.ToString(), false);
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

    private static int GetCountOfUniqueItems(IReadOnlyList<PatchOperation> patchOperations)
    {
        return patchOperations.Select(x =>
        {
            if (x.Path.SegmentCount > 1 && x.Path.GetSegment(0).AsSpan().Equals("items", StringComparison.OrdinalIgnoreCase))
                return x.Path.GetSegment(1).ToString();
            else
                return null;
        })
        .Where(x => !string.IsNullOrEmpty(x))
        .Distinct()
        .Count();
    }

    private async Task WritePatchJsonFile(string innerDirectory, string mapName, DataType dataType, string fileName, string filePath, byte[] bytes)
    {
        JsonPatch? jsonPatch = SerializedDataStoreService.GetJsonDataPatch(dataType, bytes);

        int totalItemsChanged = jsonPatch is not null ? GetCountOfUniqueItems(jsonPatch.Operations) : 0;

        Logger.LogInformation("Found {TotalItems} changed items of {DataType} for map {MapName}", totalItemsChanged, dataType, mapName);

        if (totalItemsChanged > 0 || Options.AllowEmptyMapSpecificPatchFiles)
        {
            try
            {
                IncrementFilesTotal();

                await using FileStream fileStream = File.Create(filePath);
                await JsonSerializer.SerializeAsync(fileStream, jsonPatch, JsonSerializerOptionService.JsonSerializerDataOptions);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error writing diff json file {FilePath} for map {MapName}", filePath, mapName);
                return;
            }

            IncrementFilesWritten();

            Console.Write("Created patch file ");
            WriteFilePath(Path.Join(innerDirectory, fileName));

            if (totalItemsChanged < 1)
                Console.Write($" [0]");
            else
                Console.Write($" [+{totalItemsChanged}]");

            Console.WriteLine();
        }
        else
        {
            Console.WriteLine($"No changes found for {dataType}");
        }
    }
}
