using HeroesDataParser.Infrastructure.ImageWriters;
using Serilog;

namespace HeroesDataParser.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHDPServices(this IServiceCollection services, HostApplicationBuilder builder)
    {
        services.AddSerilog();

        services.Configure<RootOptions>(builder.Configuration.GetSection(nameof(RootOptions)));
        services.AddSingleton(builder.Environment.ContentRootFileProvider);

        services.AddSingleton<IParsingConfigurationService, ParsingConfigurationService>();
        services.AddSingleton<IPreloadService, PreloadService>();

        services.AddSingleton<IHeroesXmlLoaderService, HeroesXmlLoaderService>();

        services.AddDataParsers();
        services.AddImageWriters();

        services.AddSingleton<IProcessorService, ProcessorService>();
        services.AddSingleton<IMapProcessorService, MapProcessorService>();

        services.AddSingleton<IDataParserService, DataParserService>();
        services.AddSingleton<IMapDataParserService, MapDataParserService>();

        services.AddSingleton<IDataExtractorService, DataExtractorService>();
        services.AddSingleton<IMapDataExtractorService, MapDataExtractorService>();

        services.AddSingleton<IJsonFileWriterService, JsonFileWriterService>();

        return services;
    }

    public static IServiceCollection AddDataParsers(this IServiceCollection services)
    {
        // add all data parsers
        services.AddSingleton<IDataParser<Announcer>, AnnouncerParser>();
        services.AddSingleton<IDataParser<Banner>, BannerParser>();
        services.AddSingleton<IDataParser<Boost>, BoostParser>();
        services.AddSingleton<IDataParser<Bundle>, BundleParser>();
        services.AddSingleton<IDataParser<Hero>, HeroParser>();
        services.AddSingleton<IDataParser<LootChest>, LootChestParser>();

        services.AddSingleton<IDataParser<Map>, MapParser>();
        return services;
    }

    public static IServiceCollection AddImageWriters(this IServiceCollection services)
    {
        // add all image writers
        services.AddTransient<IImageWriter<Announcer>, AnnouncerImageWriter>();
        services.AddTransient<IImageWriter<Bundle>, BundleImageWriter>();
        services.AddTransient<IImageWriter<Map>, MapImageWriter>();

        return services;
    }
}
