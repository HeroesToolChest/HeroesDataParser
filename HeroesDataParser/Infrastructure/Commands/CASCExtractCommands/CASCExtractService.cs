using CASCLib;
using Microsoft.Extensions.FileSystemGlobbing;
using Polly;
using Polly.Registry;

namespace HeroesDataParser.Infrastructure.Commands.CASCExtractCommands;

public class CASCExtractService : ICASCExtractService
{
    private const string _rootDirectory = "mods";

    private readonly ILogger<CASCExtractService> _logger;
    private readonly CASCExtractOptions _options;
    private readonly IAnsiConsole _console;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ResiliencePipeline _pipeline;

    private readonly Stopwatch _stopwatch = new();

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true,
    };

    public CASCExtractService(
        ILogger<CASCExtractService> logger,
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

    public async Task<bool> RootDirectoryExtract()
    {
        _logger.LogInformation("Load storage type {StorageType}", _options.StorageLoad.Type);

        CASCConfig cascConfig = GetCASCConfig();

        HeroesXmlLoader? heroesXmlLoader = await LoadFromCASC(cascConfig) ?? throw new InvalidOperationException("Failed to load from casc.");

        _console.MarkupLineInterpolated($"Load time: {_stopwatch.Elapsed.TotalSeconds:0.####} seconds");

        return await ExtractFiles(heroesXmlLoader);
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

    private static string GetAppendedName(string fileName, Dictionary<string, string> originalFilePathByFileName, Dictionary<string, int> suffixCounterByFileName)
    {
        string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        string extension = Path.GetExtension(fileName);

        for (int i = suffixCounterByFileName.GetValueOrDefault(fileName) + 1; ; i++)
        {
            string newFileName = $"{nameWithoutExtension} ({i}){extension}";

            // check if the new file name already exists as a file name
            if (originalFilePathByFileName.ContainsKey(newFileName))
                continue;

            suffixCounterByFileName[fileName] = i;

            return newFileName;
        }
    }

    private static Dictionary<string, string> GetOutputPaths(IEnumerable<string> filteredFiles)
    {
        Dictionary<string, string> outputPathByOriginalFilePath = new(StringComparer.OrdinalIgnoreCase);

        foreach (string file in filteredFiles)
        {
            outputPathByOriginalFilePath[file] = NormalizePath(file);
        }

        return outputPathByOriginalFilePath;
    }

    private async Task<bool> ExtractFiles(HeroesXmlLoader heroesXmlLoader)
    {
        _stopwatch.Restart();

        // filtered files to extract based on include/exclude filters
        Dictionary<string, string>? outputPathByOriginalFilePath = GetFilePaths(heroesXmlLoader);

        if (outputPathByOriginalFilePath is null)
            return false;

        int totalFiles = outputPathByOriginalFilePath.Count;

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

                await Parallel.ForEachAsync(outputPathByOriginalFilePath, parallelOptions, async (entry, cancellationToken) =>
                {
                    try
                    {
                        await _pipeline.ExecuteAsync(
                            async (_) =>
                            {
                                await CreateFile(heroesXmlLoader, entry.Key, entry.Value, cancellationToken);

                                progressTask.Increment(1);
                            },
                            cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to extract file: {FilePath}", entry.Key);
                    }
                });
            });

        _stopwatch.Stop();

        int success = (int)progressTask.Value;

        if (success < totalFiles)
        {
            int failedCount = totalFiles - success;

            _console.MarkupLineInterpolated($"[yellow]Failed to extract {failedCount} file(s)[/]");
            _logger.LogWarning("Failed to extract {FailedCount} file(s)", failedCount);
        }

        if (success > 0 && !_options.Flatten)
            await CreateInfoFile();

        _console.MarkupLineInterpolated($"Total files extracted: {success}");
        _console.MarkupLineInterpolated($"Extraction completed in {_stopwatch.Elapsed.TotalSeconds:0.####} seconds");

        _logger.LogInformation("Extraction completed in {ElapsedSeconds} seconds", _stopwatch.Elapsed.TotalSeconds);
        _logger.LogInformation("Total files extracted: {success}", success);

        return true;
    }

    private CASCConfig GetCASCConfig()
    {
        if (_options.StorageLoad.Type == StorageType.Game)
        {
            return HeroesXmlLoader.GetCASCConfig(_options.StorageLoad.Path!, new CASCLoggerOptions());
        }
        else
        {
            CascLibOptions cascLibOptions = new()
            {
                CachePath = AppPaths.GetDirectory(Constants.CacsLibCacheDirectory),
            };

            return HeroesXmlLoader.GetOnlineCASCConfig(_httpClientFactory.CreateClient(Constants.HttpClientBlizzard), _options.StorageLoad.Ptr, new CASCLoggerOptions(), cascLibOptions);
        }
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
                DisplayFileFilters();
                DisplayOutputDirectory();

                _stopwatch.Start();

                await Task.Run(() =>
                {
                    heroesXmlLoader = HeroesXmlLoader.LoadWithCASC(cascConfig, _httpClientFactory.CreateClient(Constants.HttpClientBlizzard), progressReporter: new ProgressReporter(progress));
                });

                _stopwatch.Stop();
            });

        return heroesXmlLoader;
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

    private void DisplayFileFilters()
    {
        if (_options.IncludeFilters.Count == 1 && _options.IncludeFilters.First() == "*")
        {
            _logger.LogInformation("No filters applied, extracting all files");
            _console.MarkupLine("[aqua]Filters: * (extracting all files)[/]");
        }
        else
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Applying filters: {FileFilters}", string.Join(", ", _options.IncludeFilters));

            _console.MarkupLineInterpolated($"[aqua]Filters: {string.Join(", ", _options.IncludeFilters)}[/]");
        }
    }

    private void DisplayOutputDirectory()
    {
        string fullOutputDirectory = Path.GetFullPath(_options.OutputDirectory);

        _logger.LogInformation("Output directory: {OutputDirectory}", fullOutputDirectory);
        _console.MarkupLineInterpolated($"[aqua]Output Directory: {fullOutputDirectory}[/]");
    }

    private Dictionary<string, string>? GetFilePaths(HeroesXmlLoader heroesXmlLoader)
    {
        _console.WriteLine("Gettings files for extraction...");

        CASCFolder folder = null!;

        try
        {
            folder = heroesXmlLoader.GetCASCFolder(_rootDirectory);
        }
        catch (DirectoryNotFoundException)
        {
            _logger.LogError("Root directory not found in storage: {RootDirectory}", _rootDirectory);
            _console.MarkupLineInterpolated($"[red]Error: Root directory not found in storage: {_rootDirectory}[/]");
            return [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during extraction of root directory: {RootDirectory}", _rootDirectory);
            _console.MarkupLineInterpolated($"[red]An error occurred during extraction of root directory: {_rootDirectory}: {ex.Message}[/]");
            return [];
        }

        Matcher matcher = new(StringComparison.OrdinalIgnoreCase);
        matcher.AddIncludePatterns(_options.IncludeFilters);
        matcher.AddExcludePatterns(_options.ExcludeFilters);

        IEnumerable<string> enumeratedFiles = EnumerateDirectory(folder)
            .Select(x => x.FullName.Replace('\\', '/'))
            .Where(filePath => matcher.Match(filePath).HasMatches);

        Dictionary<string, string> outputPathByOriginalFilePath;

        if (_options.Flatten)
        {
            Dictionary<string, string>? outputPaths = GetFlattenedOutputPaths(enumeratedFiles);
            if (outputPaths is null)
                return null;

            outputPathByOriginalFilePath = outputPaths;
        }
        else
        {
            outputPathByOriginalFilePath = GetOutputPaths(enumeratedFiles);
        }

        _console.WriteLine($"Total files to extract: {outputPathByOriginalFilePath.Count}");

        return outputPathByOriginalFilePath;
    }

    private Dictionary<string, string>? GetFlattenedOutputPaths(IEnumerable<string> filteredFiles)
    {
        // for final output paths, key is original file path (casc), value is output path (to extract)
        Dictionary<string, string> outputPathByOriginalFilePath = new(StringComparer.OrdinalIgnoreCase);

        // for finding duplicates
        Dictionary<string, string> originalFilePathByFileName = new(StringComparer.OrdinalIgnoreCase);

        // for generating appended names to avoid duplicates
        Dictionary<string, int> suffixCounterByFileName = new(StringComparer.OrdinalIgnoreCase);

        IOrderedEnumerable<string> orderedFiles = filteredFiles.OrderBy(x => x, StringComparer.OrdinalIgnoreCase);

        foreach (string filePath in orderedFiles)
        {
            string fileName = Path.GetFileName(filePath).ToLowerInvariant();

            if (originalFilePathByFileName.TryGetValue(fileName, out string? existingFilePath))
            {
                // duplicates
                switch (_options.DuplicateHandling)
                {
                    case CascExtractDuplicateHandling.Error:
                        _logger.LogError("Duplicate file name {FileName}: {FilePath} conflicts with {ExistingFilePath}", fileName, filePath, existingFilePath);
                        _console.MarkupLineInterpolated($"[red]Error: Duplicate file name '{fileName}': {filePath} conflicts with {existingFilePath}[/]");
                        return null;
                    case CascExtractDuplicateHandling.Ignore:
                        _logger.LogTrace("Ignoring duplicate file name: {FileName} (original: {ExistingFilePath}, duplicate: {FilePath})", fileName, existingFilePath, filePath);
                        continue; // keeps the first occurrence and ignores the duplicate
                    case CascExtractDuplicateHandling.Overwrite:
                        _logger.LogTrace("Overwriting duplicate file name: {FileName} (original: {ExistingFilePath}, duplicate: {FilePath})", fileName, existingFilePath, filePath);
                        outputPathByOriginalFilePath.Remove(existingFilePath);
                        break; // keeps the last occurrence
                    case CascExtractDuplicateHandling.Append:
                        fileName = GetAppendedName(fileName, originalFilePathByFileName, suffixCounterByFileName);
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown duplicate handling: {_options.DuplicateHandling}");
                }
            }

            originalFilePathByFileName[fileName] = filePath;
            outputPathByOriginalFilePath[filePath] = fileName;
        }

        return outputPathByOriginalFilePath;
    }

    // originalFilePath: the path of the file in the CASC storage
    // extractFilePath: the path of the file to be created in the output directory
    private async Task<bool> CreateFile(HeroesXmlLoader heroesXmlLoader, string originalFilePath, string extractFilePath, CancellationToken cancellationToken = default)
    {
        string outputFilePath = Path.Combine(_options.OutputDirectory, extractFilePath);

        string fullOutputRoot = Path.GetFullPath(_options.OutputDirectory);
        string fullOutputFile = Path.GetFullPath(outputFilePath);

        // guard check
        if (!fullOutputFile.StartsWith(fullOutputRoot, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Skipping file with path outside output directory: {FilePath}", originalFilePath);
            return false;
        }

        string? directoryPath = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
            Directory.CreateDirectory(directoryPath);

        _logger.LogTrace("Extracting file: {OriginalFilePath} to {OutputFilePath}", originalFilePath, outputFilePath);

        using Stream heroesFile = heroesXmlLoader.GetFile(originalFilePath);
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
        await JsonSerializer.SerializeAsync(fileStream, modsInfoFile, _jsonSerializerOptions);
    }
}
