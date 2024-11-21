using Serilog;

// TODO: CLI

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Verbose()
    .WriteTo.Async(x => x.File(new CompactJsonFormatter(), $"log{DateTime.Now:yyyyMMdd_HHmmss}.txt", retainedFileCountLimit: 7, fileSizeLimitBytes: 1024 * 1024 * 64), bufferSize: 500, blockWhenFull: true)
    .CreateLogger();

try
{
    Log.Information("HDP v5.0.0-alpha.1");
    Log.Information("Starting loading of heroes data");
    HeroesXmlLoader heroesXmlLoader = await HeroesDataLoader.Load();
    Log.Information("Completed loading of heroes data");

    builder.Services.AddSerilog();
    builder.Configuration
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            // TODO: from cli options, set here
            //["RootOptions:OutputDirectory"] = "mypath",
        });

    // options
    builder.Services.Configure<RootOptions>(builder.Configuration.GetSection(nameof(RootOptions)));

    // other services
    builder.Services.AddScoped<IHeroesXmlLoaderService>(provider => new HeroesXmlLoaderService(heroesXmlLoader));
    builder.Services.AddScoped<IHeroesDataService>(provider => new HeroesDataService(heroesXmlLoader.HeroesData));

    builder.Services.AddDataParsers();

    builder.Services.AddScoped<IProcessorService, ProcessorService>();
    builder.Services.AddScoped<IMapProcessorService, MapProcessorService>();

    builder.Services.AddScoped<IDataParserService, DataParserService>();
    builder.Services.AddScoped<IMapDataParserService, MapDataParserService>();

    builder.Services.AddScoped<IDataExtractorService, DataExtractorService>();
    builder.Services.AddScoped<IMapDataExtractorService, MapDataExtractorService>();

    builder.Services.AddScoped<IJsonFileWriterService, JsonFileWriterService>();

    using IHost host = builder.Build();


    var a = host.Services.GetRequiredService<IProcessorService>();
    await a.Start(StormLocale.ENUS);

    var b = host.Services.GetRequiredService<IMapProcessorService>();
    await b.Start(StormLocale.ENUS);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application error");
}
finally
{
    Log.Information("HPD done");

    await Log.CloseAndFlushAsync();
}