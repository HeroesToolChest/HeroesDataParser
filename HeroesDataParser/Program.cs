using Microsoft.Extensions.Configuration;
using Serilog;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

SetAppCulture();

IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile("appsettings.release.json", optional: true, reloadOnChange: false)
    .Build();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(configuration)
    .WriteTo.Async(SerilogLogging.LoggerConfigure(), bufferSize: 500, blockWhenFull: true)
    .CreateLogger();

try
{
    Log.Information($"HDP v{AppVersion.GetAppVersion()}");

    ServiceCollection services = new();
    services.AddConfiguration(configuration);
    services.AddCoreServices();
    services.AddHDPServices();
    services.AddResiliencePipelines();
    services.PostConfigureAll<RootOptions>(options =>
    {
        options.CurrentLocale = StormLocale.ENUS;
        options.AppVersion = AppVersion.GetAppVersion();

        if (options.GameStringText.PreserveFont.PreserveFontStyleConstantVars || options.GameStringText.PreserveFont.PreserveFontStyleVars)
            options.GameStringText.ReplaceFontStyles = true;
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
        config.AddCommand<CASCExtractCommand>("casc-extract")
            .WithDescription("Extract the data files from a 'Heroes of the Storm' directory or from online");
    });

    await app.RunAsync(args);
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
