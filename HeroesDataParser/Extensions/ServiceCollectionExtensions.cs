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
            services.Configure<GameStringTextFormatOptions>(configuration.GetSection(nameof(GameStringTextFormatOptions)));
            services.Configure<JsonSchemaExportOptions>(configuration.GetSection(nameof(JsonSchemaExportOptions)));
            services.Configure<PortraitInfoOptions>(configuration.GetSection(nameof(PortraitInfoOptions)));
            services.Configure<PortraitBattleNetCacheOptions>(configuration.GetSection(nameof(PortraitBattleNetCacheOptions)));
            services.Configure<PortraitExtractAutoOptions>(configuration.GetSection(nameof(PortraitExtractAutoOptions)));
            services.Configure<PortraitExtractOptions>(configuration.GetSection(nameof(PortraitExtractOptions)));

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

            services.AddSingleton<IResultSummaryService, ResultSummaryService>();
            services.AddSingleton<IPostCleanupService, PostCleanupService>();

            services.AddSingleton<IDataExtractorService, DataExtractorService>();
            services.AddSingleton<IMapDataExtractorService, MapDataExtractorService>();

            services.AddSingleton<IJsonDataFileWriterService, JsonDataFileWriterService>();
            services.AddSingleton<IJsonGameStringFileWriterService, JsonGameStringFileWriterService>();

            services.AddSingleton<IImageWriterService, ImageWriterService>();

            services.AddSingleton<IGameStringSerializerService, GameStringSerializerService>();

            // processors
            services.AddSingleton<IMainLocalePreProcessor, MainLocalePreProcessor>();
            services.AddSingleton<IMainLocaleProcessor, MainLocaleProcessor>();
            services.AddSingleton<IProcessorService, ProcessorService>();
            services.AddSingleton<IMapProcessorService, MapProcessorService>();
            services.AddSingleton<IXmlDataParserProcessor, XmlDataParserProcessor>();
            services.AddSingleton<IXmlMapDataParserProcessor, XmlMapDataParserProcessor>();
            services.AddSingleton<IJsonDataFileWriterProcessor, JsonDataFileWriterProcessor>();
            services.AddSingleton<IJsonGameStringFileWriterProcessor, JsonGameStringFileWriterProcessor>();
            services.AddSingleton<IImageParserProcessor, ImageParserProcessor>();

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

            services.AddKeyedSingleton<IDataParser<Announcer>, AnnouncerParser>(typeof(Announcer));
            services.AddKeyedSingleton<IDataParser<Banner>, BannerParser>(typeof(Banner));
            services.AddKeyedSingleton<IDataParser<Boost>, BoostParser>(typeof(Boost));
            services.AddKeyedSingleton<IDataParser<Bundle>, BundleParser>(typeof(Bundle));
            services.AddKeyedSingleton<IDataParser<Emoticon>, EmoticonParser>(typeof(Emoticon));
            services.AddKeyedSingleton<IDataParser<EmoticonPack>, EmoticonPackParser>(typeof(EmoticonPack));
            services.AddKeyedSingleton<IDataParser<Hero>, HeroParser>(typeof(Hero));
            services.AddKeyedSingleton<IDataParser<LootChest>, LootChestParser>(typeof(LootChest));
            services.AddKeyedSingleton<IDataParser<Mount>, MountParser>(typeof(Mount));
            services.AddKeyedSingleton<IDataParser<MatchAward>, MatchAwardParser>(typeof(MatchAward));
            services.AddKeyedSingleton<IDataParser<PortraitPack>, PortraitPackParser>(typeof(PortraitPack));
            services.AddKeyedSingleton<IDataParser<RewardPortrait>, RewardPortraitParser>(typeof(RewardPortrait));
            services.AddKeyedSingleton<IDataParser<Skin>, SkinParser>(typeof(Skin));
            services.AddKeyedSingleton<IDataParser<Spray>, SprayParser>(typeof(Spray));
            services.AddKeyedSingleton<IDataParser<TypeDescription>, TypeDescriptionParser>(typeof(TypeDescription));
            services.AddKeyedSingleton<IDataParser<Unit>, UnitParser>(typeof(Unit));
            services.AddKeyedSingleton<IDataParser<Veterancy>, VeterancyParser>(typeof(Veterancy));
            services.AddKeyedSingleton<IDataParser<VoiceLine>, VoiceLineParser>(typeof(VoiceLine));

            services.AddSingleton<IDataParser<Map>, MapParser>();

            // special for unit parser
            services.AddSingleton<IUnitParser, UnitParser>();

            return services;
        }

        private IServiceCollection AddImageWriters()
        {
            // add all image writers
            services.AddKeyedSingleton<IImageParser<Announcer>, AnnouncerImageParser>(typeof(Announcer));
            services.AddKeyedSingleton<IImageParser<Bundle>, BundleImageParser>(typeof(Bundle));
            services.AddKeyedSingleton<IImageParser<Emoticon>, EmoticonImageParser>(typeof(Emoticon));
            services.AddKeyedSingleton<IImageParser<MatchAward>, MatchAwardImageParser>(typeof(MatchAward));
            services.AddKeyedSingleton<IImageParser<Spray>, SprayImageParser>(typeof(Spray));
            services.AddKeyedSingleton<IImageParser<TypeDescription>, TypeDescriptionImageParser>(typeof(TypeDescription));
            services.AddKeyedSingleton<IImageParser<VoiceLine>, VoiceLineImageParser>(typeof(VoiceLine));

            services.AddKeyedSingleton<IImageParser<Map>, ReplayPreviewImageParser>(typeof(Map));
            services.AddKeyedSingleton<IImageParser<Map>, LoadingScreenImageParser>(typeof(Map));
            services.AddKeyedSingleton<IImageParser<Map>, MapObjectiveIconImageParser>(typeof(Map));

            services.AddKeyedSingleton<IImageParser<Hero>, HeroTalentImageParser>(typeof(Hero));
            services.AddKeyedSingleton<IImageParser<Hero>, HeroAbilityImageParser>(typeof(Hero));
            services.AddKeyedSingleton<IImageParser<Hero>, HeroAbilityTalentImageParser>(typeof(Hero));
            services.AddKeyedSingleton<IImageParser<Hero>, HeroPortraitImageParser>(typeof(Hero));
            services.AddKeyedSingleton<IImageParser<Unit>, UnitPortraitImageParser>(typeof(Unit));
            services.AddKeyedSingleton<IImageParser<Unit>, UnitAbilityImageParser>(typeof(Unit));
            services.AddKeyedSingleton<IImageParser<Unit>, UnitAbilityTalentImageParser>(typeof(Unit));

            return services;
        }

        private IServiceCollection AddCommandServices()
        {
            services.AddSingleton<ICASCExtractorService, CASCExtractorService>();
            services.AddSingleton<IJsonApplyService, JsonApplyService>();
            services.AddSingleton<IJsonCreateService, JsonCreateService>();
            services.AddSingleton<IGameStringTextUpdateService, GameStringTextFormatService>();
            services.AddSingleton<IJsonSchemaExporterService, JsonSchemaExporterService>();
            services.AddSingleton<IPortraitInfoService, PortraitInfoService>();
            services.AddSingleton<IPortraitBattleNetCacheService, PortraitBattleNetCacheService>();
            services.AddSingleton<IPortraitExtractAutoService, PortraitExtractAutoService>();
            services.AddSingleton<IPortraitExtractService, PortraitExtractService>();
            services.AddSingleton<ILocalizedTextImportService, LocalizedTextImportService>();
            services.AddSingleton<ILocalizedTextExportService, LocalizedTextExportService>();

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
