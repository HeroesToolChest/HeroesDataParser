using Serilog;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

SetAppCulture();



//AnsiConsole.MarkupLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
//AnsiConsole.MarkupLine($"[bold]Heroes Data Parser v{AppVersion.GetAppVersion()}[/]");
//AnsiConsole.MarkupLine("  --[link]https://github.com/HeroesToolChest/HeroesDataParser[/]");
//AnsiConsole.MarkupLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
//AnsiConsole.WriteLine();

//HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Async(SerilogLogging.LoggerConfigure(), bufferSize: 500, blockWhenFull: true)
    .CreateLogger();
//.CreateBootstrapLogger();

try
{
    Log.Information($"HDP v{AppVersion.GetAppVersion()}");

    ServiceCollection services = new();
    services.AddConfiguration();
    services.AddCoreServices();
    services.AddHDPServices();
    services.AddResiliencePipelines();
    services.PostConfigureAll<RootOptions>(options =>
    {
        options.CurrentLocale = StormLocale.ENUS;
        options.AppVersion = AppVersion.GetAppVersion();

        if (!options.GameStringText.ReplaceFontStyles)
        {
            options.GameStringText.PreserveFont.PreserveFontStyleConstantVars = false;
            options.GameStringText.PreserveFont.PreserveFontStyleVars = false;
        }
    });

    TypeRegistrar registrar = new(services);
    CommandApp<RootCommand> app = new(registrar);
    app.Configure(config =>
    {
        config.SetApplicationName(Constants.AppNameLower);
        config.SetApplicationVersion(AppVersion.GetAppVersion());
        config.UseStrictParsing();
        config.CaseSensitivity(CaseSensitivity.None);
#if DEBUG
        config.PropagateExceptions();
        config.ValidateExamples();
#endif
    });

    await app.RunAsync(args);

    //// TODO: from cli
    ////int? theCliBuildNumber = null;

    ////builder.Configuration
    ////    .AddInMemoryCollection(new Dictionary<string, string?>
    ////    {
    ////        // TODO: from cli options, set here
    ////        //["RootOptions:OutputDirectory"] = "mypath",
    ////        //["RootOptions:BuildNumber"] = theCliBuildNumber?.ToString(),
    ////    });

    //builder.Services.AddCoreServices(builder);
    //builder.Services.AddHDPServices(builder);
    //builder.Services.AddResiliencePipelines();

    //builder.Services.PostConfigureAll<RootOptions>(options =>
    //{
    //    options.CurrentLocale = StormLocale.ENUS;
    //    options.AppVersion = AppVersion.GetAppVersion();

    //    if (!options.GameStringText.ReplaceFontStyles)
    //    {
    //        options.GameStringText.PreserveFont.PreserveFontStyleConstantVars = false;
    //        options.GameStringText.PreserveFont.PreserveFontStyleVars = false;
    //    }
    //});

    //using IHost host = builder.Build();

    //IPreloadService preload = host.Services.GetRequiredService<IPreloadService>();
    //PreloadData preloadData = preload.Preload();

    //IHeroesXmlLoaderService loading = host.Services.GetRequiredService<IHeroesXmlLoaderService>();
    //await loading.Load(preloadData);

    //IMainService main = host.Services.GetRequiredService<IMainService>();
    //await main.Start();

    //IPostCleanupService postCleanup = host.Services.GetRequiredService<IPostCleanupService>();
    //postCleanup.Start();

    //IResultSummaryService resultSummary = host.Services.GetRequiredService<IResultSummaryService>();
    //resultSummary.PrintSummary();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application error");

    AnsiConsole.WriteLine("An application error occured. Check logs for more details.");
    AnsiConsole.WriteException(ex);
}
finally
{
    Log.Information("HPD done");

    RunLogRententionPolicy();

    await Log.CloseAndFlushAsync();
}

static void SetAppCulture()
{
    CultureInfo cultureInfo = new("en-US");
    CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
    CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
}

static void RunLogRententionPolicy()
{
    FileInfo[] allLogFiles = new DirectoryInfo(SerilogLogging.LogDirectory)
       .GetFiles($"{SerilogLogging.LogPrefix}*.txt");

    Log.Information($"Log Retention: Found {allLogFiles.Length} log files. Keeping latest {SerilogLogging.RetainedFileCountLimit} log files");

    IEnumerable<FileInfo> toBeDeletedLogFiles = allLogFiles
       .OrderByDescending(x => x.CreationTime)
       .Skip(SerilogLogging.RetainedFileCountLimit);

    foreach (FileInfo file in toBeDeletedLogFiles)
    {
        try
        {
            file.Delete();
            Log.Information("Deleted old log file: {FileName}", file.Name);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to delete old log file: {FileName}", file.Name);
        }
    }
}
