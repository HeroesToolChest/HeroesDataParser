using Microsoft.Extensions.Configuration;
using Serilog;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

SetAppCulture();

IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), optional: false, reloadOnChange: false)
    .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.release.json"), optional: true, reloadOnChange: false)
    .Build();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(configuration)
    .WriteTo.Async(SerilogLogging.LoggerConfigure(), bufferSize: 500, blockWhenFull: true)
    .CreateLogger();

int exitCode = 0;

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

        if (!options.GameStringText.ReplaceFontConstantVars)
            options.GameStringText.PreserveFontStyleConstantVars = false;

        if (!options.GameStringText.ReplaceFontStylesVars)
            options.GameStringText.PreserveFontStyleVars = false;
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
            .WithDescription("Extract data files from a 'Heroes of the Storm' directory or from online");

        config.AddBranch<JsonPatchSettings>("json-patch", jsonPatch =>
        {
            jsonPatch.SetDescription("Json-patching operations");

            jsonPatch.AddCommand<JsonApplyCommand>("apply")
                .WithDescription("Patch a JSON file with a JSON patch file");

            jsonPatch.AddCommand<JsonCreateCommand>("create")
                .WithDescription("Create a new JSON patch file from two JSON files");
        });

        config.AddBranch<GameStringTextSettings>("gamestring-text", gamestringText =>
        {
            gamestringText.SetDescription("Gamestring text operations");

            gamestringText.AddCommand<GameStringTextFormatCommand>("format")
                .WithDescription("Format the gamestring text in a JSON file");
        });

        config.AddBranch<JsonSchemaSettings>("schema", schema =>
        {
            schema.SetDescription("Json schema operations");

            schema.AddBranch<JsonSchemaExportSettings>("export", export =>
            {
                export.SetDescription("Json schema export operations");
                export.AddCommand<JsonSchemaExportDataCommand>("data")
                    .WithDescription("Export the JSON schema for the data files");

                export.AddCommand<JsonSchemaExportGameStringCommand>("gamestrings")
                    .WithDescription("Export the JSON schema for the gamestrings file");
            });
        });

        config.AddBranch<PortraitSettings>("portrait", portrait =>
        {
            portrait.SetDescription("Reward portrait data operations");
            portrait.AddCommand<PortraitInfoCommand>("info")
                .WithDescription("Display information about the reward portraits data file");

            portrait.AddCommand<PortraitBattleNetCacheCommand>("battlenet-cache")
                .WithDescription("Copy .wafl files from the Battle.net cache directory (or a custom directory) to the output directory");

            portrait.AddCommand<PortraitExtractCommand>("extract")
                .WithDescription("Extract reward portraits from the texture sheets");

            portrait.AddCommand<PortraitExtractAutoCommand>("extract-auto")
                .WithDescription("Automatically extract reward portraits from the texture sheets");
        });
    });

    exitCode = await app.RunAsync(args);
}
catch (CommandParseException ex)
{
    Log.Error(ex, "Command line parsing error");
    AnsiConsole.WriteException(ex);

    exitCode = -1;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application error");

    AnsiConsole.WriteLine("An application error occured. Check logs for more details.");
    AnsiConsole.WriteException(ex);

    exitCode = 1;
}
finally
{
    Log.Information("HPD done");

    RunLogRententionPolicy();

    await Log.CloseAndFlushAsync();
}

return exitCode;

static void SetAppCulture()
{
    CultureInfo cultureInfo = new("en-US");
    CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
    CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
}

static void RunLogRententionPolicy()
{
    FileInfo[] allLogFiles = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, SerilogLogging.LogDirectory))
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
