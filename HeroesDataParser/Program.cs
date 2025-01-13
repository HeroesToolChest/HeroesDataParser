using Serilog;
using System.Globalization;

// TODO: CLI

Console.WriteLine($"Heroes Data Parser ({AppVersion.GetAppVersion()})");
Console.WriteLine("  --https://github.com/HeroesToolChest/HeroesDataParser");
Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
Console.WriteLine();

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

SetAppCulture();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Verbose()
    .WriteTo.Async(x => x.File(new CompactJsonFormatter(), $"log{DateTime.Now:yyyyMMdd_HHmmss}.txt", retainedFileCountLimit: 7, fileSizeLimitBytes: 1024 * 1024 * 64), bufferSize: 500, blockWhenFull: true)
    .CreateLogger();

try
{
    Log.Information($"HDP v{AppVersion.GetAppVersion()}");

    // TODO: from cli
    //int? theCliBuildNumber = null;

    builder.Configuration
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            // TODO: from cli options, set here
            //["RootOptions:OutputDirectory"] = "mypath",
            //["RootOptions:BuildNumber"] = theCliBuildNumber?.ToString(),
        });

    builder.Services.AddHDPServices(builder);

    using IHost host = builder.Build();

    var loading = host.Services.GetRequiredService<IHeroesDataLoaderService>();
    await loading.Load();

    var a = host.Services.GetRequiredService<IProcessorService>();
    await a.Start(StormLocale.ENUS);

    var b = host.Services.GetRequiredService<IMapProcessorService>();
    await b.Start(StormLocale.ENUS);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application error");

    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("An application error occured. Check logs for details.");
    Console.ResetColor();
}
finally
{
    Log.Information("HPD done");

    await Log.CloseAndFlushAsync();
}

static void SetAppCulture()
{
    CultureInfo cultureInfo = new("en-US");
    CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
    CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
}