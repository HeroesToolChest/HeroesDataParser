using Polly;
using Serilog;
using Serilog.Configuration;
using System.Net;
using System.Net.Http.Headers;

namespace HeroesDataParser.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, HostApplicationBuilder builder)
    {
        services.AddSerilog((services, lc) => lc
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
            .WriteTo.Async(LoggerConfigure(), bufferSize: 500, blockWhenFull: true));

        services.AddRedaction();
        services
            .AddHttpClient("Blizzard", httpClient =>
            {
                httpClient.Timeout = TimeSpan.FromSeconds(15);
                httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("HeroesDataParser", AppVersion.GetAppVersion()));
            })
            .AddResilienceHandler("hdp-pipeline", builder =>
            {
                builder
                    .AddRetry(new Polly.Retry.RetryStrategyOptions<HttpResponseMessage>()
                    {
                        ShouldHandle = args => HandleTransientHttpError(args.Outcome),
                        MaxRetryAttempts = 2,
                        BackoffType = DelayBackoffType.Exponential,
                        Delay = TimeSpan.FromSeconds(2),
                    });
            });

        return services;
    }

    public static Action<LoggerSinkConfiguration> LoggerConfigure()
    {
        return x => x.File(new CompactJsonFormatter(), Path.Join(SerilogLogging.LogDirectory, $"{SerilogLogging.LogPrefix}{SerilogLogging.StartDateTime:yyyyMMdd_HHmmss}.txt"), retainedFileCountLimit: SerilogLogging.RetainedFileCountLimit, fileSizeLimitBytes: 1024 * 1024 * 64);
    }

    public static IServiceCollection AddHDPServices(this IServiceCollection services, HostApplicationBuilder builder)
    {
        services.Configure<RootOptions>(builder.Configuration.GetSection(nameof(RootOptions)));
        services.AddSingleton(builder.Environment.ContentRootFileProvider);

        services.AddSingleton<IParsingConfigurationService, ParsingConfigurationService>();
        services.AddSingleton<ICustomConfigurationService, CustomConfigurationService>();
        services.AddSingleton<IPreloadService, PreloadService>();

        services.AddSingleton<ISerializedElementsService, SerializedElementsService>();
        services.AddSingleton<IJsonSerializerOptionService, JsonSerializerOptionService>();

        services.AddSingleton<IHeroesXmlLoaderService, HeroesXmlLoaderService>();
        services.AddSingleton<IGameStringTextService, GameStringTextService>();

        services.AddDataParsers();
        services.AddImageWriters();

        services.AddSingleton<ISavedGameStringsService, SavedGameStringsService>();
        services.AddSingleton<IMainService, MainService>();
        services.AddSingleton<IProcessorService, ProcessorService>();
        services.AddSingleton<IMapProcessorService, MapProcessorService>();
        services.AddSingleton<IGameStringFileProcessorService, GameStringFileProcessorService>();

        //services.AddSingleton<IDataParserService, DataParserService>();
        //services.AddSingleton<IMapDataParserService, MapDataParserService>();

        services.AddSingleton<IDataExtractorService, DataExtractorService>();
        services.AddSingleton<IMapDataExtractorService, MapDataExtractorService>();

        services.AddSingleton<IJsonDataFileWriterService, JsonDataFileWriterService>();
        services.AddSingleton<IJsonGameStringFileWriterService, JsonGameStringFileWriterService>();

        return services;
    }

    private static IServiceCollection AddDataParsers(this IServiceCollection services)
    {
        // add all data parsers
        services.AddSingleton<IAbilityParser, AbilityParser>();
        services.AddSingleton<ITalentParser, TalentParser>();

        services.AddSingleton<IDataParser<Announcer>, AnnouncerParser>();
        services.AddSingleton<IDataParser<Banner>, BannerParser>();
        services.AddSingleton<IDataParser<Boost>, BoostParser>();
        services.AddSingleton<IDataParser<Bundle>, BundleParser>();
        services.AddSingleton<IDataParser<Hero>, HeroParser>();
        services.AddSingleton<IDataParser<LootChest>, LootChestParser>();
        services.AddSingleton<IDataParser<Unit>, UnitParser>();

        services.AddSingleton<IDataParser<Map>, MapParser>();

        // special for unit parser
        services.AddSingleton<IUnitParser, UnitParser>();

        return services;
    }

    private static IServiceCollection AddImageWriters(this IServiceCollection services)
    {
        // add all image writers
        services.AddSingleton<IImageWriter<Announcer>, AnnouncerImageParser>();
        services.AddSingleton<IImageWriter<Bundle>, BundleImageParser>();
        services.AddSingleton<IImageWriter<Map>, ReplayPreviewImageParser>();
        services.AddSingleton<IImageWriter<Map>, LoadingScreenImageParser>();
        services.AddSingleton<IImageWriter<Map>, MapObjectiveIconImageParser>();
        services.AddSingleton<IImageWriter<Hero>, HeroTalentImageParser>();
        services.AddSingleton<IImageWriter<Hero>, HeroAbilityImageParser>();
        services.AddSingleton<IImageWriter<Hero>, HeroAbilityTalentImageParser>();
        services.AddSingleton<IImageWriter<Hero>, HeroPortraitImageParser>();

        return services;
    }

    private static ValueTask<bool> HandleTransientHttpError(Outcome<HttpResponseMessage> outcome) => outcome switch
    {
        { Exception: HttpRequestException } => PredicateResult.True(),
        { Result.StatusCode: HttpStatusCode.RequestTimeout } => PredicateResult.True(),
        { Result.StatusCode: >= HttpStatusCode.InternalServerError } => PredicateResult.False(),
        _ => PredicateResult.False(),
    };
}
