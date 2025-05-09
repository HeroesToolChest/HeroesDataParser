namespace HeroesDataParser.Infrastructure.Configurations;

public class CustomConfigurationService : ConfigurationServiceBase, ICustomConfigurationService
{
    private const string _customDataExtension = ".xml";

    private readonly ILogger<CustomConfigurationService> _logger;
    private readonly IFileProvider _fileProvider;

    private readonly string _customConfigurationDirectory = Path.Join("config-files", "custom");

    // key: extractor name
    // key: file prefix name
    // key: build number
    // value: relative file path
    private readonly Dictionary<string, Dictionary<string, SortedDictionary<int, string>>> _customElementByExtractorName = new(StringComparer.OrdinalIgnoreCase);

    public CustomConfigurationService(ILogger<CustomConfigurationService> logger, IOptions<RootOptions> options, IFileProvider fileProvider)
        : base(options.Value)
    {
        _logger = logger;
        _fileProvider = fileProvider;
    }

    public ISet<string> SelectedCustomDataFilePaths { get; private set; } = new HashSet<string>();

    public string CustomConfigurationDirectory => _customConfigurationDirectory;

    public override void Load()
    {
        LoadFiles();
        ProcessFiles();
    }

    protected override void LoadFiles()
    {
        _logger.LogInformation("Loading custom configuration files");

        // directory should contain directory names that are the name of the xml parsers (extractors), i.e hero, announcer, etc..
        IDirectoryContents directoryContents = _fileProvider.GetDirectoryContents(_customConfigurationDirectory);

        // load the extractor name directories
        LoadExtractorConfigurations(directoryContents);

        _logger.LogTrace("Custom configuration files loaded: {@Files}", _customElementByExtractorName);
    }

    protected override void ProcessFiles()
    {
        foreach (var fileNamePrefixCollection in _customElementByExtractorName.Values)
        {
            foreach (var filePathsByFilePrefixName in fileNamePrefixCollection)
            {
                string filePrefixName = filePathsByFilePrefixName.Key;

                string? selectedFilePath = GetSelectedFilePath(filePathsByFilePrefixName.Value);

                if (string.IsNullOrWhiteSpace(selectedFilePath))
                {
                    _logger.LogWarning("No parsing configuration file found for {FilePrefixName}", filePrefixName);
                    continue;
                }

                _logger.LogInformation("Selected parsing configuration file {SelectedFilePath} for {FilePrefixName}", selectedFilePath, filePrefixName);

                SelectedCustomDataFilePaths.Add(selectedFilePath);
            }
        }
    }

    private void LoadExtractorConfigurations(IDirectoryContents directoryContents)
    {
        foreach (IFileInfo fileInfo in directoryContents)
        {
            Dictionary<string, string> relativeFilePathByDefaultFileName = new(StringComparer.OrdinalIgnoreCase);

            // we are looking for a directory so anything else then no
            if (!fileInfo.Exists || fileInfo.PhysicalPath is null || !fileInfo.IsDirectory)
                continue;

            // check if the directory name is in the extractors list
            if (!Options.Extractors.ContainsKey(fileInfo.Name))
                continue;

            IDirectoryContents extractorDirectoryContents = _fileProvider.GetDirectoryContents(Path.Join(_customConfigurationDirectory, fileInfo.Name));

            // load the files in the extractor directories
            LoadCustomDataFiles(extractorDirectoryContents, fileInfo.Name, relativeFilePathByDefaultFileName);
        }
    }

    private void LoadCustomDataFiles(IDirectoryContents directoryContents, string extractorName, Dictionary<string, string> relativeFilePathByDefaultFileName)
    {
        foreach (IFileInfo fileInfo in directoryContents)
        {
            // we only want files...
            if (!fileInfo.Exists || fileInfo.PhysicalPath is null || fileInfo.IsDirectory)
                continue;

            // ...with the custom data extension
            if (!fileInfo.Name.EndsWith(_customDataExtension, StringComparison.OrdinalIgnoreCase))
                continue;

            ReadOnlySpan<char> filePathSpan = Path.GetFileNameWithoutExtension(fileInfo.Name.AsSpan());
            int index = filePathSpan.LastIndexOf('_');

            string relativeFilePath = Path.Join(_customConfigurationDirectory, extractorName, fileInfo.Name);

            // check for default file (no build number suffix)
            if (index < 0)
            {
                relativeFilePathByDefaultFileName[filePathSpan.ToString()] = relativeFilePath;
                continue;
            }

            if (index < 1 || !int.TryParse(filePathSpan[(index + 1)..], out int build))
                continue;

            // for files with a build number suffix
            string fileNamePrefix = filePathSpan[..index].ToString();

            if (_customElementByExtractorName.TryGetValue(extractorName, out var customElementsByFileNamePrefix))
            {
                if (customElementsByFileNamePrefix.TryGetValue(fileNamePrefix, out var relativePathsByBuild))
                {
                    relativePathsByBuild[build] = relativeFilePath;
                }
                else
                {
                    customElementsByFileNamePrefix[fileNamePrefix] = new SortedDictionary<int, string>
                    {
                        [build] = relativeFilePath,
                    };
                }
            }
            else
            {
                _customElementByExtractorName[extractorName] = new Dictionary<string, SortedDictionary<int, string>>(StringComparer.OrdinalIgnoreCase)
                {
                    [fileNamePrefix] = new SortedDictionary<int, string>
                    {
                        [build] = relativeFilePath,
                    },
                };
            }
        }

        // add in the default file (no build number suffix)
        foreach (var defaultFile in relativeFilePathByDefaultFileName)
        {
            if (_customElementByExtractorName.TryGetValue(extractorName, out var customElementsByFileNamePrefix))
            {
                if (customElementsByFileNamePrefix.TryGetValue(defaultFile.Key, out var relativePathsByBuild))
                {
                    if (relativePathsByBuild.Keys.Count > 0)
                        relativePathsByBuild[relativePathsByBuild.Keys.Max() + 1] = defaultFile.Value;
                    else
                        relativePathsByBuild[0] = defaultFile.Value;
                }
                else
                {
                    customElementsByFileNamePrefix[defaultFile.Key] = new SortedDictionary<int, string>
                    {
                        [0] = defaultFile.Value,
                    };
                }
            }
            else
            {
                _customElementByExtractorName[extractorName] = new Dictionary<string, SortedDictionary<int, string>>(StringComparer.OrdinalIgnoreCase)
                {
                    [defaultFile.Key] = new SortedDictionary<int, string>
                    {
                        [0] = defaultFile.Value,
                    },
                };
            }
        }
    }
}
