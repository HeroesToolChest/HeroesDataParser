namespace HeroesDataParser.Infrastructure;

public class JsonSerializerOptionService : IJsonSerializerOptionService
{
    private readonly RootOptions _options;
    private readonly IGameStringSerializerService _gameStringSerializerService;

    private readonly JsonSerializerOptions _jsonSerializerDataOptions;
    private readonly JsonSerializerOptions _jsonSerializerGameStringOptions;

    public JsonSerializerOptionService(IOptions<RootOptions> options, IGameStringSerializerService gameStringSerializerService)
    {
        _options = options.Value;
        _gameStringSerializerService = gameStringSerializerService;

        _jsonSerializerDataOptions = SetJsonSerializerDataOptions();
        _jsonSerializerGameStringOptions = SetJsonSerializerGameStringOptions();
    }

    public JsonSerializerOptions JsonSerializerDataOptions => _jsonSerializerDataOptions;

    public JsonSerializerOptions JsonSerializerGameStringOptions => _jsonSerializerGameStringOptions;

    private JsonSerializerOptions SetJsonSerializerDataOptions()
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
                new GameStringTextConverter(gameStringTextType: _options.GameStringText.Type),
                new HeroesDataVersionConverter(),
            },
            TypeInfoResolver = new HeroesElementResolver()
            {
                Modifiers =
                {
                    typeInfo => JsonTypeInfoModifiers.SerializationModifiers(typeInfo, _options.LocalizedText, _gameStringSerializerService.DataGameStringItemDictionary),
                },
            },
        };
    }

    private JsonSerializerOptions SetJsonSerializerGameStringOptions()
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
