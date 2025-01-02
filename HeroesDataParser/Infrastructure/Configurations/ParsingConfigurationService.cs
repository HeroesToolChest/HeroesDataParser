using HeroesDataParser.Core.Models.ConfigParsing;
using System.Text.RegularExpressions;

namespace HeroesDataParser.Infrastructure.Configurations;

public class ParsingConfigurationService : IParsingConfigurationService
{
    private const string _parsingConfigFileNameNoExt = "parsing";
    private const string _parsingConfigExtension = ".json";
    private const string _parsingConfigFileName = $"{_parsingConfigFileNameNoExt}{_parsingConfigExtension}";

    private readonly string _parsingConfigurationDirectory = Path.Join("config-files", "parsing");

    private readonly ILogger<ParsingConfigurationService> _logger;
    private readonly RootOptions _options;
    private readonly IFileProvider _fileProvider;

    private readonly SortedDictionary<int, string> _relativeFilePathsByBuild = [];
    private readonly Dictionary<string, ParsingDataObjectType> _parsingDataObjectTypeByDOT = [];

    private readonly JsonDocumentOptions _jsonDocumentOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
    };

    public ParsingConfigurationService(ILogger<ParsingConfigurationService> logger, IOptions<RootOptions> options, IFileProvider fileProvider)
    {
        _logger = logger;
        _options = options.Value;
        _fileProvider = fileProvider;

        LoadFiles();
        ProcessFile();
    }

    // the relative file path
    public string? SelectedFilePath { get; private set; }

    public string ParsingConfigurationDirectory => _parsingConfigurationDirectory;

    public IEnumerable<string> FilterAllowedItems(string dataObjectType, IEnumerable<string> items)
    {
        if (_parsingDataObjectTypeByDOT.TryGetValue(dataObjectType, out ParsingDataObjectType? type))
        {
            ParsingDisallow parsingDisallow = type.ParsingDisallow;

            foreach (string item in items)
            {
                if (!parsingDisallow.Exact.Contains(item) && !parsingDisallow.Regex.Any(x => Regex.IsMatch(item, x)))
                    yield return item;
                else
                    _logger.LogTrace("Item {Item} is disallowed for {DataObjectType}", item, dataObjectType);
            }
        }
        else
        {
            _logger.LogTrace("No parsing configuration found for {DataObjectType}", dataObjectType);

            foreach (string item in items)
            {
                yield return item;
            }
        }
    }

    private void LoadFiles()
    {
        _logger.LogInformation("Loading parsing configuration files");

        // get all<fileName>_<build>.json files
        IDirectoryContents directoryContents = _fileProvider.GetDirectoryContents(_parsingConfigurationDirectory);

        foreach (IFileInfo fileInfo in directoryContents)
        {
            if (fileInfo.IsDirectory || !fileInfo.Exists || fileInfo.PhysicalPath is null)
                continue;

            if (!fileInfo.Name.StartsWith(_parsingConfigFileNameNoExt, StringComparison.OrdinalIgnoreCase) || !fileInfo.Name.EndsWith(_parsingConfigExtension, StringComparison.OrdinalIgnoreCase))
                continue;

            ReadOnlySpan<char> filePathSpan = Path.GetFileNameWithoutExtension(fileInfo.Name.AsSpan());

            int buildIndex = filePathSpan.IndexOf('_') + 1;
            if (buildIndex < _parsingConfigFileNameNoExt.Length)
                continue;

            if (int.TryParse(filePathSpan[buildIndex..], out int buildNumber))
            {
                _relativeFilePathsByBuild.Add(buildNumber, Path.Join(_parsingConfigurationDirectory, fileInfo.Name));
            }
        }

        // add in the default file
        IFileInfo defaultFile = _fileProvider.GetFileInfo(Path.Join(_parsingConfigurationDirectory, _parsingConfigFileName));
        if (defaultFile.Exists)
        {
            if (_relativeFilePathsByBuild.Keys.Count > 0)
                _relativeFilePathsByBuild.Add(_relativeFilePathsByBuild.Keys.Max() + 1, Path.Join(_parsingConfigurationDirectory, _parsingConfigFileName));
            else
                _relativeFilePathsByBuild.Add(0, Path.Join(_parsingConfigurationDirectory, _parsingConfigFileName));
        }

        _logger.LogTrace("Parsing configuration files loaded: {@Files}", _relativeFilePathsByBuild);
    }

    private void ProcessFile()
    {
        // check if a build number was set
        if (_options.BuildNumber.HasValue)
        {
            // are there any files loaded
            if (_relativeFilePathsByBuild.Count > 0)
            {
                // exact match
                if (_relativeFilePathsByBuild.TryGetValue(_options.BuildNumber.Value, out string? filePath))
                {
                    SelectedFilePath = filePath;
                }
                else if (_options.BuildNumber.Value <= _relativeFilePathsByBuild.Keys.Min())
                {
                    // lowest build number
                    SelectedFilePath = _relativeFilePathsByBuild.First().Value;
                }
                else
                {
                    // load next lowest
                    int index = _relativeFilePathsByBuild.Keys.ToList().BinarySearch(_options.BuildNumber.Value);

                    int closestLowerIndex = ~index - 1;
                    if (closestLowerIndex >= 0)
                    {
                        SelectedFilePath = _relativeFilePathsByBuild.ElementAt(closestLowerIndex).Value;
                    }
                }
            }
        }
        else if (_relativeFilePathsByBuild.Count > 0)
        {
            // default
            SelectedFilePath = _relativeFilePathsByBuild.Last().Value;
        }

        if (string.IsNullOrWhiteSpace(SelectedFilePath))
        {
            _logger.LogWarning("No parsing configuration file found");
            return;
        }

        _logger.LogInformation("Selected parsing configuration file: {SelectedFilePath}", SelectedFilePath);

        LoadFile(SelectedFilePath);
    }

    private void LoadFile(string selectedFilePath)
    {
        using Stream fileStream = _fileProvider.GetFileInfo(selectedFilePath).CreateReadStream();
        using JsonDocument jsonDocument = JsonDocument.Parse(fileStream, _jsonDocumentOptions);

        JsonElement root = jsonDocument.RootElement;

        if (root.TryGetProperty("XmlDataParsers", out JsonElement xmlDataParserElement))
        {
            foreach (JsonProperty dataObjectTypeProperty in xmlDataParserElement.EnumerateObject())
            {
                ParsingDataObjectType parsingDataObjectType = new();

                string dataObjectType = dataObjectTypeProperty.Name;

                if (dataObjectTypeProperty.Value.TryGetProperty("Disallow", out JsonElement disallowElement))
                {
                    if (disallowElement.TryGetProperty("Exact", out JsonElement exactElement))
                    {
                        foreach (JsonElement item in exactElement.EnumerateArray())
                        {
                            string? value = item.GetString();
                            if (value is not null)
                                parsingDataObjectType.ParsingDisallow.Exact.Add(value);
                        }
                    }

                    if (disallowElement.TryGetProperty("Regex", out JsonElement regexElement))
                    {
                        foreach (JsonElement item in regexElement.EnumerateArray())
                        {
                            string? value = item.GetString();
                            if (value is not null)
                                parsingDataObjectType.ParsingDisallow.Regex.Add(value);
                        }
                    }
                }

                _parsingDataObjectTypeByDOT[dataObjectType] = parsingDataObjectType;
            }
        }

        _logger.LogInformation("Number of parsing configuration loaded: {Count}", _parsingDataObjectTypeByDOT.Count);
        _logger.LogTrace("Parsing configuration loaded: {@ParsingDataObjectTypeByDOT}", _parsingDataObjectTypeByDOT);
    }
}
