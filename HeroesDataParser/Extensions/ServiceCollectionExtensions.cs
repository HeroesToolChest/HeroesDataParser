using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Retry;
using Serilog;
using System.Net;
using System.Net.Http.Headers;

namespace HeroesDataParser.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddConfiguration(IConfigurationRoot configuration)
        {
            services.AddSingleton(configuration);
            services.Configure<RootOptions>(configuration.GetSection(nameof(RootOptions)));
            services.Configure<CASCExtractOptions>(configuration.GetSection(nameof(CASCExtractOptions)));
            services.Configure<JsonApplyOptions>(configuration.GetSection(nameof(JsonApplyOptions)));
            services.Configure<JsonCreateOptions>(configuration.GetSection(nameof(JsonCreateOptions)));

            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog(Log.Logger, dispose: true);
            });

            return services;
        }

        public IServiceCollection AddCoreServices()
        {
            services.AddSingleton<IFileProvider>(sp => new PhysicalFileProvider(AppContext.BaseDirectory));
            services.AddRedaction();
            services
                .AddHttpClient(Constants.HttpClientBlizzard, httpClient =>
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(15);
                    httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("HeroesDataParser", AppVersion.GetAppVersion()));
                })
                .AddResilienceHandler("hdp-pipeline", builder =>
                {
                    builder
                        .AddRetry(new RetryStrategyOptions<HttpResponseMessage>()
                        {
                            ShouldHandle = args => HandleTransientHttpError(args.Outcome),
                            MaxRetryAttempts = 5,
                            BackoffType = DelayBackoffType.Exponential,
                            Delay = TimeSpan.FromSeconds(2),
                        });
                });

            return services;
        }

        public IServiceCollection AddHDPServices()
        {
            services.AddSingleton<IParsingConfigurationService, ParsingConfigurationService>();
            services.AddSingleton<ICustomConfigurationService, CustomConfigurationService>();
            services.AddSingleton<IConfigurationLoaderService, ConfigurationLoaderService>();

            services.AddSingleton<ISerializedDataStoreService, SerializedDataStoreService>();
            services.AddSingleton<IJsonSerializerOptionService, JsonSerializerOptionService>();

            services.AddSingleton<IHeroesXmlLoaderService, HeroesXmlLoaderService>();
            services.AddSingleton<IGameStringTextService, GameStringTextService>();

            services.AddDataParsers();
            services.AddImageWriters();
            services.AddCommandServices();

            services.AddSingleton<IPreLoaderService, PreLoaderService>();
            services.AddSingleton<IMainService, MainService>();
            services.AddSingleton<IMainLocalePreProcess, MainLocalePreProcess>();
            services.AddSingleton<IMainLocaleProcess, MainLocaleProcess>();
            services.AddSingleton<IProcessorService, ProcessorService>();
            services.AddSingleton<IMapProcessorService, MapProcessorService>();
            services.AddSingleton<IResultSummaryService, ResultSummaryService>();
            services.AddSingleton<IPostCleanupService, PostCleanupService>();

            services.AddSingleton<IDataExtractorService, DataExtractorService>();
            services.AddSingleton<IMapDataExtractorService, MapDataExtractorService>();

            services.AddSingleton<IJsonDataFileWriterService, JsonDataFileWriterService>();
            services.AddSingleton<IJsonGameStringFileWriterService, JsonGameStringFileWriterService>();

            services.AddSingleton<IImageWriterService, ImageWriterService>();

            services.AddSingleton<IGameStringSerializerService, GameStringSerializerService>();

            return services;
        }

        public IServiceCollection AddResiliencePipelines()
        {
            services.AddResiliencePipeline(Constants.ImageWriterPipeline, pipeline =>
            {
                pipeline.AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Constant,
                });
            });
            services.AddResiliencePipeline(Constants.CASCFileExtractorPipeline, pipeline =>
            {
                pipeline.AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Constant,
                });
            });

            return services;
        }

        private IServiceCollection AddDataParsers()
        {
            // add all data parsers
            services.AddSingleton<IAbilityParser, AbilityParser>();
            services.AddSingleton<ITalentParser, TalentParser>();

            services.AddSingleton<IDataParser<Announcer>, AnnouncerParser>();
            services.AddSingleton<IDataParser<Banner>, BannerParser>();
            services.AddSingleton<IDataParser<Boost>, BoostParser>();
            services.AddSingleton<IDataParser<Bundle>, BundleParser>();
            services.AddSingleton<IDataParser<Emoticon>, EmoticonParser>();
            services.AddSingleton<IDataParser<EmoticonPack>, EmoticonPackParser>();
            services.AddSingleton<IDataParser<Hero>, HeroParser>();
            services.AddSingleton<IDataParser<LootChest>, LootChestParser>();
            services.AddSingleton<IDataParser<Mount>, MountParser>();
            services.AddSingleton<IDataParser<MatchAward>, MatchAwardParser>();
            services.AddSingleton<IDataParser<PortraitPack>, PortraitPackParser>();
            services.AddSingleton<IDataParser<RewardPortrait>, RewardPortraitParser>();
            services.AddSingleton<IDataParser<Skin>, SkinParser>();
            services.AddSingleton<IDataParser<Spray>, SprayParser>();
            services.AddSingleton<IDataParser<TypeDescription>, TypeDescriptionParser>();
            services.AddSingleton<IDataParser<Unit>, UnitParser>();
            services.AddSingleton<IDataParser<Veterancy>, VeterancyParser>();
            services.AddSingleton<IDataParser<VoiceLine>, VoiceLineParser>();

            services.AddSingleton<IDataParser<Map>, MapParser>();

            // special for unit parser
            services.AddSingleton<IUnitParser, UnitParser>();

            return services;
        }

        private IServiceCollection AddImageWriters()
        {
            // add all image writers
            services.AddSingleton<IImageParser<Announcer>, AnnouncerImageParser>();
            services.AddSingleton<IImageParser<Bundle>, BundleImageParser>();
            services.AddSingleton<IImageParser<Emoticon>, EmoticonImageParser>();
            services.AddSingleton<IImageParser<MatchAward>, MatchAwardImageParser>();
            services.AddSingleton<IImageParser<Spray>, SprayImageParser>();
            services.AddSingleton<IImageParser<TypeDescription>, TypeDescriptionImageParser>();
            services.AddSingleton<IImageParser<VoiceLine>, VoiceLineImageParser>();

            services.AddSingleton<IImageParser<Map>, ReplayPreviewImageParser>();
            services.AddSingleton<IImageParser<Map>, LoadingScreenImageParser>();
            services.AddSingleton<IImageParser<Map>, MapObjectiveIconImageParser>();

            services.AddSingleton<IImageParser<Hero>, HeroTalentImageParser>();
            services.AddSingleton<IImageParser<Hero>, HeroAbilityImageParser>();
            services.AddSingleton<IImageParser<Hero>, HeroAbilityTalentImageParser>();
            services.AddSingleton<IImageParser<Hero>, HeroPortraitImageParser>();
            services.AddSingleton<IImageParser<Unit>, UnitPortraitImageParser>();
            services.AddSingleton<IImageParser<Unit>, UnitAbilityImageParser>();
            services.AddSingleton<IImageParser<Unit>, UnitAbilityTalentImageParser>();

            return services;
        }

        private IServiceCollection AddCommandServices()
        {
            services.AddSingleton<ICASCExtractorService, CASCExtractorService>();
            services.AddSingleton<IJsonApplyService, JsonApplyService>();
            services.AddSingleton<IJsonCreateService, JsonCreateService>();

            return services;
        }
    }

    private static ValueTask<bool> HandleTransientHttpError(Outcome<HttpResponseMessage> outcome) => outcome switch
    {
        { Exception: HttpRequestException } => PredicateResult.True(),
        { Result.StatusCode: HttpStatusCode.RequestTimeout } => PredicateResult.True(),
        { Result.StatusCode: >= HttpStatusCode.InternalServerError } => PredicateResult.False(),
        _ => PredicateResult.False(),
    };
}
