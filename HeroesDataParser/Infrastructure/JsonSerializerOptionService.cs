namespace HeroesDataParser.Infrastructure;

public class JsonSerializerOptionService : IJsonSerializerOptionService
{
    private readonly RootOptions _options;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public JsonSerializerOptionService(IOptions<RootOptions> options)
    {
        _options = options.Value;

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
                new GameStringTextConverter(gameStringTextType: _options.GameStringText.Type),
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

    public JsonSerializerOptions HeroesJsonSerializerOptions => _jsonSerializerOptions;
}
