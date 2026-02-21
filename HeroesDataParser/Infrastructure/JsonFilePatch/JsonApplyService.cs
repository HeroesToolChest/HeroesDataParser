namespace HeroesDataParser.Infrastructure.JsonFilePatch;

public class JsonApplyService : IJsonApplyService
{
    private readonly ILogger<JsonApplyService> _logger;
    private readonly JsonApplyOptions _options;
    private readonly IAnsiConsole _console;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public JsonApplyService(ILogger<JsonApplyService> logger, IOptions<JsonApplyOptions> options, IAnsiConsole console)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;
    }

    public async Task ApplyJsonPatch()
    {
        _logger.LogInformation("Applying JSON patch from {PatchFilePath} to {JsonFilePath}", _options.JsonPatchFilePath, _options.JsonFilePath);

        using FileStream jsonPatchFileStream = File.OpenRead(_options.JsonPatchFilePath);
        JsonPatch? patch = null;

        try
        {
            patch = JsonSerializer.Deserialize<JsonPatch>(jsonPatchFileStream);

            if (patch is null)
            {
                _logger.LogError("Deserialize JSON patch from {PatchFilePath} to null", _options.JsonPatchFilePath);
                _console.MarkupLine("[red]Not a valid JSON patch[/]");
                return;
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize JSON patch from {PatchFilePath}", _options.JsonPatchFilePath);
            _console.MarkupLine("[red]Not a valid JSON patch[/]");
            return;
        }

        using FileStream jsonFileStream = File.OpenRead(_options.JsonFilePath);
        JsonNode? jsonNode = JsonNode.Parse(jsonFileStream);

        PatchResult result = patch.Apply(jsonNode);

        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to apply JSON patch: {ErrorMessage}", result.Error);
            _console.MarkupLine($"[red]Failed to apply JSON patch: {result.Error}[/]");
            return;
        }

        string outputFileName = Path.GetFileNameWithoutExtension(_options.JsonPatchFilePath.Replace(".patch", string.Empty)) + ".json";
        string outputFilePath = Path.Combine(_options.OutputDirectory, outputFileName);

        Directory.CreateDirectory(_options.OutputDirectory);

        await using var stream = File.Create(outputFilePath);
        await JsonSerializer.SerializeAsync(stream, result.Result, _jsonSerializerOptions);

        _logger.LogInformation("JSON patch applied successfully. Output saved to {OutputFilePath}", outputFilePath);
        _console.MarkupLine($"[green]JSON patch applied successfully. Output saved to {outputFilePath}[/]");
    }
}
