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
        (RootJsonDataElement RootJsonDataElement, JsonSerializerOptions JsonSerializerOptions)? elementDocument = await GetElementDocument();

        if (elementDocument is null)
            return;

        await CreateOutputFile(elementDocument.Value.RootJsonDataElement, elementDocument.Value.JsonSerializerOptions);
    }

    private static RootJsonDataElement GetRootJsonDataElement(IElementDocument elementDocument)
    {
        SortedDictionary<string, object> itemObjects = new(StringComparer.OrdinalIgnoreCase);

        IEnumerable<IElementObject> elementObjects = elementDocument.GetElementObjects();

        foreach (IElementObject elementObject in elementObjects)
        {
            itemObjects.Add(elementObject.Id, elementObject);
        }

        MetaDataProperties updatedMetaProperties = elementDocument.MetaProperties;
        updatedMetaProperties.LocalizedText = LocalizedTextOption.None;

        return new RootJsonDataElement()
        {
            MetaDataProperties = updatedMetaProperties,
            Items = itemObjects,
        };
    }

    private async Task<(RootJsonDataElement RootJsonDataElement, JsonSerializerOptions JsonSerializerOptions)?> GetElementDocument()
    {
        using FileStream dataFileStream = File.OpenRead(_options.DataFilePath);
        using JsonDocument dataJsonDocument = await JsonDocument.ParseAsync(dataFileStream);

        using FileStream gameStringFileStream = File.OpenRead(_options.GameStringsFilePath);
        using JsonDocument gameStringJsonDocument = await JsonDocument.ParseAsync(gameStringFileStream);

        using GameStringDocument gameStringDocument = GameStringDocument.Load(gameStringJsonDocument);

        using IElementDocument elementDocument = DataDocument.Load(dataJsonDocument, gameStringDocument);

        if (elementDocument.MismatchedHdpVersion)
        {
            _logger.LogError("The HDP version of the data file does not match the version in the gamestrings file. Data file HDP version: {DataFileHdpVersion}, Gamestrings file HDP version: {GameStringsFileHdpVersion}", elementDocument.MetaProperties.HdpVersion, gameStringDocument.MetaGameStringProperties.HdpVersion);
            _console.MarkupLineInterpolated($"[red]The HDP version of the data file does not match the version in the gamestrings file. Data file HDP version: {elementDocument.MetaProperties.HdpVersion}, Gamestrings file HDP version: {gameStringDocument.MetaGameStringProperties.HdpVersion}[/]");

            return null;
        }

        if (elementDocument.MismatchedHeroesVersion)
        {
            _logger.LogError("The Heroes of the Storm version of the data file does not match the version in the gamestrings file. Data file Heroes version: {DataFileHeroesVersion}, Gamestrings file Heroes version: {GameStringsFileHeroesVersion}", elementDocument.MetaProperties.HeroesVersion, gameStringDocument.MetaGameStringProperties.HeroesVersion);
            _console.MarkupLineInterpolated($"[red]The Heroes of the Storm version of the data file does not match the version in the gamestrings file. Data file Heroes version: {elementDocument.MetaProperties.HeroesVersion}, Gamestrings file Heroes version: {gameStringDocument.MetaGameStringProperties.HeroesVersion}[/]");

            return null;
        }

        RootJsonDataElement rootJsonDataElement = GetRootJsonDataElement(elementDocument);
        JsonSerializerOptions jsonSerializerOptions = GetJsonSerializerOptions(elementDocument);

        return (rootJsonDataElement, jsonSerializerOptions);
    }

    private JsonSerializerOptions GetJsonSerializerOptions(IElementDocument elementDocument)
    {
        JsonSerializerOptions jsonSerializerOptions = new(_jsonSerializerOptionService.GeneralJsonSerializerOptions)
        {
            WriteIndented = _options.JsonIndent,
        };

        jsonSerializerOptions.Converters.Add(new GameStringTextConverter(new GameStringTextConverterOptions()
        {
            StormLocale = elementDocument.MetaProperties.GameStringTextProperties!.Locale,
            GameStringTextType = elementDocument.MetaProperties.GameStringTextProperties.GameStringTextType!.Value,
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

    private async Task CreateOutputFile(RootJsonDataElement rootJsonDataElement, JsonSerializerOptions jsonSerializerOptions)
    {
        Directory.CreateDirectory(_options.OutputDirectory);
        using FileStream stream = File.Create(_options.OutputFilePath);

        await JsonSerializer.SerializeAsync(stream, rootJsonDataElement, jsonSerializerOptions);

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
}
