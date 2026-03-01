namespace HeroesDataParser.Infrastructure;

public class JsonSerializerOptionService : IJsonSerializerOptionService
{
    private readonly RootOptions _options;
    private readonly IGameStringSerializerService _gameStringSerializerService;

    private JsonSerializerOptions? _jsonSerializerDataOptions;
    private JsonSerializerOptions? _jsonSerializerGameStringOptions;

    public JsonSerializerOptionService(IOptions<RootOptions> options, IGameStringSerializerService gameStringSerializerService)
    {
        _options = options.Value;
        _gameStringSerializerService = gameStringSerializerService;

        GeneralJsonSerializerOptions = GetGeneralJsonSerializerOptions();
    }

    public JsonSerializerOptions GeneralJsonSerializerOptions { get; }

    public JsonSerializerOptions JsonSerializerDataOptions => GetJsonSerializerDataOptions();

    public JsonSerializerOptions JsonSerializerGameStringOptions => GetJsonSerializerGameStringOptions();

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
        if (_jsonSerializerDataOptions is not null)
            return _jsonSerializerDataOptions;

        _jsonSerializerDataOptions = new(GeneralJsonSerializerOptions);

        _jsonSerializerDataOptions.Converters.Add(new GameStringTextConverter(gameStringTextType: _options.GameStringText.Type));
        _jsonSerializerDataOptions.TypeInfoResolver = new HeroesElementResolver()
        {
            Modifiers =
            {
                typeInfo => JsonTypeInfoModifiers.SerializationModifiers(typeInfo, _options.LocalizedText, _gameStringSerializerService.DataGameStringItemDictionary),
            },
        };

        return _jsonSerializerDataOptions;
    }

    private JsonSerializerOptions GetJsonSerializerGameStringOptions()
    {
        if (_jsonSerializerGameStringOptions is not null)
            return _jsonSerializerGameStringOptions;

        _jsonSerializerGameStringOptions = new JsonSerializerOptions()
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
                new GameStringItemDictionaryConverter(),
            },
        };

        return _jsonSerializerGameStringOptions;
    }
}
