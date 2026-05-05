using Heroes.Element;

namespace HeroesDataParser.Infrastructure.Commands.LocalizedTextCommands;

public class LocalizedTextExportService : ILocalizedTextExportService
{
    private readonly ILogger<LocalizedTextExportService> _logger;
    private readonly LocalizedTextExportOptions _options;
    private readonly IAnsiConsole _console;
    private readonly IJsonSerializerOptionService _jsonSerializerOptionService;
    private readonly IGameStringSerializerService _gameStringSerializerService;

    public LocalizedTextExportService(
        ILogger<LocalizedTextExportService> logger,
        IOptions<LocalizedTextExportOptions> options,
        IAnsiConsole console,
        IJsonSerializerOptionService jsonSerializerOptionService,
        IGameStringSerializerService gameStringSerializerService)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;
        _jsonSerializerOptionService = jsonSerializerOptionService;
        _gameStringSerializerService = gameStringSerializerService;
    }

    public async Task ExportGameStrings()
    {
        Func<Task>? dataOutputFileTask = await CreateDataOutputFile();

        if (dataOutputFileTask is null)
            return;

        if (_options.ExtractType == ExtractType.Remove)
        {
            await dataOutputFileTask.Invoke();
            return;
        }

        // if no output gamestring file path provided, create a new one, otherwise update the existing one
        if (string.IsNullOrWhiteSpace(_options.GameStringFilePath))
            await CreateGameStringOutputFile(dataOutputFileTask);
        else
            await UpdateGameStringFile(dataOutputFileTask);
    }

    private static RootJsonDataElement GetRootJsonDataElement(IElementDocument elementDocument)
    {
        SortedDictionary<string, object> itemObjects = new(StringComparer.OrdinalIgnoreCase);

        IEnumerable<IElementObject> elementObjects = elementDocument.GetElementObjects();

        foreach (IElementObject elementObject in elementObjects)
        {
            itemObjects.Add(elementObject.Id, elementObject);
        }

        return new RootJsonDataElement()
        {
            MetaDataProperties = elementDocument.Meta,
            Items = itemObjects,
        };
    }

    // for new gamestring file
    private static MetaGameStringProperties GetMetaGameStringProperties(MetaDataProperties meta)
    {
        return new()
        {
            ItemsType = ItemsType.GameStrings,
            DataTypes = [meta.DataType],
            MapName = meta.MapName,
            HeroesVersion = meta.HeroesVersion,
            HdpVersion = meta.HdpVersion,
            GameStringTextProperties = new()
            {
                Locale = meta.GameStringTextProperties!.Locale,
                GameStringTextType = meta.GameStringTextProperties.GameStringTextType,
                ConstantVars =
                {
                    Replaced = meta.GameStringTextProperties.ConstantVars.Replaced,
                    Preserved = meta.GameStringTextProperties.ConstantVars.Preserved,
                },
                StyleVars =
                {
                    Replaced = meta.GameStringTextProperties.StyleVars.Replaced,
                    Preserved = meta.GameStringTextProperties.StyleVars.Preserved,
                },
            },
        };
    }

    // defer the file writing until we know whether we can successfully create/update the gamestring file
    private async Task<Func<Task>?> CreateDataOutputFile()
    {
        if (ShouldSkipFileWrite(_options.OutputDataFilePath, true))
            return null;

        RootJsonDataElement? rootJsonDataElement = await GetRootJsonDataElement();
        if (rootJsonDataElement is null)
            return null;

        return async () => await WriteDataOutputFile(rootJsonDataElement);
    }

    private async Task WriteDataOutputFile(RootJsonDataElement rootJsonDataElement)
    {
        if (_options.ExtractType == ExtractType.Remove || _options.ExtractType == ExtractType.Extract)
        {
            rootJsonDataElement.MetaDataProperties.LocalizedText = LocalizedText.Extracted;
            rootJsonDataElement.MetaDataProperties.GameStringTextProperties = null;
        }

        Directory.CreateDirectory(_options.OutputDirectory);
        using FileStream stream = File.Create(_options.OutputDataFilePath);

        await JsonSerializer.SerializeAsync(stream, rootJsonDataElement, GetDataJsonSerializerOptions());

        if (_options.IsNewDataFile)
        {
            _logger.LogInformation("New data file created at {OutputDataFilePath}", _options.OutputDataFilePath);
            _console.MarkupLineInterpolated($"New data file created at {_options.OutputDataFilePath}");
        }
        else
        {
            _logger.LogInformation("Updated data file at {OutputDataFilePath}", _options.OutputDataFilePath);
            _console.MarkupLineInterpolated($"Updated data file at {_options.OutputDataFilePath}");
        }
    }

    private async Task CreateGameStringOutputFile(Func<Task> dataOutputFileTask)
    {
        RootJsonDataElement rootJsonDataElement = (await GetRootJsonDataElement())!; // not null here since we already dealt with the data file
        MetaDataProperties meta = rootJsonDataElement.MetaDataProperties;

        string fileName = $"{DataType.GameStrings}_{meta.HeroesVersion.Build}_{meta.GameStringTextProperties!.Locale}.json".ToLowerInvariant();

        string outputFilePath = Path.Combine(_options.OutputDirectory, fileName);
        if (ShouldSkipFileWrite(outputFilePath, false))
            return;

        await dataOutputFileTask.Invoke();

        Directory.CreateDirectory(_options.OutputDirectory);

        byte[] bytes = _gameStringSerializerService.SerializeGameStrings(GetMetaGameStringProperties(meta), GetGameStringJsonSerializerOptions());

        using FileStream stream = File.Create(outputFilePath);
        await stream.WriteAsync(bytes);

        DisplayGameStringSuccess(outputFilePath);
    }

    private async Task UpdateGameStringFile(Func<Task> dataOutputFileTask)
    {
        RootJsonDataElement rootJsonDataElement = (await GetRootJsonDataElement())!; // not null here since we already dealt with the data file
        MetaDataProperties meta = rootJsonDataElement.MetaDataProperties;

        string outputFilePath = Path.Combine(_options.OutputDirectory, Path.GetFileName(_options.GameStringFilePath!));

        MetaGameStringProperties? metaGameStringProperties = await GetUpdatedMetaGameStringProperties(outputFilePath, meta, dataOutputFileTask);
        if (metaGameStringProperties is null)
            return;

        byte[] bytes = _gameStringSerializerService.SerializeGameStrings(metaGameStringProperties, GetGameStringJsonSerializerOptions());

        using FileStream stream = File.Create(outputFilePath);
        await stream.WriteAsync(bytes);

        DisplayGameStringSuccess(outputFilePath);
    }

    private async Task<MetaGameStringProperties?> GetUpdatedMetaGameStringProperties(string outputFilePath, MetaDataProperties meta, Func<Task> dataOutputFileTask)
    {
        using FileStream gameStringFileStream = File.OpenRead(_options.GameStringFilePath!);
        using JsonDocument gameStringJsonDocument = await JsonDocument.ParseAsync(gameStringFileStream);

        using GameStringDocument gameStringDocument = GameStringDocument.Load(gameStringJsonDocument);

        if (ShouldSkipFileWrite(outputFilePath, false))
            return null;

        if (!IsValidateGameStringFile(meta, gameStringDocument.Meta))
            return null;

        await dataOutputFileTask.Invoke();

        AddToSerializer(MergeItems(gameStringDocument));

        // add the "new" data type to the gamestrings meta
        MetaGameStringProperties metaGameStringProperties = gameStringDocument.Meta;
        metaGameStringProperties.DataTypes.Add(meta.DataType);

        return metaGameStringProperties;
    }

    private async Task<RootJsonDataElement?> GetRootJsonDataElement()
    {
        using FileStream dataFileStream = File.OpenRead(_options.DataFilePath);
        using JsonDocument dataJsonDocument = await JsonDocument.ParseAsync(dataFileStream);

        using IElementDocument elementDocument = DataDocument.Load(dataJsonDocument);

        if (elementDocument.Meta.LocalizedText == LocalizedText.Extracted)
        {
            _logger.LogInformation("The provided data file already has gamestrings extracted");
            _console.MarkupLine("The provided data file already has gamestrings extracted");
            return null;
        }

        return GetRootJsonDataElement(elementDocument);
    }

    private LocalizedTextOption GetLocalizedTextOption()
    {
        return _options.ExtractType switch
        {
            ExtractType.Extract => LocalizedTextOption.Extract,
            ExtractType.Copy => LocalizedTextOption.Copy,
            ExtractType.Remove => LocalizedTextOption.Extract,
            _ => throw new InvalidOperationException("Invalid extract type"),
        };
    }

    private JsonSerializerOptions GetDataJsonSerializerOptions()
    {
        JsonSerializerOptions jsonSerializerOptions = new(_jsonSerializerOptionService.GeneralJsonSerializerOptions)
        {
            WriteIndented = _options.JsonIndent,
        };

        jsonSerializerOptions.Converters.Add(new GameStringTextConverter(new GameStringTextConverterOptions()
        {
            GameStringTextType = GameStringTextType.RawText,
            RemoveHltForConstantTags = GameStringTextHltRemoveMode.None,
            RemoveHltForStyleTags = GameStringTextHltRemoveMode.None,
        }));

        jsonSerializerOptions.Converters.Add(new GameStringItemDictionaryConverter());
        jsonSerializerOptions.TypeInfoResolver = new HeroesElementResolver()
        {
            Modifiers =
            {
                typeInfo => JsonTypeInfoModifiers.SerializationModifiers(typeInfo, GetLocalizedTextOption(), _gameStringSerializerService.DataGameStringItemDictionary),
            },
        };

        return jsonSerializerOptions;
    }

    private JsonSerializerOptions GetGameStringJsonSerializerOptions()
    {
        JsonSerializerOptions jsonSerializerOptions = new(_jsonSerializerOptionService.GeneralJsonSerializerOptions)
        {
            WriteIndented = _options.JsonIndent,
        };

        jsonSerializerOptions.Converters.Add(new GameStringTextConverter(new GameStringTextConverterOptions()
        {
            GameStringTextType = GameStringTextType.RawText,
            RemoveHltForConstantTags = GameStringTextHltRemoveMode.None,
            RemoveHltForStyleTags = GameStringTextHltRemoveMode.None,
        }));
        jsonSerializerOptions.Converters.Add(new GameStringItemDictionaryConverter());

        jsonSerializerOptions.TypeInfoResolver = new HeroesElementResolver()
        {
            Modifiers =
            {
                typeInfo => JsonTypeInfoModifiers.SerializationModifiers(typeInfo, LocalizedTextOption.None, []),
            },
        };

        return jsonSerializerOptions;
    }

    private bool ShouldSkipFileWrite(string filePath, bool isDataFile)
    {
        if (File.Exists(filePath))
        {
            if (!_options.AllowOverwrite)
            {
                _logger.LogError("Output file already exists and overwrite is not allowed: {OutputFilePath}", filePath);
                _console.MarkupLine($"[red]Output file already exists: {filePath}[/]");

                return true;
            }
        }
        else
        {
            if (isDataFile)
                _options.IsNewDataFile = true;
            else
                _options.IsNewGameStringFile = true;
        }

        return false;
    }

    private GameStringItemDictionary MergeItems(GameStringDocument gameStringDocument)
    {
        GameStringItemDictionary currentItems = gameStringDocument.GetItems();
        GameStringItemDictionary newItems = _gameStringSerializerService.DataGameStringItemDictionary;

        // combine both dictionaries, for duplicate keys, newItems will overwrite currentItems
        foreach ((string itemKey, GameStringFilePropertyName newPropertyNames) in newItems)
        {
            if (!currentItems.TryGetValue(itemKey, out GameStringFilePropertyName? currentPropertyNames))
            {
                currentItems[itemKey] = newPropertyNames;
                continue;
            }

            foreach ((string propertyKey, GameStringFilePropertyId newPropertyId) in newPropertyNames)
            {
                if (!currentPropertyNames.TryGetValue(propertyKey, out GameStringFilePropertyId? currentPropertyId))
                {
                    currentPropertyNames[propertyKey] = newPropertyId;
                    continue;
                }

                foreach ((string key, GameStringText value) in newPropertyId.KeyValuePairs)
                    currentPropertyId.KeyValuePairs[key] = value;

                foreach ((string key, List<GameStringText> value) in newPropertyId.KeyArrayPairs)
                    currentPropertyId.KeyArrayPairs[key] = value;
            }
        }

        return currentItems;
    }

    private void AddToSerializer(GameStringItemDictionary items)
    {
        _gameStringSerializerService.DataGameStringItemDictionary.Clear();

        foreach (var item in items)
        {
            _gameStringSerializerService.DataGameStringItemDictionary.Add(item.Key, item.Value);
        }
    }

    private void DisplayGameStringSuccess(string outputFilePath)
    {
        if (_options.IsNewGameStringFile)
        {
            _logger.LogInformation("New gamestring file created at {OutputGameStringFilePath}", outputFilePath);
            _console.MarkupLineInterpolated($"New gamestring file created at {outputFilePath}");
        }
        else
        {
            _logger.LogInformation("Updated gamestring file at {OutputGameStringFilePath}", outputFilePath);
            _console.MarkupLineInterpolated($"Updated gamestring file at {outputFilePath}");
        }
    }

    private bool IsValidateGameStringFile(MetaDataProperties metaDataProperties, MetaGameStringProperties metaGameStringProperties)
    {
        if (!metaDataProperties.HdpVersion.Equals(metaGameStringProperties.HdpVersion, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogError("The HDP version of the data file does not match the version in the gamestrings file. Data file HDP version: {DataFileHdpVersion}, Gamestrings file HDP version: {GameStringsFileHdpVersion}", metaDataProperties.HdpVersion, metaGameStringProperties.HdpVersion);
            _console.MarkupLineInterpolated($"[red]The HDP version of the data file does not match the version in the gamestrings file. Data file HDP version: {metaDataProperties.HdpVersion}, Gamestrings file HDP version: {metaGameStringProperties.HdpVersion}[/]");

            return false;
        }

        if (metaDataProperties.HeroesVersion != metaGameStringProperties.HeroesVersion)
        {
            _logger.LogError("The Heroes of the Storm version of the data file does not match the version in the gamestrings file. Data file Heroes version: {DataFileHeroesVersion}, Gamestrings file Heroes version: {GameStringsFileHeroesVersion}", metaDataProperties.HeroesVersion, metaGameStringProperties.HeroesVersion);
            _console.MarkupLineInterpolated($"[red]The Heroes of the Storm version of the data file does not match the version in the gamestrings file. Data file Heroes version: {metaDataProperties.HeroesVersion}, Gamestrings file Heroes version: {metaGameStringProperties.HeroesVersion}[/]");

            return false;
        }

        if (metaGameStringProperties.DataTypes.Contains(metaDataProperties.DataType))
        {
            _logger.LogError("The gamestrings file already contains the data type of {DataType}", metaDataProperties.DataType);
            _console.MarkupLineInterpolated($"[red]The gamestrings file already contains the data type of {metaDataProperties.DataType}[/]");

            return false;
        }

        if ((metaDataProperties.MapName is null && metaGameStringProperties.MapName is not null) ||
            (metaDataProperties.MapName is not null && metaGameStringProperties.MapName is null) ||
            (metaDataProperties.MapName is not null && !metaDataProperties.MapName.Equals(metaGameStringProperties.MapName, StringComparison.Ordinal)))
        {
            _logger.LogError("The map name of the data file does not match the map name in the gamestrings file. Data file map name: {DataFileMapName}, Gamestrings file map name: {GameStringsFileMapName}", metaDataProperties.MapName, metaGameStringProperties.MapName);
            _console.MarkupLineInterpolated($"[red]The map name of the data file does not match the map name in the gamestrings file. Data file map name: {metaDataProperties.MapName ?? "null"}, Gamestrings file map name: {metaGameStringProperties.MapName ?? "null"}[/]");

            return false;
        }

        if (metaDataProperties.GameStringTextProperties is null)
        {
            _logger.LogError("gameStringText properties is missing in the data file.");
            _console.MarkupLine("[red]'gameStringText' properties is missing in the data file.[/]");
            return false;
        }

        if (!metaDataProperties.GameStringTextProperties.Equals(metaGameStringProperties.GameStringTextProperties))
        {
            _logger.LogError("The gameStringText properties of the data file does not match the gameStringText properties in the gamestrings file.");
            _console.MarkupLine("[red]The 'gameStringText' properties of the data file does not match the 'gameStringText' properties in the gamestrings file.[/]");
            return false;
        }

        return true;
    }
}
