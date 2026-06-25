using Heroes.Element;

namespace HeroesDataParser.Infrastructure.Commands.LocalizedTextCommands;

public class LocalizedTextImportService : ILocalizedTextImportService
{
    private readonly ILogger<LocalizedTextImportService> _logger;
    private readonly LocalizedTextImportOptions _options;
    private readonly IAnsiConsole _console;
    private readonly IJsonSerializerOptionService _jsonSerializerOptionService;

    public LocalizedTextImportService(
        ILogger<LocalizedTextImportService> logger,
        IOptions<LocalizedTextImportOptions> options,
        IAnsiConsole console,
        IJsonSerializerOptionService jsonSerializerOptionService)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;
        _jsonSerializerOptionService = jsonSerializerOptionService;
    }

    public async Task ImportGameStrings()
    {
        RootJsonDataElement? rootJsonDataElement = await GetRootJsonDataElement();

        if (rootJsonDataElement is null)
            return;

        await CreateOutputFile(rootJsonDataElement);
    }

    private static RootJsonDataElement GetRootJsonDataElement(IElementDocument elementDocument)
    {
        SortedDictionary<string, object> itemObjects = new(StringComparer.OrdinalIgnoreCase);

        IEnumerable<IElementObject> elementObjects = elementDocument.GetElementObjects();

        foreach (IElementObject elementObject in elementObjects)
        {
            itemObjects.Add(elementObject.Id, elementObject);
        }

        MetaDataProperties updatedMetaProperties = elementDocument.Meta;
        updatedMetaProperties.LocalizedText = LocalizedText.None;
        updatedMetaProperties.GameStringTextProperties = elementDocument.GameStringsDocument!.Meta.GameStringTextProperties;

        return new RootJsonDataElement()
        {
            MetaDataProperties = updatedMetaProperties,
            Items = itemObjects,
        };
    }

    private async Task<RootJsonDataElement?> GetRootJsonDataElement()
    {
        using FileStream dataFileStream = File.OpenRead(_options.DataFilePath);
        using JsonDocument dataJsonDocument = await JsonDocument.ParseAsync(dataFileStream);

        using FileStream gameStringFileStream = File.OpenRead(_options.GameStringsFilePath);
        using JsonDocument gameStringJsonDocument = await JsonDocument.ParseAsync(gameStringFileStream);

        using GameStringsDocument gameStringsDocument = GameStringsDocument.Load(gameStringJsonDocument);

        using IElementDocument elementDocument = DataDocument.Load(dataJsonDocument, gameStringsDocument);

        if (!ValidateElementDocument(elementDocument))
            return null;

        return GetRootJsonDataElement(elementDocument);
    }

    private bool ValidateElementDocument(IElementDocument elementDocument)
    {
        if (!elementDocument.IsMatchedHdpVersion)
        {
            _logger.LogError("The HDP version of the data file does not match the version in the gamestrings file. Data file HDP version: {DataFileHdpVersion}, Gamestrings file HDP version: {GameStringsFileHdpVersion}", elementDocument.Meta.HdpVersion, elementDocument.GameStringsDocument!.Meta.HdpVersion);
            _console.MarkupLineInterpolated($"[red]The HDP version of the data file does not match the version in the gamestrings file. Data file HDP version: {elementDocument.Meta.HdpVersion}, Gamestrings file HDP version: {elementDocument.GameStringsDocument!.Meta.HdpVersion}[/]");

            return false;
        }

        if (!elementDocument.IsMatchedHeroesVersion)
        {
            _logger.LogError("The Heroes of the Storm version of the data file does not match the version in the gamestrings file. Data file Heroes version: {DataFileHeroesVersion}, Gamestrings file Heroes version: {GameStringsFileHeroesVersion}", elementDocument.Meta.HeroesVersion, elementDocument.GameStringsDocument!.Meta.HeroesVersion);
            _console.MarkupLineInterpolated($"[red]The Heroes of the Storm version of the data file does not match the version in the gamestrings file. Data file Heroes version: {elementDocument.Meta.HeroesVersion}, Gamestrings file Heroes version: {elementDocument.GameStringsDocument!.Meta.HeroesVersion}[/]");

            return false;
        }

        if (!elementDocument.IsMatchedDataType)
        {
            _logger.LogError("The data types in the gamestrings file does not contain the data type of the data file. Data file data type: {DataFileDataType}, Gamestrings file data types: {@GameStringsFileDataTypes}", elementDocument.Meta.DataType, elementDocument.GameStringsDocument!.Meta.DataTypes);
            _console.MarkupLineInterpolated($"[red]The data types in the gamestrings file does not contain the data type of the data file. Data file data type: {elementDocument.Meta.DataType}, Gamestrings file data types: {string.Join(", ", elementDocument.GameStringsDocument!.Meta.DataTypes)}[/]");

            return false;
        }

        if (!elementDocument.IsMatchedMapName)
        {
            _logger.LogError("The map name of the data file does not match the map name in the gamestrings file. Data file map name: {DataFileMapName}, Gamestrings file map name: {GameStringsFileMapName}", elementDocument.Meta.MapName, elementDocument.GameStringsDocument!.Meta.MapName);
            _console.MarkupLineInterpolated($"[red]The map name of the data file does not match the map name in the gamestrings file. Data file map name: {elementDocument.Meta.MapName ?? "null"}, Gamestrings file map name: {elementDocument.GameStringsDocument!.Meta.MapName ?? "null"}[/]");

            return false;
        }

        return true;
    }

    private async Task CreateOutputFile(RootJsonDataElement rootJsonDataElement)
    {
        Directory.CreateDirectory(_options.OutputDirectory);
        using FileStream stream = File.Create(_options.OutputFilePath);

        await JsonSerializer.SerializeAsync(stream, rootJsonDataElement, GetJsonSerializerOptions());

        if (_options.IsNewFile)
        {
            _logger.LogInformation("New data file created at {OutputFilePath}", _options.OutputFilePath);
            _console.MarkupInterpolated($"New data file created at {_options.OutputFilePath}");
        }
        else
        {
            _logger.LogInformation("Updated data file at {OutputFilePath}", _options.OutputFilePath);
            _console.MarkupInterpolated($"Updated data file at {_options.OutputFilePath}");
        }
    }

    private JsonSerializerOptions GetJsonSerializerOptions()
    {
        JsonSerializerOptions jsonSerializerOptions = new(_jsonSerializerOptionService.GeneralJsonSerializerOptions)
        {
            WriteIndented = _options.JsonIndent,
        };

        jsonSerializerOptions.Converters.Add(new GameStringTextConverter(new GameStringTextConverterOptions()
        {
            GameStringTextType = GameStringTextType.RawText,
            RemoveHltForConstantTags = GameStringTextHltRemoveMode.None,
            RemoveHltForStyleTags = GameStringTextHltRemoveMode.None,
        }));

        jsonSerializerOptions.Converters.Add(new GameStringItemDictionaryConverter());
        jsonSerializerOptions.TypeInfoResolver = new HeroesElementResolver()
        {
            Modifiers =
            {
                typeInfo => JsonTypeInfoModifiers.SerializationModifiers(typeInfo, LocalizedTextOption.None, []),
            },
        };

        return jsonSerializerOptions;
    }
}
