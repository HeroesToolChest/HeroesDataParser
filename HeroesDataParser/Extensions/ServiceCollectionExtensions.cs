using HeroesDataParser.Infrastructure.ImageWriters;

namespace HeroesDataParser.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataParsers(this IServiceCollection services)
    {
        // add all data parsers
        services.AddScoped<IDataParser<Announcer>, AnnouncerParser>();
        services.AddScoped<IDataParser<Banner>, BannerParser>();
        services.AddScoped<IDataParser<Boost>, BoostParser>();
        services.AddScoped<IDataParser<Bundle>, BundleParser>();
        services.AddScoped<IDataParser<LootChest>, LootChestParser>();

        services.AddScoped<IDataParser<Map>, MapParser>();

        //services.AddScoped(typeof(IDataExtractorService<,>), typeof(DataExtractorService<,>));
        //services.AddScoped<IDataExtractorService<Map, IDataParser<Map>>, MapDataExtractorService>();

        return services;
    }

    public static IServiceCollection AddImageWriters(this IServiceCollection services)
    {
        // add all image writers
        services.AddTransient<IImageWriter<Announcer>, AnnouncerImageWriter>();
        services.AddTransient<IImageWriter<Bundle>, BundleImageWriter>();

        return services;
    }
}
