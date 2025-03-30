using System.Text.Json.Schema;

namespace HeroesDataParser.Infrastructure;

public class JsonFileWriterService : IJsonFileWriterService
{
    private const string _jsonFileDirectory = "data";

    private readonly ILogger<JsonFileWriterService> _logger;
    private readonly RootOptions _options;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly HeroesData _heroesData;

    public JsonFileWriterService(ILogger<JsonFileWriterService> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService)
    {
        _logger = logger;
        _options = options.Value;
        _heroesData = heroesXmlLoaderService.HeroesXmlLoader.HeroesData;
        _jsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters =
            {
                new JsonStringEnumConverter(),
                new TooltipDescriptionWriteConverter(DescriptionType.RawDescription),
            },
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers =
                {
                    JsonTypeInfoModifiers.SerialiazationModifiers,
                },
            },
        };
    }

    public async Task Write<TElement>(Dictionary<string, TElement> elementsById, StormLocale stormLocale)
        where TElement : IElementObject
    {
        if (!IsSerializationRequired(elementsById.Count))
            return;

        await WriteTo(elementsById, Path.Combine(_options.OutputDirectory, _jsonFileDirectory), stormLocale);
    }

    // write to the maps sub directory
    public async Task WriteToMaps<TElement>(string mapDirectory, Dictionary<string, TElement> elementsById, StormLocale stormLocale)
        where TElement : IElementObject
    {
        if (!IsSerializationRequired(elementsById.Count))
            return;

        Span<char> buffer = stackalloc char[mapDirectory.Length];
        int length = SanitizeMapDirectory(buffer, mapDirectory);

        await WriteTo(elementsById, Path.Join(_options.OutputDirectory, _jsonFileDirectory, "maps", buffer[..length]), stormLocale);
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

    private async Task WriteTo<TElement>(Dictionary<string, TElement> elementsById, string outputDirectory, StormLocale stormLocale)
        where TElement : IElementObject
    {
        //if (_options.LocalizedText)
        //{

        //}

        Directory.CreateDirectory(outputDirectory);

        string filePath = Path.Join(outputDirectory, $"{typeof(TElement).Name}data_{_heroesData.Build ?? 0}_{stormLocale}.json").ToLowerInvariant();
        _logger.LogInformation("Writing to {FilePath}", filePath);

        await using FileStream fileStream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(fileStream, elementsById, _jsonSerializerOptions);
    }
}
