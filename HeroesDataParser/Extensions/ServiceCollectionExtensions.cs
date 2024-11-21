namespace HeroesDataParser.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataParsers(this IServiceCollection services)
    {
        // add all data parsers
        services.AddScoped<IDataParser<Announcer>, AnnouncerParser>();
        services.AddScoped<IDataParser<Banner>, BannerParser>();

        services.AddScoped<IDataParser<Map>, MapParser>();

        //services.AddScoped(typeof(IDataExtractorService<,>), typeof(DataExtractorService<,>));
        //services.AddScoped<IDataExtractorService<Map, IDataParser<Map>>, MapDataExtractorService>();

        return services;
    }
}
