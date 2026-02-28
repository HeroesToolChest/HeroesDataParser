namespace HeroesDataParser.Infrastructure.JsonFilePatch;

public class JsonCreateService : IJsonCreateService
{
    private readonly ILogger<JsonCreateService> _logger;
    private readonly JsonCreateOptions _options;
    private readonly IAnsiConsole _console;

    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public JsonCreateService(
        ILogger<JsonCreateService> logger,
        IOptions<JsonCreateOptions> options,
        IAnsiConsole console)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;

        _jsonSerializerOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
    }

    public async Task CreateJsonPatch()
    {
        _logger.LogInformation("Creating JSON patch from {OldJsonFilePath} to {NewJsonFilePath}", _options.OldJsonFilePath, _options.NewJsonFilePath);

        using FileStream startStream = File.OpenRead(_options.OldJsonFilePath);
        using FileStream targetStream = File.OpenRead(_options.NewJsonFilePath);

        JsonNode? start = JsonNode.Parse(startStream);
        JsonNode? target = JsonNode.Parse(targetStream);

        if (start is null || target is null)
        {
            _logger.LogError("Either JSON file is the 'null' json value");
            _console.MarkupLine("[red]Either JSON file is the 'null' json value[/]");
            return;
        }

        // create patch here
        JsonPatch patch = start.CreatePatch(target);

        if (patch.Operations.Count == 0)
        {
            _logger.LogInformation("No differences found between the old and new JSON files. No patch file will be created.");
            _console.MarkupLine("No differences found between the old and new JSON files. No patch file will be created.");
            return;
        }

        CreateDirectory();

        using FileStream fileStream = File.Create(_options.OutputFilePath);
        await JsonSerializer.SerializeAsync(fileStream, patch, _jsonSerializerOptions);

        _logger.LogInformation("JSON patch created successfully at {OutputFilePath}", _options.OutputFilePath);
        _console.MarkupLine($"[green]JSON patch created successfully at {_options.OutputFilePath}[/]");
    }

    private void CreateDirectory()
    {
        string? directoryName = Path.GetDirectoryName(_options.OutputFilePath);
        if (!string.IsNullOrEmpty(directoryName))
            Directory.CreateDirectory(directoryName);
    }
}
