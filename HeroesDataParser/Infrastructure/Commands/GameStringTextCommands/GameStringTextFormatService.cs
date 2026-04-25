using Heroes.Element;

namespace HeroesDataParser.Infrastructure.Commands.GameStringTextCommands;

public class GameStringTextFormatService : IGameStringTextUpdateService
{
    private readonly ILogger<GameStringTextFormatService> _logger;
    private readonly GameStringTextFormatOptions _options;
    private readonly IAnsiConsole _console;
    private readonly IJsonSerializerOptionService _jsonSerializerOptionService;

    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public GameStringTextFormatService(
        ILogger<GameStringTextFormatService> logger,
        IOptions<GameStringTextFormatOptions> options,
        IAnsiConsole console,
        IJsonSerializerOptionService jsonSerializerOptionService)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;
        _jsonSerializerOptionService = jsonSerializerOptionService;
        _jsonSerializerOptions = new(_jsonSerializerOptionService.GeneralJsonSerializerOptions);
    }

    public async Task FormatGameStringText()
    {
        SetSerializationOptions();

        FileStream fileStream = File.OpenRead(_options.FilePath);
        JsonDocument jsonDocument = await JsonDocument.ParseAsync(fileStream);

        try
        {
            ItemsType? itemsType = GetItemsType(jsonDocument);
            if (itemsType is null)
                return;

            if (itemsType == ItemsType.Data)
            {
                RootJsonDataElement? rootJsonDataElement = await UpdateDataFile(jsonDocument);

                if (rootJsonDataElement is null)
                    return;

                fileStream.Dispose();
                jsonDocument.Dispose();

                using FileStream stream = GetNewFileStream();
                await JsonSerializer.SerializeAsync(stream, rootJsonDataElement, _jsonSerializerOptions);

                if (_options.IsNewFile)
                {
                    _logger.LogInformation("New formatted data file created at {OutputFilePath}", _options.OutputFilePath);
                    _console.MarkupInterpolated($"New formatted data file created at {_options.OutputFilePath}");
                }
                else
                {
                    _logger.LogInformation("Updated formatted data file at {OutputFilePath}", _options.OutputFilePath);
                    _console.MarkupInterpolated($"Updated formatted data file at {_options.OutputFilePath}");
                }
            }
            else if (itemsType == ItemsType.GameStrings)
            {
                RootJsonGameStringElement? rootJsonGameStringElement = await UpdateGameStringsFile(jsonDocument);

                if (rootJsonGameStringElement is null)
                    return;

                fileStream.Dispose();
                jsonDocument.Dispose();

                using FileStream stream = GetNewFileStream();
                await JsonSerializer.SerializeAsync(stream, rootJsonGameStringElement, _jsonSerializerOptions);

                if (_options.IsNewFile)
                {
                    _logger.LogInformation("New formatted gamestring file created at {OutputFilePath}", _options.OutputFilePath);
                    _console.MarkupInterpolated($"New formatted gamestring file created at {_options.OutputFilePath}");
                }
                else
                {
                    _logger.LogInformation("Updated formatted gamestring file at {OutputFilePath}", _options.OutputFilePath);
                    _console.MarkupInterpolated($"Updated formatted gamestring file at {_options.OutputFilePath}");
                }
            }
            else
            {
                _logger.LogError("Unsupported itemsType: {ItemsType}", itemsType);
                _console.MarkupLine($"[red]Unsupported itemsType: {itemsType}[/]");
            }
        }
        finally
        {
            fileStream?.Dispose();
            jsonDocument?.Dispose();
        }
    }

    private async Task<RootJsonDataElement?> UpdateDataFile(JsonDocument jsonDocument)
    {
        MetaDataProperties metaDataProperties = jsonDocument.RootElement.GetProperty(ElementConstants.RootMetaPropertyName).Deserialize<MetaDataProperties>(_jsonSerializerOptions)!;
        if (!ValidateLocalizedText(metaDataProperties))
            return null;

        metaDataProperties.GameStringTextProperties!.GameStringTextType = _options.GameStringTextType;

        if (_options.GameStringTextHltConstantRemoveMode != GameStringTextHltRemoveMode.None)
        {
            metaDataProperties.GameStringTextProperties.ReplaceFontConstantVars = GetReplaceFontConstantVarsValue();
            metaDataProperties.GameStringTextProperties.PreserveFontConstantVars = GetPreserveFontConstantVarsValue();
        }

        if (_options.GameStringTextHltStyleRemoveMode != GameStringTextHltRemoveMode.None)
        {
            metaDataProperties.GameStringTextProperties.ReplaceFontStylesVars = GetReplaceFontStylesVarsValue();
            metaDataProperties.GameStringTextProperties.PreserveFontStyleVars = GetPreserveFontStyleVars();
        }

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

        return new RootJsonDataElement()
        {
            MetaDataProperties = metaDataProperties,
            Items = itemObjects,
        };
    }

    private async Task<RootJsonGameStringElement?> UpdateGameStringsFile(JsonDocument jsonDocument)
    {
        RootJsonGameStringElement? rootJsonGameStringElement = jsonDocument.RootElement.Deserialize<RootJsonGameStringElement>(_jsonSerializerOptions)!;

        rootJsonGameStringElement.MetaGameStringProperties.GameStringTextProperties!.GameStringTextType = _options.GameStringTextType;

        if (_options.GameStringTextHltConstantRemoveMode != GameStringTextHltRemoveMode.None)
        {
            rootJsonGameStringElement.MetaGameStringProperties.GameStringTextProperties.ReplaceFontConstantVars = GetReplaceFontConstantVarsValue();
            rootJsonGameStringElement.MetaGameStringProperties.GameStringTextProperties.PreserveFontConstantVars = GetPreserveFontConstantVarsValue();
        }

        if (_options.GameStringTextHltStyleRemoveMode != GameStringTextHltRemoveMode.None)
        {
            rootJsonGameStringElement.MetaGameStringProperties.GameStringTextProperties.ReplaceFontStylesVars = GetReplaceFontStylesVarsValue();
            rootJsonGameStringElement.MetaGameStringProperties.GameStringTextProperties.PreserveFontStyleVars = GetPreserveFontStyleVars();
        }

        return rootJsonGameStringElement;
    }

    private bool GetReplaceFontConstantVarsValue()
    {
        return _options.GameStringTextHltConstantRemoveMode == GameStringTextHltRemoveMode.Remove;
    }

    private bool GetReplaceFontStylesVarsValue() => _options.GameStringTextHltStyleRemoveMode == GameStringTextHltRemoveMode.Remove;

    private bool GetPreserveFontConstantVarsValue() => _options.GameStringTextHltConstantRemoveMode != GameStringTextHltRemoveMode.Remove && _options.GameStringTextHltConstantRemoveMode != GameStringTextHltRemoveMode.RemoveAndUndo;

    private bool GetPreserveFontStyleVars() => _options.GameStringTextHltStyleRemoveMode != GameStringTextHltRemoveMode.Remove && _options.GameStringTextHltStyleRemoveMode != GameStringTextHltRemoveMode.RemoveAndUndo;

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
            _console.MarkupLineInterpolated($"[red]Not a valid '{ElementConstants.ItemsTypePropertyName}' value in JSON: {metaProperties.ItemsType}[/]");
            return null;
        }

        return metaProperties.ItemsType;
    }

    private bool ValidateLocalizedText(MetaDataProperties metaDataProperties)
    {
        if (metaDataProperties.LocalizedText == LocalizedTextOption.Extract)
        {
            _logger.LogInformation("'{PropertyName}' value in JSON is '{LocalizedTextOption}'", ElementConstants.LocalizedTextPropertyName, LocalizedTextOption.Extract);
            _console.MarkupLine($"'{ElementConstants.LocalizedTextPropertyName}' value in JSON is '{LocalizedTextOption.Extract}', no gamestrings to update");
            return false;
        }

        return true;
    }

    private void SetSerializationOptions()
    {
        _jsonSerializerOptions.WriteIndented = _options.JsonIndent;
        _jsonSerializerOptions.Converters.Add(new GameStringTextConverter(new GameStringTextConverterOptions()
        {
            GameStringTextType = _options.GameStringTextType,
            RemoveHltForConstantTags = _options.GameStringTextHltConstantRemoveMode,
            RemoveHltForStyleTags = _options.GameStringTextHltStyleRemoveMode,
        }));
        _jsonSerializerOptions.Converters.Add(new GameStringItemDictionaryConverter());

        _jsonSerializerOptions.TypeInfoResolver = new HeroesElementResolver()
        {
            Modifiers =
            {
                typeInfo => JsonTypeInfoModifiers.SerializationModifiers(typeInfo, LocalizedTextOption.None, []),
            },
        };
    }

    private FileStream GetNewFileStream()
    {
        Directory.CreateDirectory(_options.OutputDirectory);

        return File.Create(_options.OutputFilePath);
    }
}