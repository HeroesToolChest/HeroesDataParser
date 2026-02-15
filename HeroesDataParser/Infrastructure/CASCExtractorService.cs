using CASCLib;
using HeroesDataParser.Options.CASCExtractOptions;
using Polly;
using Polly.Registry;

namespace HeroesDataParser.Infrastructure;

public class CASCExtractorService : ICASCExtractorService
{
    private readonly ILogger<CASCExtractorService> _logger;
    private readonly CASCExtractOptions _options;
    private readonly IAnsiConsole _console;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ResiliencePipeline _pipeline;

    private readonly Stopwatch _stopwatch = new();

    public CASCExtractorService(
        ILogger<CASCExtractorService> logger,
        IOptions<CASCExtractOptions> options,
        IAnsiConsole console,
        IHttpClientFactory httpClientFactory,
        ResiliencePipelineProvider<string> pipelineProvider)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;
        _httpClientFactory = httpClientFactory;
        _pipeline = pipelineProvider.GetPipeline(Constants.CASCFileExtractorPipeline);
    }

    public async Task RootDirectoryExtract()
    {
        _logger.LogInformation("Load storage type {StorageType}", _options.StorageLoad.Type);

        CASCConfig cascConfig = GetCASCConfig();

        HeroesXmlLoader? heroesXmlLoader = await LoadFromCASC(cascConfig) ?? throw new InvalidOperationException("Failed to load from casc.");

        _console.MarkupLineInterpolated($"Load time: {_stopwatch.Elapsed.TotalSeconds:0.####} seconds");

        await ExtractFiles(heroesXmlLoader);
    }

    private static string NormalizePath(ReadOnlySpan<char> filePath)
    {
        if (filePath.IsEmpty || filePath.IsWhiteSpace())
            return string.Empty;

        Span<char> buffer = stackalloc char[filePath.Length];
        filePath.CopyTo(buffer);

        NormalizePath(buffer);

        return buffer.ToString();
    }

    private static void NormalizePath(Span<char> filePath)
    {
        if (filePath.IsEmpty)
            return;

        for (int i = 0; i < filePath.Length; i++)
        {
            if (filePath[i] is '/' or '\\')
                filePath[i] = Path.DirectorySeparatorChar;
            else
                filePath[i] = char.ToLowerInvariant(filePath[i]);
        }
    }

    private static IEnumerable<CASCFile> EnumerateDirectory(CASCFolder gameDataFolder)
    {
        foreach (KeyValuePair<string, CASCFile> file in gameDataFolder.Files)
        {
            yield return file.Value;
        }

        foreach (KeyValuePair<string, CASCFolder> folder in gameDataFolder.Folders)
        {
            foreach (CASCFile file in EnumerateDirectory(folder.Value))
            {
                yield return file;
            }
        }
    }

    private HashSet<string> GetFiles(HeroesXmlLoader heroesXmlLoader)
    {
        HashSet<string> files = new(StringComparer.OrdinalIgnoreCase);

        foreach (string rootDirectory in _options.Directories)
        {
            CASCFolder folder = null!;

            try
            {
                folder = heroesXmlLoader.GetCASCFolder(rootDirectory);
            }
            catch (DirectoryNotFoundException)
            {
                _logger.LogError("Root directory not found in storage: {RootDirectory}", rootDirectory);
                _console.MarkupLineInterpolated($"[red]Error: Root directory not found in storage: {rootDirectory}[/]");
                continue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during extraction of root directory: {RootDirectory}", rootDirectory);
                _console.MarkupLineInterpolated($"[red]An error occurred during extraction of root directory: {rootDirectory}: {ex.Message}[/]");
                continue;
            }

            IEnumerable<string> enumeratedFiles = EnumerateDirectory(folder)
                .Where(x => _options.FileFilters.Contains("*") || _options.FileFilters.Any(filter => Path.GetExtension(x.Name.AsSpan()).Equals(filter, StringComparison.OrdinalIgnoreCase)))
                .Select(x => NormalizePath(x.FullName))
                .OrderBy(x => x);

            foreach (string file in enumeratedFiles)
                files.Add(file);
        }

        return files;
    }

    private async Task<HeroesXmlLoader?> LoadFromCASC(CASCConfig cascConfig)
    {
        HeroesXmlLoader? heroesXmlLoader = null;

        await _console.Progress()
            .Columns(
            [
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
            ])
            .StartAsync(async ctx =>
            {
                ProgressTask progressTask = ctx.AddTask(_options.StorageLoad.Type == StorageType.Game ? "Loading Local" : "Loading Online");
                progressTask.MaxValue = 500;

                Progress<ProgressInfo> progress = new(p =>
                {
                    switch (p.Stage)
                    {
                        case ProgressStage.CDNIndexes:
                            progressTask.Value = p.Percentage;
                            break;
                        case ProgressStage.LocalIndexes:
                            progressTask.Value = 100 + p.Percentage;
                            break;
                        case ProgressStage.Encoding:
                            progressTask.Value = 200 + p.Percentage;
                            break;
                        case ProgressStage.Root:
                            progressTask.Value = 300 + p.Percentage;
                            break;
                        case ProgressStage.ListFile:
                            progressTask.Value = 400 + p.Percentage;
                            break;
                        default:
                            break;
                    }
                });

                DisplayHeroesVersion(cascConfig);
                DisplayStorageType();
                DisplayOutputDirectory();
                DisplayFileFilters();

                _stopwatch.Start();

                if (_options.StorageLoad.Type == StorageType.Game)
                {
                    await Task.Run(() =>
                    {
                        heroesXmlLoader = HeroesXmlLoader.LoadWithCASC(cascConfig, _httpClientFactory.CreateClient(Constants.HttpClientBlizzard), progressReporter: new ProgressReporter(progress));
                    });
                }
                else
                {
                    await Task.Run(() =>
                    {
                        heroesXmlLoader = HeroesXmlLoader.LoadWithCASC(cascConfig, _httpClientFactory.CreateClient(Constants.HttpClientBlizzard), progressReporter: new ProgressReporter(progress));
                    });
                }

                _stopwatch.Stop();
            });

        return heroesXmlLoader;
    }

    private async Task ExtractFiles(HeroesXmlLoader heroesXmlLoader)
    {
        _stopwatch.Restart();

        HashSet<string> files = GetFiles(heroesXmlLoader);

        int totalFiles = files.Count;

        ProgressTask progressTask = null!;

        await _console.Progress()
            .Columns(
            [
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new ItemsProgressColumn(),
            ])
            .StartAsync(async ctx =>
            {
                progressTask = ctx.AddTask("Extracting", maxValue: totalFiles);

                ParallelOptions parallelOptions = new()
                {
                    MaxDegreeOfParallelism = _options.Threads,
                };

                await Parallel.ForEachAsync(files, parallelOptions, async (filePath, cancellationToken) =>
                {
                    try
                    {
                        await _pipeline.ExecuteAsync(
                            async (_) =>
                            {
                                await CreateFile(heroesXmlLoader, filePath, cancellationToken);

                                progressTask.Increment(1);
                            },
                            cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to extract file: {FilePath}", filePath);
                    }
                });
            });

        _stopwatch.Stop();

        int success = (int)progressTask.Value;

        if (success < totalFiles)
            _console.MarkupLineInterpolated($"[yellow]Failed to extract {totalFiles - success} file(s)[/]");

        if (totalFiles > 0)
            await CreateInfoFile();

        _console.MarkupLineInterpolated($"Extraction completed in {_stopwatch.Elapsed.TotalSeconds:0.####} seconds");
        _logger.LogInformation("Extraction completed in {ElapsedSeconds} seconds", _stopwatch.Elapsed.TotalSeconds);
    }

    private void DisplayHeroesVersion(CASCConfig cascConfig)
    {
        HeroesDataVersion? heroesDataVersion = cascConfig.GetVersionFromCascConfig();
        if (heroesDataVersion is null)
        {
            _logger.LogWarning("Could not determine Heroes of the Storm data version from the selected storage");
            _console.MarkupLineInterpolated($"[yellow]Version: UNKNOWN[/]");
        }
        else
        {
            _options.HeroesVersion.Major = heroesDataVersion.Major;
            _options.HeroesVersion.Minor = heroesDataVersion.Minor;
            _options.HeroesVersion.Revision = heroesDataVersion.Revision;
            _options.HeroesVersion.Build = heroesDataVersion.Build;
            _options.HeroesVersion.IsPtr = heroesDataVersion.IsPtr;

            _console.MarkupLineInterpolated($"[aqua]Version: [bold]{_options.HeroesVersion.GetAsHeroesDataVersion()}[/][/]");
        }
    }

    private void DisplayStorageType()
    {
        if (_options.StorageLoad.Type == StorageType.Game)
        {
            _logger.LogInformation("Loading heroes data by game storage");

            _console.MarkupLine("[aqua]Storage: 'Heroes of the Storm' directory[/]");
        }
        else
        {
            _logger.LogInformation("Downloading heroes data by online storage");

            _console.MarkupLine("[aqua]Storage: Online[/]");
        }
    }

    private void DisplayOutputDirectory()
    {
        _logger.LogInformation("Root directory(s) for extraction: {RootDirectories}", string.Join(", ", _options.Directories));

        if (_options.Directories.Length == 1)
        {
            _console.MarkupLineInterpolated($"[aqua]Root Directory: {_options.Directories[0]}[/]");
        }
        else
        {
            _console.MarkupLineInterpolated($"[aqua]Root Directory(s)[/]");
            foreach (string rootDirectory in _options.Directories)
            {
                _console.MarkupLineInterpolated($"  [aqua]{rootDirectory}[/]");
            }
        }

        string fullOutputDirectory = Path.GetFullPath(_options.OutputDirectory);

        _logger.LogInformation("Output directory: {OutputDirectory}", fullOutputDirectory);
        _console.MarkupLineInterpolated($"[aqua]Output Directory: {fullOutputDirectory}[/]");
    }

    private void DisplayFileFilters()
    {
        if (_options.FileFilters.Length == 1)
        {
            _logger.LogInformation("No filters applied, extracting all files");
            _console.MarkupLine("[aqua]Filters: * (extracting all files)[/]");
        }
        else
        {
            _logger.LogInformation("Applying filters: {FileFilters}", string.Join(", ", _options.FileFilters));
            _console.MarkupLineInterpolated($"[aqua]Filters: {string.Join(", ", _options.FileFilters)}[/]");
        }
    }

    private CASCConfig GetCASCConfig()
    {
        if (_options.StorageLoad.Type == StorageType.Game)
        {
            return HeroesXmlLoader.GetCASCConfig(_options.StorageLoad.Path!, new CASCLoggerOptions());
        }
        else
        {
            return HeroesXmlLoader.GetOnlineCASCConfig(_httpClientFactory.CreateClient(Constants.HttpClientBlizzard), _options.StorageLoad.Ptr, new CASCLoggerOptions());
        }
    }

    private async Task<bool> CreateFile(HeroesXmlLoader heroesXmlLoader, string filePath, CancellationToken cancellationToken = default)
    {
        string outputFilePath = NormalizePath(Path.Combine(_options.OutputDirectory, filePath));

        string fullOutputRoot = Path.GetFullPath(_options.OutputDirectory);
        string fullOutputFile = Path.GetFullPath(outputFilePath);

        // guard check
        if (!fullOutputFile.StartsWith(fullOutputRoot, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Skipping file with path outside output directory: {FilePath}", filePath);
            return false;
        }

        string? directoryPath = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
            Directory.CreateDirectory(directoryPath);

        using Stream heroesFile = heroesXmlLoader.GetFile(filePath);
        using FileStream fileStream = File.Create(outputFilePath);

        await heroesFile.CopyToAsync(fileStream, cancellationToken);

        return true;
    }

    private async Task CreateInfoFile()
    {
        HeroesVersionOptions heroesVersionOptions = _options.HeroesVersion;
        HeroesDataVersion heroesDataVersion = new(heroesVersionOptions.Major, heroesVersionOptions.Minor, heroesVersionOptions.Revision, heroesVersionOptions.Build, isPtr: false);

        ModsInfoFile modsInfoFile = new()
        {
            Version = heroesDataVersion.GetAsVersionString(),
            IsPtr = heroesVersionOptions.IsPtr,
            HdpVersion = AppVersion.GetAppVersion(),
            ExtractedDate = DateTimeOffset.UtcNow,
        };

        await using FileStream fileStream = File.Create(Path.Combine(_options.OutputDirectory, "mods", HeroesXmlLoader.ModsHdpInfoFileName));
        await JsonSerializer.SerializeAsync(fileStream, modsInfoFile, new JsonSerializerOptions()
        {
            WriteIndented = true,
        });
    }
}
