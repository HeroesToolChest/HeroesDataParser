﻿using Microsoft.AspNetCore.Identity.Data;
using Serilog;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

// TODO: CLI

AnsiConsole.MarkupLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
AnsiConsole.MarkupLine($"[bold]Heroes Data Parser[/] ({AppVersion.GetAppVersion()})");
AnsiConsole.MarkupLine("  --[link]https://github.com/HeroesToolChest/HeroesDataParser[/]");
AnsiConsole.MarkupLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
AnsiConsole.WriteLine();

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

SetAppCulture();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Verbose()
    .WriteTo.Async(x => x.File(new CompactJsonFormatter(), Path.Join(SerilogLogging.LogDirectory, $"{SerilogLogging.LogPrefix}{DateTime.Now:yyyyMMdd_HHmmss}.txt"), retainedFileCountLimit: SerilogLogging.RetainedFileCountLimit, fileSizeLimitBytes: 1024 * 1024 * 64), bufferSize: 500, blockWhenFull: true)
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

    IEnumerable<FileInfo> tobeDeletedLogFiles = allLogFiles
       .OrderByDescending(x => x.CreationTime)
       .Skip(SerilogLogging.RetainedFileCountLimit);

    foreach (FileInfo file in tobeDeletedLogFiles)
    {
        try
        {
            file.Delete();
            Log.Information($"Deleted old log file: {file.Name}");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Failed to delete old log file: {file.Name}");
        }
    }
}