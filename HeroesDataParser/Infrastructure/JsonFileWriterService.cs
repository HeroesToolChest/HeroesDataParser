using Heroes.Element.Models.Meta;
using HeroesDataParser.JsonConverters;

namespace HeroesDataParser.Infrastructure;

public class JsonFileWriterService : IJsonFileWriterService
{
    private const string _jsonFileDirectory = "data";

    private readonly ILogger<JsonFileWriterService> _logger;
    private readonly RootOptions _options;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly HeroesData _heroesData;
    private readonly IGameStringTextService _gameStringTextService;

    public JsonFileWriterService(
        ILogger<JsonFileWriterService> logger,
        IOptions<RootOptions> options,
        IHeroesXmlLoaderService heroesXmlLoaderService,
        IGameStringTextService gameStringTextService)
    {
        _logger = logger;
        _options = options.Value;
        _heroesData = heroesXmlLoaderService.HeroesXmlLoader.HeroesData;
        _gameStringTextService = gameStringTextService;

        _jsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters =
            {
                new JsonStringEnumConverter(),
                new DoubleRoundingConverter(),
                new LinkIdConverter(),
                new AbilityLinkIdConverter(),
                new TalentLinkIdConverter(),
                new GameStringTextWriteConverter(_options.GameStringText),
            },
            TypeInfoResolver = new HeroesElementResolver()
            {
                Modifiers =
                {
                    JsonTypeInfoModifiers.SerialiazationModifiers,
                },
            },
            //TypeInfoResolver = new DefaultJsonTypeInfoResolver
            //{
            //    Modifiers =
            //    {
            //        JsonTypeInfoModifiers.SerialiazationModifiers,
            //    },
            //},
        };
    }

    public async Task Write<TElement>(Dictionary<string, TElement> elementsById)
        where TElement : IElementObject
    {
        if (!IsSerializationRequired(elementsById.Count))
            return;

        await WriteTo(elementsById, _jsonFileDirectory);
    }

    // write to the maps sub directory
    public async Task WriteToMaps<TElement>(string mapDirectory, Dictionary<string, TElement> elementsById)
        where TElement : IElementObject
    {
        if (!IsSerializationRequired(elementsById.Count))
            return;

        Span<char> buffer = stackalloc char[mapDirectory.Length];
        int length = SanitizeMapDirectory(buffer, mapDirectory);

        await WriteTo(elementsById, Path.Join(_jsonFileDirectory, "maps", buffer[..length]));
    }

    private static int SanitizeMapDirectory(Span<char> buffer, string mapDirectory)
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

    private bool IsSerializationRequired(int count)
    {
        if (count > 0)
        {
            _logger.LogInformation("{Count} items to serialize", count);
            return true;
        }
        else
        {
            _logger.LogInformation("No items to serialize");
            return false;
        }
    }

    private async Task WriteTo<TElement>(Dictionary<string, TElement> elementsById, string innerDirectory)
        where TElement : IElementObject
    {
        //if (_options.LocalizedText)
        //{
        //}

        string fullOutputDirectory = Path.Join(_options.OutputDirectory, innerDirectory);

        Directory.CreateDirectory(fullOutputDirectory);

        string dataName = $"{typeof(TElement).Name}data".ToLowerInvariant();
        string fileName = $"{dataName}_{_heroesData.Build ?? 0}_{_options.CurrentLocale}.json".ToLowerInvariant();
        string filePath = Path.Join(fullOutputDirectory, fileName);

        _logger.LogInformation("Writing to {FilePath}", filePath);

        RootElement<TElement> rootElement = new()
        {
            Meta =
            {
                DataType = dataName,
                IsLocalizedText = _options.LocalizedText,
                // TODO Version
                DescriptionText = _options.LocalizedText ? null : new()
                {
                    Locale = _options.CurrentLocale,
                    GameStringTextType = _options.GameStringText.Type,
                    ReplaceFontStyles = _options.GameStringText.ReplaceFontStyles,
                    PreserveFontStyleConstantVars = _options.GameStringText.PreserveFont.PreserveFontStyleConstantVars,
                    PreserveFontStyleVars = _options.GameStringText.PreserveFont.PreserveFontStyleVars,
                },
            },
            Items = new SortedDictionary<string, TElement>(elementsById, StringComparer.Ordinal),
        };

        await using FileStream fileStream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(fileStream, rootElement, _jsonSerializerOptions);

        AnsiConsole.Write("Created json file ");
        AnsiConsole.Write(new TextPath(Path.Join(innerDirectory, fileName))
            .SeparatorColor(Color.SpringGreen1)
            .StemColor(Color.SteelBlue1_1)
            .LeafColor(Color.Orange1));
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();
    }
}
