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

        services.AddActivatedSingleton<IHeroesDataLoaderService, HeroesDataLoaderService>();
        services.AddActivatedSingleton<IParsingConfigurationService, ParsingConfigurationService>();

        services.AddDataParsers();
        services.AddImageWriters();

        services.AddScoped<IProcessorService, ProcessorService>();
        services.AddScoped<IMapProcessorService, MapProcessorService>();

        services.AddScoped<IDataParserService, DataParserService>();
        services.AddScoped<IMapDataParserService, MapDataParserService>();

        services.AddScoped<IDataExtractorService, DataExtractorService>();
        services.AddScoped<IMapDataExtractorService, MapDataExtractorService>();

        services.AddScoped<IJsonFileWriterService, JsonFileWriterService>();

        return services;
    }

    public static IServiceCollection AddDataParsers(this IServiceCollection services)
    {
        // add all data parsers
        services.AddScoped<IDataParser<Announcer>, AnnouncerParser>();
        services.AddScoped<IDataParser<Banner>, BannerParser>();
        services.AddScoped<IDataParser<Boost>, BoostParser>();
        services.AddScoped<IDataParser<Bundle>, BundleParser>();
        services.AddScoped<IDataParser<LootChest>, LootChestParser>();

        services.AddScoped<IDataParser<Map>, MapParser>();
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
