using CASCLib;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace HeroesDataParser.Infrastructure;

public class HeroesDataLoaderService : IHeroesDataLoaderService
{
    private readonly ILogger<HeroesDataLoaderService> _logger;
    private readonly RootOptions _options;

    public HeroesDataLoaderService(ILogger<HeroesDataLoaderService> logger, IOptions<RootOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public HeroesXmlLoader HeroesXmlLoader { get; private set; } = null!;

    public async Task Load()
    {
        _logger.LogInformation("Loading heroes data...");

        using BackgroundWorkerEx backgroundWorkerEx = new();
        backgroundWorkerEx.DoWork += (_, e) =>
        {
            if (_options.StorageLoad.Type == StorageType.Game && IsValidPath())
            {
                _logger.LogInformation("Loading heroes data with game storage.");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Found 'Heroes of the Storm' directory");
                Console.WriteLine();
                Console.WriteLine("Loading local CASC storage... please wait");
                Console.ResetColor();

                HeroesXmlLoader = HeroesXmlLoader.LoadWithCASC(_options.StorageLoad.Path!, backgroundWorkerEx);
            }
            else if (_options.StorageLoad.Type == StorageType.Mods && IsValidPath())
            {
                _logger.LogInformation("Loading heroes data with mods storage.");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Found 'mods' directory");
                Console.WriteLine();
                Console.WriteLine("Loading local mods storage... please wait");
                Console.ResetColor();

                HeroesXmlLoader = HeroesXmlLoader.LoadWithFile(_options.StorageLoad.Path!, backgroundWorkerEx);
            }
            else if (_options.StorageLoad.Type == StorageType.Online)
            {
                _logger.LogInformation("Loading heroes data with online storage.");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Loading online CASC storage... please wait");
                Console.ResetColor();

                HeroesXmlLoader = HeroesXmlLoader.LoadWithOnlineCASC(backgroundWorkerEx);
            }
            else
            {
                _logger.LogWarning("Unknown storage load type, defaulting to online storage.");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Unknown storage load type, defaulting to online storage.");
                Console.WriteLine();
                Console.WriteLine("Loading online CASC storage... please wait");
                Console.ResetColor();

                HeroesXmlLoader = HeroesXmlLoader.LoadWithOnlineCASC(backgroundWorkerEx);
            }

            HeroesXmlLoader
                .LoadStormMods()
                .LoadGameStrings();
        };

        string? currentMessage = string.Empty;

        backgroundWorkerEx.ProgressChanged += (_, e) =>
        {
            Task.Run(() =>
            {
                //currentMessage = e.UserState?.ToString();

                //if (!string.IsNullOrWhiteSpace(currentMessage))
                //{
                    
                //}
                //Console.WriteLine($"{e.UserState}{" ",-50}");
                //Console.Write($"{e.ProgressPercentage}%{" ",-5}");

                //Console.CursorLeft = 0;
                //Console.CursorTop = Console.CursorTop - 1;

                //Console.Write($"\r{e.ProgressPercentage}% - {e.UserState}{" ",-10}");
                Console.WriteLine($"{e.ProgressPercentage}% - {e.UserState}");
            });
        };
        backgroundWorkerEx.RunWorkerCompleted += (_, e) =>
        {
            Console.WriteLine("done");
        };

        backgroundWorkerEx.RunWorkerAsync();

        while (backgroundWorkerEx.IsBusy)
        {
            await Task.Delay(1000);
        }

        if (HeroesXmlLoader is null)
            throw new InvalidOperationException("Failed to load heroes data");

        _logger.LogInformation("Heroes data loaded.");
    }

    private bool IsValidPath()
    {
        if (string.IsNullOrWhiteSpace(_options.StorageLoad.Path))
        {
            _logger.LogCritical("StorageLoad path is empty.");
            Console.WriteLine("Error: The storage load path is empty. Please provide a path to the game or mods directory.");

            return false;
        }

        if (!Directory.Exists(_options.StorageLoad.Path))
        {
            _logger.LogCritical("StorageLoad path does not exist.");
            Console.WriteLine("Error: The storage load path does not exist. Please provide a valid path to the game or mods directory.");

            return false;
        }

        return true;
    }
}
