using Heroes.Element;

namespace HeroesDataParser.Infrastructure.Commands.JsonFilePatchCommands;

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
        _jsonSerializerOptions = new(_jsonSerializerOptionService.GeneralJsonSerializerOptions)
        {
            WriteIndented = _options.JsonIndent,
        };
        _jsonSerializerOptions.Converters.Add(new GameStringTextConverter(new GameStringTextConverterOptions() { GameStringTextType = GameStringTextType.RawText }));
        _jsonSerializerOptions.Converters.Add(new GameStringItemDictionaryConverter());
    }

    public async Task ApplyJsonPatch()
    {
        _logger.LogInformation("Applying JSON patch from {PatchFilePath} to {JsonFilePath}", _options.JsonPatchFilePath, _options.JsonFilePath);

        JsonPatch? patch = await GetPatchFile();
        if (patch is null)
            return;

        JsonDocument? originalDocument = await GetOriginalAsJsonDocument();
        if (originalDocument is null)
            return;

        ItemsType? itemsType = GetItemsType(originalDocument);
        if (itemsType is null)
            return;

        JsonDocument? patchedDocument = patch.Apply(originalDocument);
        if (patchedDocument is null)
        {
            _logger.LogError("Failed to apply JSON patch, result is null");
            _console.MarkupLine("[red]Failed to apply JSON patch, result is null[/]");
            return;
        }

        await ProcessPatchResult(itemsType.Value, patchedDocument);

        DeletePatchFile();
    }

    private async Task CreateReserializedData(JsonDocument jsonDocument)
    {
        MetaDataProperties metaDataProperties = jsonDocument.RootElement.GetProperty(ElementConstants.RootMetaPropertyName).Deserialize<MetaDataProperties>(_jsonSerializerOptions)!;

        SortedDictionary<string, object> itemObjects = new(StringComparer.OrdinalIgnoreCase);
        Type elementType = DataDocument.Load(jsonDocument).ElementType;

        foreach (JsonProperty property in jsonDocument.RootElement.GetProperty(ElementConstants.ItemsPropertyName).EnumerateObject())
        {
            object? @object = property.Value.Deserialize(elementType, _jsonSerializerOptions);
            if (@object is null)
                continue;

            ((IElementObjectSetter)@object).SetId(property.Name);

            itemObjects.Add(property.Name, @object);
        }

        RootJsonDataElement rootJsonElement = new()
        {
            MetaDataProperties = metaDataProperties,
            Items = itemObjects,
        };

        CreateDirectory();

        await using FileStream stream = File.Create(_options.OutputFilePath);
        await JsonSerializer.SerializeAsync(stream, rootJsonElement, _jsonSerializerOptions);

        DisplaySuccess();
    }

    private async Task CreateReserializedGameString(JsonDocument jsonDocument)
    {
        RootJsonGameStringElement? rootJsonGameStringElement = jsonDocument.RootElement.Deserialize<RootJsonGameStringElement>(_jsonSerializerOptions);

        CreateDirectory();

        await using FileStream stream = File.Create(_options.OutputFilePath);
        await JsonSerializer.SerializeAsync(stream, rootJsonGameStringElement, _jsonSerializerOptions);

        DisplaySuccess();
    }

    private async Task<JsonPatch?> GetPatchFile()
    {
        JsonPatch? jsonPatch;

        using FileStream jsonPatchFileStream = File.OpenRead(_options.JsonPatchFilePath);

        try
        {
            jsonPatch = await JsonSerializer.DeserializeAsync<JsonPatch>(jsonPatchFileStream);

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

    private async Task<JsonDocument?> GetOriginalAsJsonDocument()
    {
        using FileStream jsonFileStream = File.OpenRead(_options.JsonFilePath);
        try
        {
            return await JsonDocument.ParseAsync(jsonFileStream);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse JSON file from {JsonFilePath}", _options.JsonFilePath);
            _console.MarkupLine($"[red]Failed to parse JSON file from {_options.JsonFilePath}[/]");
            return null;
        }
    }

    private ItemsType? GetItemsType(JsonDocument jsonDocument)
    {
        if (!jsonDocument.RootElement.TryGetProperty(ElementConstants.RootMetaPropertyName, out JsonElement metaJsonElement))
        {
            _logger.LogError("Missing '{PropertyName}' property in JSON", ElementConstants.RootMetaPropertyName);
            _console.MarkupLine($"[red]Missing '{ElementConstants.RootMetaPropertyName}' property in JSON[/]");
            return null;
        }

        MetaProperties metaProperties = metaJsonElement.Deserialize<MetaProperties>(_jsonSerializerOptions)!;

        if (metaProperties.ItemsType != ItemsType.Data && metaProperties.ItemsType != ItemsType.GameStrings)
        {
            _logger.LogError("Not a valid '{PropertyName}' value in JSON: {ItemsType}", ElementConstants.ItemsTypePropertyName, metaProperties.ItemsType);
            _console.MarkupInterpolated($"[red]Not a valid '{ElementConstants.ItemsTypePropertyName}' value in JSON: {metaProperties.ItemsType}[/]");
            return null;
        }

        return metaProperties.ItemsType;
    }

    private async Task ProcessPatchResult(ItemsType itemsType, JsonDocument jsonDocument)
    {
        if (itemsType == ItemsType.Data)
        {
            await CreateReserializedData(jsonDocument);
        }
        else if (itemsType == ItemsType.GameStrings)
        {
            await CreateReserializedGameString(jsonDocument);
        }
        else
        {
            _logger.LogError("Unsupported itemsType: {ItemsType}", itemsType);
            _console.MarkupLine($"[red]Unsupported itemsType: {itemsType}[/]");
        }
    }

    private void DeletePatchFile()
    {
        if (_options.DeletePatchFile)
        {
            try
            {
                File.Delete(_options.JsonPatchFilePath);
                _logger.LogInformation("Deleted JSON patch file at {PatchFilePath}", _options.JsonPatchFilePath);
                _console.MarkupLine($"Deleted JSON patch file at {_options.JsonPatchFilePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete JSON patch file at {PatchFilePath}", _options.JsonPatchFilePath);
                _console.MarkupLine($"[red]Failed to delete JSON patch file at {_options.JsonPatchFilePath}[/]");
            }
        }
    }

    private void DisplayJsonFailed()
    {
        _console.MarkupLine("[red]Not a valid JSON patch[/]");
    }

    private void DisplaySuccess()
    {
        _logger.LogInformation("JSON patch applied successfully. Output saved to {OutputFilePath}", _options.OutputFilePath);
        _console.MarkupLine($"JSON patch applied successfully. Output saved to {_options.OutputFilePath}");
    }

    private void CreateDirectory()
    {
        string? directoryName = Path.GetDirectoryName(_options.OutputFilePath);
        if (!string.IsNullOrEmpty(directoryName))
            Directory.CreateDirectory(directoryName);
    }
}
