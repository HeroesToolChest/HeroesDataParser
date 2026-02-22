namespace HeroesDataParser.Infrastructure;

public class JsonSerializerOptionService : IJsonSerializerOptionService
{
    private readonly RootOptions _options;
    private readonly IGameStringSerializerService _gameStringSerializerService;

    private readonly JsonSerializerOptions _generalJsonSerializerOptions;
    private readonly JsonSerializerOptions _jsonSerializerDataOptions;
    private readonly JsonSerializerOptions _jsonSerializerGameStringOptions;

    public JsonSerializerOptionService(IOptions<RootOptions> options, IGameStringSerializerService gameStringSerializerService)
    {
        _options = options.Value;
        _gameStringSerializerService = gameStringSerializerService;

        _generalJsonSerializerOptions = GetGeneralJsonSerializerOptions();
        _jsonSerializerDataOptions = GetJsonSerializerDataOptions();
        _jsonSerializerGameStringOptions = GetJsonSerializerGameStringOptions();
    }

    public JsonSerializerOptions GeneralJsonSerializerOptions => _generalJsonSerializerOptions;

    public JsonSerializerOptions JsonSerializerDataOptions => _jsonSerializerDataOptions;

    public JsonSerializerOptions JsonSerializerGameStringOptions => _jsonSerializerGameStringOptions;

    private static JsonSerializerOptions GetGeneralJsonSerializerOptions()
    {
        return new JsonSerializerOptions()
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
                new HeroesDataVersionConverter(),
            },
        };
    }

    private JsonSerializerOptions GetJsonSerializerDataOptions()
    {
        JsonSerializerOptions jsonSerializerOptions = new(_generalJsonSerializerOptions);

        jsonSerializerOptions.Converters.Add(new GameStringTextConverter(gameStringTextType: _options.GameStringText.Type));
        jsonSerializerOptions.TypeInfoResolver = new HeroesElementResolver()
        {
            Modifiers =
            {
                typeInfo => JsonTypeInfoModifiers.SerializationModifiers(typeInfo, _options.LocalizedText, _gameStringSerializerService.DataGameStringItemDictionary),
            },
        };

        return jsonSerializerOptions;
    }

    private JsonSerializerOptions GetJsonSerializerGameStringOptions()
    {
        return new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters =
            {
                new JsonStringEnumConverter(),
                new GameStringTextConverter(gameStringTextType: _options.GameStringText.Type),
                new HeroesDataVersionConverter(),
            },
        };
    }
}
