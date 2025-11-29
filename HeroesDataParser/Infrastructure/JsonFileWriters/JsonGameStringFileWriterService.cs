namespace HeroesDataParser.Infrastructure.JsonFileWriters;

public class JsonGameStringFileWriterService : IJsonGameStringFileWriterService
{
    private const string _jsonFileDirectory = "gamestrings";

    private readonly ILogger<JsonGameStringFileWriterService> _logger;
    private readonly RootOptions _options;
    private readonly IJsonSerializerOptionService _jsonSerializerOptionService;
    private readonly IResultSummaryService _resultSummaryService;

    public JsonGameStringFileWriterService(ILogger<JsonGameStringFileWriterService> logger, IOptions<RootOptions> options, IJsonSerializerOptionService jsonSerializerOptionService, IResultSummaryService resultSummaryService)
    {
        _logger = logger;
        _options = options.Value;
        _jsonSerializerOptionService = jsonSerializerOptionService;
        _resultSummaryService = resultSummaryService;
    }

    public async Task Write(GameStringItemDictionary gameStringItemDictionary, IEnumerable<string> dataTypes)
    {
        string fullOutputDirectory = Path.Combine(_options.OutputDirectory, _jsonFileDirectory);

        Directory.CreateDirectory(fullOutputDirectory);

        string fileName = $"gamestrings_{_options.BuildNumber ?? 0}_{_options.CurrentLocale}.json".ToLowerInvariant();
        string filePath = Path.Combine(fullOutputDirectory, fileName);

        _logger.LogInformation("Writing to {FilePath}", filePath);

        RootGameStrings rootGameStrings = new()
        {
            Meta = new()
            {
                DataTypes = [.. dataTypes],
                HeroesVersion = _options.HeroesVersion.GetAsHeroesDataVersion(),
                HdpVersion = _options.AppVersion,
                DescriptionText = new()
                {
                    Locale = _options.CurrentLocale,
                    GameStringTextType = _options.GameStringText.Type,
                    ReplaceFontStyles = _options.GameStringText.ReplaceFontStyles,
                    PreserveFontStyleConstantVars = _options.GameStringText.PreserveFont.PreserveFontStyleConstantVars,
                    PreserveFontStyleVars = _options.GameStringText.PreserveFont.PreserveFontStyleVars,
                },
            },
        };

        try
        {
            _resultSummaryService.GameStringFilesTotal++;
            await Write(rootGameStrings.Meta, gameStringItemDictionary, filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing game string JSON file to {FilePath}", filePath);
            return;
        }

        _resultSummaryService.GameStringFilesWritten++;

        AnsiConsole.Write("Created file ");
        AnsiConsoleHelpers.WriteFilePath(Path.Join(_jsonFileDirectory, fileName));
        AnsiConsole.WriteLine();
    }

    private async Task Write(MetaGameStringProperties metaProperties, GameStringItemDictionary gameStringItemDictionary, string filePath)
    {
        byte[] metaJson = JsonSerializer.SerializeToUtf8Bytes(metaProperties, _jsonSerializerOptionService.JsonSerializerGameStringOptions);

        await using FileStream fileStream = File.Create(filePath);
        using Utf8JsonWriter utf8JsonWriter = new(fileStream, new JsonWriterOptions()
        {
            Indented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        });

        utf8JsonWriter.WriteStartObject();

        utf8JsonWriter.WritePropertyName(nameof(RootGameStrings.Meta).ToLowerInvariant());

        // JsonDocument instead of WriteRawValue to format the meta json properly
        using JsonDocument jsonMetaDocument = JsonDocument.Parse(metaJson);
        jsonMetaDocument.RootElement.WriteTo(utf8JsonWriter);

        utf8JsonWriter.WriteStartObject(RootGameStrings.GameStringItemPropertyName);

        foreach (KeyValuePair<string, GameStringFilePropertyName> elementName in gameStringItemDictionary)
        {
            utf8JsonWriter.WriteStartObject(elementName.Key);

            foreach (KeyValuePair<string, GameStringFilePropertyId> propertyName in elementName.Value)
            {
                utf8JsonWriter.WriteStartObject(propertyName.Key);

                if (propertyName.Value.KeyArrayPairs.Count > 0)
                {
                    foreach (KeyValuePair<string, List<GameStringText>> arrayPropertyId in propertyName.Value.KeyArrayPairs)
                    {
                        utf8JsonWriter.WriteStartArray(arrayPropertyId.Key);

                        foreach (GameStringText arrayValue in arrayPropertyId.Value)
                        {
                            utf8JsonWriter.WriteStringValue(arrayValue.RawText);
                        }

                        utf8JsonWriter.WriteEndArray();
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, GameStringText> propertyId in propertyName.Value.KeyValuePairs)
                    {
                        utf8JsonWriter.WriteString(propertyId.Key, propertyId.Value.RawText);
                    }
                }

                utf8JsonWriter.WriteEndObject();
            }

            utf8JsonWriter.WriteEndObject();
        }

        utf8JsonWriter.WriteEndObject(); // end gamestrings
        utf8JsonWriter.WriteEndObject();
        await utf8JsonWriter.FlushAsync();
    }
}
