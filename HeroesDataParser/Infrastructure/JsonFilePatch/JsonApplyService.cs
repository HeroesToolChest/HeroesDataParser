using Heroes.Element;

namespace HeroesDataParser.Infrastructure.JsonFilePatch;

public class JsonApplyService : IJsonApplyService
{
    private readonly ILogger<JsonApplyService> _logger;
    private readonly JsonApplyOptions _options;
    private readonly IAnsiConsole _console;
    private readonly IJsonSerializerOptionService _jsonSerializerOptionService;

    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public JsonApplyService(
        ILogger<JsonApplyService> logger,
        IOptions<JsonApplyOptions> options,
        IAnsiConsole console,
        IJsonSerializerOptionService jsonSerializerOptionService)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;
        _jsonSerializerOptionService = jsonSerializerOptionService;
        _jsonSerializerOptions = new(_jsonSerializerOptionService.GeneralJsonSerializerOptions);
        _jsonSerializerOptions.Converters.Add(new GameStringTextConverter(gameStringTextType: _options.GameStringText.Type));
        _jsonSerializerOptions.Converters.Add(new GameStringItemDictionaryConverter());
    }

    public async Task ApplyJsonPatch()
    {
        _logger.LogInformation("Applying JSON patch from {PatchFilePath} to {JsonFilePath}", _options.JsonPatchFilePath, _options.JsonFilePath);

        JsonPatch? patch = GetPatchFile();
        if (patch is null)
            return;

        JsonNode? jsonNode = GetJsonNode();
        if (jsonNode is null)
            return;

        ItemsType? itemsType = GetItemsType(jsonNode);
        if (itemsType is null)
            return;

        PatchResult result = patch.Apply(jsonNode);

        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to apply JSON patch: {ErrorMessage}", result.Error);
            _console.MarkupLine($"[red]Failed to apply JSON patch: {result.Error}[/]");
            return;
        }

        if (itemsType == ItemsType.Data)
        {
            await CreateReserializedData(result);
        }
        else if (itemsType == ItemsType.GameStrings)
        {
            await CreateReserializedGameString(result);
        }
        else
        {
            _logger.LogError("Unsupported itemsType: {ItemsType}", itemsType);
            _console.MarkupLine($"[red]Unsupported itemsType: {itemsType}[/]");
        }
    }

    private async Task CreateReserializedData(PatchResult result)
    {
        MetaDataProperties? metaDataProperties = result.Result?[0].Deserialize<MetaDataProperties>(_jsonSerializerOptions);
        if (metaDataProperties is null)
        {
            _logger.LogError("Failed to deserialize MetaDataProperties from JSON patch result");
            _console.MarkupLine("[red]Failed to deserialize MetaDataProperties from JSON patch result[/]");
            return;
        }

        SortedDictionary<string, object> itemObjects = new(StringComparer.OrdinalIgnoreCase);

        foreach (KeyValuePair<string, JsonNode?> property in result.Result![1]!.AsObject())
        {
            object? @object = property.Value.Deserialize(GetElementType(), _jsonSerializerOptions);
            if (@object is not null)
            {
                ((IElementObjectSetter)@object).SetId(property.Key);

                itemObjects.Add(property.Key, @object);
            }
        }

        RootJsonDataElement rootJsonElement = new()
        {
            MetaDataProperties = metaDataProperties,
            Items = itemObjects,
        };

        // get file name based on patch file
        string outputFileName = Path.GetFileNameWithoutExtension(_options.JsonPatchFilePath.Replace(".patch", string.Empty)) + ".json";
        string outputFilePath = Path.Combine(_options.OutputDirectory, outputFileName);

        Directory.CreateDirectory(_options.OutputDirectory);

        await using var stream2 = File.Create(outputFilePath);
        await JsonSerializer.SerializeAsync(stream2, rootJsonElement, _jsonSerializerOptions);

        _logger.LogInformation("JSON patch applied successfully. Output saved to {OutputFilePath}", outputFilePath);
        _console.MarkupLine($"[green]JSON patch applied successfully. Output saved to {outputFilePath}[/]");
    }

    private async Task CreateReserializedGameString(PatchResult result)
    {
        RootJsonGameStringElement? rootJsonGameStringElement = result.Result?.AsObject().Deserialize<RootJsonGameStringElement>(_jsonSerializerOptions);

        // get file name based on patch file
        string outputFileName = Path.GetFileNameWithoutExtension(_options.JsonPatchFilePath.Replace(".patch", string.Empty)) + ".json";
        string outputFilePath = Path.Combine(_options.OutputDirectory, outputFileName);

        Directory.CreateDirectory(_options.OutputDirectory);

        await using var stream2 = File.Create(outputFilePath);
        await JsonSerializer.SerializeAsync(stream2, rootJsonGameStringElement, _jsonSerializerOptions);

        _logger.LogInformation("JSON patch applied successfully. Output saved to {OutputFilePath}", outputFilePath);
        _console.MarkupLine($"[green]JSON patch applied successfully. Output saved to {outputFilePath}[/]");
    }

    private JsonPatch? GetPatchFile()
    {
        JsonPatch? jsonPatch;

        using FileStream jsonPatchFileStream = File.OpenRead(_options.JsonPatchFilePath);

        try
        {
            jsonPatch = JsonSerializer.Deserialize<JsonPatch>(jsonPatchFileStream);

            if (jsonPatch is null)
            {
                _logger.LogError("Deserialize JSON patch from {PatchFilePath} to null", _options.JsonPatchFilePath);
                DisplayJsonFailed();
                return null;
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize JSON patch from {PatchFilePath}", _options.JsonPatchFilePath);
            DisplayJsonFailed();
            return null;
        }

        return jsonPatch;
    }

    private JsonNode? GetJsonNode()
    {
        using FileStream jsonFileStream = File.OpenRead(_options.JsonFilePath);
        try
        {
            JsonNode? jsonNode = JsonNode.Parse(jsonFileStream);
            if (jsonNode is null)
            {
                _logger.LogError("Failed to parse JSON file from {JsonFilePath}", _options.JsonFilePath);
                _console.MarkupLine($"[red]Failed to parse JSON file from {_options.JsonFilePath}[/]");
                return null;
            }

            return jsonNode;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse JSON file from {JsonFilePath}", _options.JsonFilePath);
            _console.MarkupLine($"[red]Failed to parse JSON file from {_options.JsonFilePath}[/]");
            return null;
        }
    }

    private ItemsType? GetItemsType(JsonNode jsonNode)
    {
        if (!jsonNode.AsObject().TryGetPropertyValue("meta", out JsonNode? metaJsonNode) || metaJsonNode is null ||
            !metaJsonNode.AsObject().TryGetPropertyValue("itemsType", out JsonNode? itemsTypeNode) || itemsTypeNode is null)
        {
            _logger.LogError("Could not find 'itemsType' property in JSON");
            _console.MarkupLine("[red]Could not find 'itemsType' property in JSON[/]");
            return null;
        }

        if (!Enum.TryParse(itemsTypeNode.GetValue<string>(), out ItemsType itemsType))
        {
            _logger.LogError("Not a valid 'itemsType' value in JSON: {ItemsTypeValue}", itemsTypeNode.GetValue<string>());
            _console.MarkupInterpolated($"[red]Not a valid 'itemsType' value in JSON: {itemsTypeNode.GetValue<string>()}");
            return null;
        }

        return itemsType;
    }

    private Type GetElementType()
    {
        using FileStream jsonFileStream = File.OpenRead(_options.JsonFilePath);
        using JsonDocument jsonDocument = JsonDocument.Parse(jsonFileStream);
        using IElementDocument elementDocument = DataDocument.Load(jsonDocument);

        return elementDocument.GetElementType;
    }

    private void DisplayJsonFailed()
    {
        _console.MarkupLine("[red]Not a valid JSON patch[/]");
    }
}
