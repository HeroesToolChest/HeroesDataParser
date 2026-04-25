using NSubstitute.ReceivedExtensions;
using Spectre.Console;

namespace HeroesDataParser.Infrastructure.JsonFileWriters.Tests;

[TestClass]
public class JsonDataFileWriterServiceTests
{
    private readonly ILogger<JsonDataFileWriterService> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IAnsiConsole _console;
    private readonly ISerializedDataStoreService _serializedDataStoreService;
    private readonly IResultSummaryService _resultSummaryService;
    private readonly IGameStringSerializerService _extractedGameStringsService;

    public JsonDataFileWriterServiceTests()
    {
        _logger = Substitute.For<ILogger<JsonDataFileWriterService>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _console = Substitute.For<IAnsiConsole>();
        _serializedDataStoreService = Substitute.For<ISerializedDataStoreService>();
        _resultSummaryService = Substitute.For<IResultSummaryService>();
        _extractedGameStringsService = Substitute.For<IGameStringSerializerService>();
    }

    [TestMethod]
    public async Task Write_HasItems_CreatesJsonFile()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(Write_HasItems_CreatesJsonFile)),
            LocalizedText = LocalizedTextOption.None,
            CurrentLocale = StormLocale.ENUS,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStylesVars = false,
                ReplaceFontConstantVars = false,
                PreserveFontConstantVars = false,
                PreserveFontStyleVars = false,
            },
            HeroesVersion = new HeroesVersionOptions
            {
                Major = 2,
                Minor = 23,
                Revision = 2345,
                Build = 34566,
                IsPtr = false,
            },
            AppVersion = "5.0.0",
            JsonIndent = true,
        };

        _options.Value.Returns(rootOptions);

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _extractedGameStringsService); // create real instance
        JsonDataFileWriterService service = new(_logger, _options, _console, _serializedDataStoreService, jsonSerializerOptionService, _resultSummaryService);

        SortedDictionary<string, Hero> heroesByElementId = [];
        heroesByElementId.Add("hero1", new Hero("hero1") { UnitId = "hero1" });
        heroesByElementId.Add("hero2", new Hero("hero2")
        {
            UnitId = "hero2",
            Armor = new Dictionary<ArmorSet, UnitArmor>()
            {
                { ArmorSet.Hero, new UnitArmor { BasicArmor = 10, AbilityArmor = 5, SplashArmor = 2 } },
            },
            SummonedUnitIds = new HashSet<string> { "summonedUnit1", "summonedUnit2" },
        });

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "data", $"herodata_{rootOptions.BuildNumber}_enus.json");

        // act
        await service.Write(heroesByElementId);

        // assert
        _serializedDataStoreService.ReceivedWithAnyArgs(1).AddSerializedData(default!, default!);
        _ = _resultSummaryService.Received(1).JsonDataFilesWritten;
        _ = _resultSummaryService.Received(1).JsonDataFilesTotal;
        File.Exists(expectedFilePath).Should().BeTrue();
        File.ReadAllText(expectedFilePath).Should().Be(
        """
        {
          "meta": {
            "heroesVersion": "2.23.2345.34566",
            "hdpVersion": "5.0.0",
            "itemsType": "Data",
            "dataType": "HeroData",
            "localizedText": "None",
            "gameStringText": {
              "locale": "ENUS",
              "textType": "RawText",
              "replaceFontConstantVars": false,
              "replaceFontStylesVars": false,
              "preserveFontConstantVars": false,
              "preserveFontStyleVars": false
            },
            "totalItems": 2
          },
          "items": {
            "hero1": {
              "unitId": "hero1",
              "franchise": "Starcraft",
              "isMelee": false,
              "gender": "Male",
              "radius": 0,
              "innerRadius": 0,
              "sight": 0,
              "speed": 0,
              "roles": [],
              "ratings": {
                "complexity": 1,
                "damage": 1,
                "survivability": 1,
                "utility": 1
              },
              "portraits": {
                "heroSelect": "",
                "leaderboard": "",
                "loading": "",
                "partyPanel": "",
                "target": "",
                "draftScreen": "",
                "partyFrames": []
              }
            },
            "hero2": {
              "unitId": "hero2",
              "franchise": "Starcraft",
              "isMelee": false,
              "gender": "Male",
              "radius": 0,
              "innerRadius": 0,
              "sight": 0,
              "speed": 0,
              "roles": [],
              "ratings": {
                "complexity": 1,
                "damage": 1,
                "survivability": 1,
                "utility": 1
              },
              "portraits": {
                "heroSelect": "",
                "leaderboard": "",
                "loading": "",
                "partyPanel": "",
                "target": "",
                "draftScreen": "",
                "partyFrames": []
              },
              "armor": {
                "Hero": {
                  "basic": 10,
                  "ability": 5,
                  "splash": 2
                }
              },
              "summonedUnitIds": [
                "summonedUnit1",
                "summonedUnit2"
              ]
            }
          }
        }
        """.ReplaceLineEndings("\n"));
    }

    [TestMethod]
    public async Task Write_HasItemsNoIndent_CreatesJsonFileWithNoIndent()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(Write_HasItemsNoIndent_CreatesJsonFileWithNoIndent)),
            LocalizedText = LocalizedTextOption.None,
            CurrentLocale = StormLocale.ENUS,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStylesVars = false,
                ReplaceFontConstantVars = false,
                PreserveFontConstantVars = false,
                PreserveFontStyleVars = false,
            },
            HeroesVersion = new HeroesVersionOptions
            {
                Major = 2,
                Minor = 23,
                Revision = 2345,
                Build = 34566,
                IsPtr = false,
            },
            AppVersion = "5.0.0",
            JsonIndent = false,
        };

        _options.Value.Returns(rootOptions);

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _extractedGameStringsService); // create real instance
        JsonDataFileWriterService service = new(_logger, _options, _console, _serializedDataStoreService, jsonSerializerOptionService, _resultSummaryService);

        SortedDictionary<string, Hero> heroesByElementId = [];
        heroesByElementId.Add("hero1", new Hero("hero1") { UnitId = "hero1" });
        heroesByElementId.Add("hero2", new Hero("hero2")
        {
            UnitId = "hero2",
            Armor = new Dictionary<ArmorSet, UnitArmor>()
            {
                { ArmorSet.Hero, new UnitArmor { BasicArmor = 10, AbilityArmor = 5, SplashArmor = 2 } },
            },
            SummonedUnitIds = new HashSet<string> { "summonedUnit1", "summonedUnit2" },
        });

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "data", $"herodata_{rootOptions.BuildNumber}_enus.json");

        // act
        await service.Write(heroesByElementId);

        // assert
        _serializedDataStoreService.ReceivedWithAnyArgs(1).AddSerializedData(default!, default!);
        _ = _resultSummaryService.Received(1).JsonDataFilesWritten;
        _ = _resultSummaryService.Received(1).JsonDataFilesTotal;
        File.Exists(expectedFilePath).Should().BeTrue();
        File.ReadAllText(expectedFilePath).Should().Be(
        """
        {"meta":{"heroesVersion":"2.23.2345.34566","hdpVersion":"5.0.0","itemsType":"Data","dataType":"HeroData","localizedText":"None","gameStringText":{"locale":"ENUS","textType":"RawText","replaceFontConstantVars":false,"replaceFontStylesVars":false,"preserveFontConstantVars":false,"preserveFontStyleVars":false},"totalItems":2},"items":{"hero1":{"unitId":"hero1","franchise":"Starcraft","isMelee":false,"gender":"Male","radius":0,"innerRadius":0,"sight":0,"speed":0,"roles":[],"ratings":{"complexity":1,"damage":1,"survivability":1,"utility":1},"portraits":{"heroSelect":"","leaderboard":"","loading":"","partyPanel":"","target":"","draftScreen":"","partyFrames":[]}},"hero2":{"unitId":"hero2","franchise":"Starcraft","isMelee":false,"gender":"Male","radius":0,"innerRadius":0,"sight":0,"speed":0,"roles":[],"ratings":{"complexity":1,"damage":1,"survivability":1,"utility":1},"portraits":{"heroSelect":"","leaderboard":"","loading":"","partyPanel":"","target":"","draftScreen":"","partyFrames":[]},"armor":{"Hero":{"basic":10,"ability":5,"splash":2}},"summonedUnitIds":["summonedUnit1","summonedUnit2"]}}}
        """);
    }

    [TestMethod]
    public async Task Write_NoItems_NoFilesCreated()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(Write_NoItems_NoFilesCreated)),
        };

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _extractedGameStringsService); // create real instance
        JsonDataFileWriterService service = new(_logger, _options, _console, _serializedDataStoreService, jsonSerializerOptionService, _resultSummaryService);

        _options.Value.Returns(rootOptions);

        SortedDictionary<string, Hero> heroesByElementId = [];

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "data", $"herodata_{rootOptions.BuildNumber}_enus.json");

        // act
        await service.Write(heroesByElementId);

        // assert
        _serializedDataStoreService.DidNotReceiveWithAnyArgs().AddSerializedData(default!, default!);
        _ = _resultSummaryService.DidNotReceive().JsonDataFilesWritten;
        _ = _resultSummaryService.DidNotReceive().JsonDataFilesTotal;
        File.Exists(expectedFilePath).Should().BeFalse();
    }

    [TestMethod]
    public async Task WriteToMapSpecific_HasItemsWithNoDiffFound_CreatesJsonFile()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(WriteToMapSpecific_HasItemsWithNoDiffFound_CreatesJsonFile)),
            CurrentLocale = StormLocale.ENUS,
            MapSpecificWriterJsonOutputType = MapSpecificWriterJsonOutputType.Patch,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStylesVars = false,
                ReplaceFontConstantVars = false,
                PreserveFontConstantVars = false,
                PreserveFontStyleVars = false,
            },
            HeroesVersion = new HeroesVersionOptions
            {
                Major = 2,
                Minor = 23,
                Revision = 2345,
                Build = 34566,
                IsPtr = false,
            },
            AppVersion = "5.0.0",
            AllowEmptyMapSpecificPatchFiles = true,
            JsonIndent = true,
        };

        _options.Value.Returns(rootOptions);

        JsonPatch jsonPatch = JsonSerializer.Deserialize<JsonPatch>(
        """
        [
          {
            "op": "add",
            "path": "/meta/mapName",
            "value": "Blackheart's Bay"
          }
        ]
        """)!;
        _serializedDataStoreService.GetJsonDataPatch(default!, default!).ReturnsForAnyArgs(jsonPatch);

        SortedDictionary<string, Hero> heroesByElementId = [];
        heroesByElementId.Add("hero1", new Hero("hero1") { UnitId = "hero1" });

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "data", "maps", "map_nameyes", $"herodata_{rootOptions.BuildNumber}_enus.patch.json");

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _extractedGameStringsService);
        JsonDataFileWriterService service = new(_logger, _options, _console, _serializedDataStoreService, jsonSerializerOptionService, _resultSummaryService);

        // act
        await service.WriteToMapSpecific("map name!_yes?", heroesByElementId);

        // assert
        _serializedDataStoreService.DidNotReceiveWithAnyArgs().AddSerializedData(default!, default!);
        _ = _resultSummaryService.Received(1).JsonDataFilesWritten;
        _ = _resultSummaryService.Received(1).JsonDataFilesTotal;
        File.Exists(expectedFilePath).Should().BeTrue();
        File.ReadAllText(expectedFilePath).Should().Be(
        """
        [
          {
            "op": "add",
            "path": "/meta/mapName",
            "value": "Blackheart's Bay"
          }
        ]
        """.ReplaceLineEndings("\n"));
    }

    [TestMethod]
    public async Task WriteToMapSpecific_HasItemsWithNoDiffFound_NoJsonFileCreated()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(WriteToMapSpecific_HasItemsWithNoDiffFound_NoJsonFileCreated)),
            CurrentLocale = StormLocale.ENUS,
            MapSpecificWriterJsonOutputType = MapSpecificWriterJsonOutputType.Patch,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStylesVars = false,
                ReplaceFontConstantVars = false,
                PreserveFontConstantVars = false,
                PreserveFontStyleVars = false,
            },
            HeroesVersion = new HeroesVersionOptions
            {
                Major = 2,
                Minor = 23,
                Revision = 2345,
                Build = 34566,
                IsPtr = false,
            },
            AppVersion = "5.0.0",
            AllowEmptyMapSpecificPatchFiles = false,
        };

        _options.Value.Returns(rootOptions);

        JsonPatch jsonPatch = JsonSerializer.Deserialize<JsonPatch>(
        """
        [
          {
            "op": "add",
            "path": "/meta/mapName",
            "value": "Blackheart's Bay"
          }
        ]
        """)!;
        _serializedDataStoreService.GetJsonDataPatch(default!, default!).ReturnsForAnyArgs(jsonPatch);

        SortedDictionary<string, Hero> heroesByElementId = [];
        heroesByElementId.Add("hero1", new Hero("hero1") { UnitId = "hero1" });

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "data", "maps", "map_nameyes", $"herodata_{rootOptions.BuildNumber}_enus.diff.json");

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _extractedGameStringsService);
        JsonDataFileWriterService service = new(_logger, _options, _console, _serializedDataStoreService, jsonSerializerOptionService, _resultSummaryService);

        // act
        await service.WriteToMapSpecific("map name!_yes?", heroesByElementId);

        // assert
        _serializedDataStoreService.DidNotReceiveWithAnyArgs().AddSerializedData(default!, default!);
        _ = _resultSummaryService.DidNotReceive().JsonDataFilesWritten;
        _ = _resultSummaryService.DidNotReceive().JsonDataFilesTotal;
        File.Exists(expectedFilePath).Should().BeFalse();
    }

    [TestMethod]
    [DataRow(MapSpecificWriterJsonOutputType.Normal)]
    [DataRow(MapSpecificWriterJsonOutputType.Patch)]
    [DataRow(MapSpecificWriterJsonOutputType.Normal | MapSpecificWriterJsonOutputType.Patch)]
    [DataRow(MapSpecificWriterJsonOutputType.None)]
    public async Task WriteToMapSpecific_HasItemsWithPatchWasFound_CreatesJsonFile(MapSpecificWriterJsonOutputType mapWriterJsonOutputType)
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, $"{nameof(WriteToMapSpecific_HasItemsWithPatchWasFound_CreatesJsonFile)}_{mapWriterJsonOutputType}"),
            CurrentLocale = StormLocale.ENUS,
            MapSpecificWriterJsonOutputType = mapWriterJsonOutputType,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStylesVars = false,
                ReplaceFontConstantVars = false,
                PreserveFontConstantVars = false,
                PreserveFontStyleVars = false,
            },
            HeroesVersion = new HeroesVersionOptions
            {
                Major = 2,
                Minor = 23,
                Revision = 2345,
                Build = 34566,
                IsPtr = false,
            },
            AppVersion = "5.0.0",
            JsonIndent = true,
        };

        _options.Value.Returns(rootOptions);

        JsonPatch jsonPatch = JsonSerializer.Deserialize<JsonPatch>(
        """
        [
          {
            "op": "replace",
            "path": "/meta/totalItems",
            "value": 918
          },
          {
            "op": "add",
            "path": "/meta/mapName",
            "value": "Blackheart's Bay"
          },
          {
            "op": "add",
            "path": "/items/CatapultMinion/portraits/targetInfo",
            "value": "storm_ui_ingame_targetinfopanel_unit_bb_minion_catapult.png"
          },
          {
            "op": "add",
            "path": "/items/DocksTreasureChest/portraits/targetInfo",
            "value": "storm_ui_ingame_targetinfopanel_unit_bb_doodad_chest.png"
          }
        ]
        """)!;
        _serializedDataStoreService.GetJsonDataPatch(default!, default!).ReturnsForAnyArgs(jsonPatch);

        SortedDictionary<string, Hero> heroesByElementId = [];
        heroesByElementId.Add("hero1", new Hero("hero1") { UnitId = "hero1" });

        string filePath = Path.Combine(rootOptions.OutputDirectory, "data", "maps", "map_nameyes");
        string expectedNormalFilePath = Path.Combine(filePath, $"herodata_{rootOptions.BuildNumber}_enus.json");
        string expectedPatchFilePath = Path.Combine(filePath, $"herodata_{rootOptions.BuildNumber}_enus.patch.json");

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _extractedGameStringsService); // create real instance
        JsonDataFileWriterService service = new(_logger, _options, _console, _serializedDataStoreService, jsonSerializerOptionService, _resultSummaryService);

        // act
        await service.WriteToMapSpecific("map name!_yes?", heroesByElementId);

        // assert
        _serializedDataStoreService.DidNotReceiveWithAnyArgs().AddSerializedData(default!, default!);

        if (mapWriterJsonOutputType == MapSpecificWriterJsonOutputType.All)
        {
            _ = _resultSummaryService.Received(2).JsonDataFilesWritten;
            _ = _resultSummaryService.Received(2).JsonDataFilesTotal;
        }
        else if (mapWriterJsonOutputType == MapSpecificWriterJsonOutputType.None)
        {
            _ = _resultSummaryService.DidNotReceive().JsonDataFilesWritten;
            _ = _resultSummaryService.DidNotReceive().JsonDataFilesTotal;
        }
        else
        {
            _ = _resultSummaryService.Received(1).JsonDataFilesWritten;
            _ = _resultSummaryService.Received(1).JsonDataFilesTotal;
        }

        if (mapWriterJsonOutputType.HasFlag(MapSpecificWriterJsonOutputType.Normal))
            AssertMapNormal(expectedNormalFilePath);
        else
            File.Exists(expectedNormalFilePath).Should().BeFalse();

        if (mapWriterJsonOutputType.HasFlag(MapSpecificWriterJsonOutputType.Patch))
            AssertMapPatch(expectedPatchFilePath);
        else
            File.Exists(expectedPatchFilePath).Should().BeFalse();
    }

    [TestMethod]
    public async Task Write_LocalizedTextNone_CreatesWithGamestrings()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(Write_LocalizedTextNone_CreatesWithGamestrings)),
            LocalizedText = LocalizedTextOption.None,
            CurrentLocale = StormLocale.DEDE,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStylesVars = false,
                ReplaceFontConstantVars = false,
                PreserveFontConstantVars = false,
                PreserveFontStyleVars = false,
            },
            HeroesVersion = new HeroesVersionOptions
            {
                Major = 2,
                Minor = 23,
                Revision = 2345,
                Build = 34566,
                IsPtr = true,
            },
            AppVersion = "5.0.0",
            JsonIndent = true,
        };

        _options.Value.Returns(rootOptions);

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _extractedGameStringsService); // create real instance
        JsonDataFileWriterService service = new(_logger, _options, _console, _serializedDataStoreService, jsonSerializerOptionService, _resultSummaryService);

        SortedDictionary<string, Hero> heroesByElementId = [];
        heroesByElementId.Add("hero1", new Hero("hero1")
        {
            UnitId = "hero1",
            Description = new GameStringText("Hero description"),
        });

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "data", $"herodata_{rootOptions.BuildNumber}_dede.json");

        // act
        await service.Write(heroesByElementId);

        // assert
        _serializedDataStoreService.ReceivedWithAnyArgs(1).AddSerializedData(default!, default!);
        _ = _resultSummaryService.Received(1).JsonDataFilesWritten;
        _ = _resultSummaryService.Received(1).JsonDataFilesTotal;
        File.Exists(expectedFilePath).Should().BeTrue();
        File.ReadAllText(expectedFilePath).Should().Be(
        """
        {
          "meta": {
            "heroesVersion": "2.23.2345.34566_ptr",
            "hdpVersion": "5.0.0",
            "itemsType": "Data",
            "dataType": "HeroData",
            "localizedText": "None",
            "gameStringText": {
              "locale": "DEDE",
              "textType": "RawText",
              "replaceFontConstantVars": false,
              "replaceFontStylesVars": false,
              "preserveFontConstantVars": false,
              "preserveFontStyleVars": false
            },
            "totalItems": 1
          },
          "items": {
            "hero1": {
              "unitId": "hero1",
              "franchise": "Starcraft",
              "isMelee": false,
              "gender": "Male",
              "radius": 0,
              "innerRadius": 0,
              "sight": 0,
              "speed": 0,
              "roles": [],
              "ratings": {
                "complexity": 1,
                "damage": 1,
                "survivability": 1,
                "utility": 1
              },
              "portraits": {
                "heroSelect": "",
                "leaderboard": "",
                "loading": "",
                "partyPanel": "",
                "target": "",
                "draftScreen": "",
                "partyFrames": []
              },
              "description": "Hero description"
            }
          }
        }
        """.ReplaceLineEndings("\n"));
    }

    [TestMethod]
    public async Task Write_LocalizedTextExtractNoFirstRun_CreatesWithoutGamestrings()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(Write_LocalizedTextExtractNoFirstRun_CreatesWithoutGamestrings)),
            LocalizedText = LocalizedTextOption.Extract,
            IsLocalizedExtractFirstRun = false,
            CurrentLocale = StormLocale.DEDE,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStylesVars = false,
                ReplaceFontConstantVars = false,
                PreserveFontConstantVars = false,
                PreserveFontStyleVars = false,
            },
            HeroesVersion = new HeroesVersionOptions
            {
                Major = 2,
                Minor = 23,
                Revision = 2345,
                Build = 34566,
                IsPtr = true,
            },
            AppVersion = "5.0.0",
            JsonIndent = true,
        };

        _options.Value.Returns(rootOptions);
        _extractedGameStringsService.DataGameStringItemDictionary.Returns([]);

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _extractedGameStringsService); // create real instance
        JsonDataFileWriterService service = new(_logger, _options, _console, _serializedDataStoreService, jsonSerializerOptionService, _resultSummaryService);

        SortedDictionary<string, Hero> heroesByElementId = [];
        heroesByElementId.Add("hero1", new Hero("hero1")
        {
            UnitId = "hero1",
            Description = new GameStringText("Hero description"),
        });

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "data", $"herodata_{rootOptions.BuildNumber}.json");

        // act
        await service.Write(heroesByElementId);

        // assert
        _serializedDataStoreService.DidNotReceiveWithAnyArgs().AddSerializedData(default!, default!);
        _ = _resultSummaryService.DidNotReceive().JsonDataFilesWritten;
        _ = _resultSummaryService.DidNotReceive().JsonDataFilesTotal;
        File.Exists(expectedFilePath).Should().BeFalse();
    }

    [TestMethod]
    public async Task Write_LocalizedTextExtractFirstRun_CreatesWithoutGamestrings()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, nameof(Write_LocalizedTextExtractFirstRun_CreatesWithoutGamestrings)),
            LocalizedText = LocalizedTextOption.Extract,
            IsLocalizedExtractFirstRun = true,
            CurrentLocale = StormLocale.DEDE,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStylesVars = false,
                ReplaceFontConstantVars = false,
                PreserveFontConstantVars = false,
                PreserveFontStyleVars = false,
            },
            HeroesVersion = new HeroesVersionOptions
            {
                Major = 2,
                Minor = 23,
                Revision = 2345,
                Build = 34566,
                IsPtr = true,
            },
            AppVersion = "5.0.0",
            JsonIndent = true,
        };

        _options.Value.Returns(rootOptions);
        _extractedGameStringsService.DataGameStringItemDictionary.Returns([]);

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _extractedGameStringsService); // create real instance
        JsonDataFileWriterService service = new(_logger, _options, _console, _serializedDataStoreService, jsonSerializerOptionService, _resultSummaryService);

        SortedDictionary<string, Hero> heroesByElementId = [];
        heroesByElementId.Add("hero1", new Hero("hero1")
        {
            UnitId = "hero1",
            Description = new GameStringText("Hero description"),
        });

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "data", $"herodata_{rootOptions.BuildNumber}.json");

        // act
        await service.Write(heroesByElementId);

        // assert
        _serializedDataStoreService.ReceivedWithAnyArgs(1).AddSerializedData(default!, default!);
        _ = _resultSummaryService.Received(1).JsonDataFilesWritten;
        _ = _resultSummaryService.Received(1).JsonDataFilesTotal;
        File.Exists(expectedFilePath).Should().BeTrue();
        File.ReadAllText(expectedFilePath).Should().Be(
        """
        {
          "meta": {
            "heroesVersion": "2.23.2345.34566_ptr",
            "hdpVersion": "5.0.0",
            "itemsType": "Data",
            "dataType": "HeroData",
            "localizedText": "Extract",
            "totalItems": 1
          },
          "items": {
            "hero1": {
              "unitId": "hero1",
              "franchise": "Starcraft",
              "isMelee": false,
              "gender": "Male",
              "radius": 0,
              "innerRadius": 0,
              "sight": 0,
              "speed": 0,
              "ratings": {
                "complexity": 1,
                "damage": 1,
                "survivability": 1,
                "utility": 1
              },
              "portraits": {
                "heroSelect": "",
                "leaderboard": "",
                "loading": "",
                "partyPanel": "",
                "target": "",
                "draftScreen": "",
                "partyFrames": []
              }
            }
          }
        }
        """.ReplaceLineEndings("\n"));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task Write_LocalizedTextCopy_CreatesWithGamestrings(bool isLocalizedExtractFirstRun)
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine(TestConstants.TestDirectory, $"{nameof(Write_LocalizedTextCopy_CreatesWithGamestrings)}_{isLocalizedExtractFirstRun}"),
            LocalizedText = LocalizedTextOption.Copy,
            IsLocalizedExtractFirstRun = isLocalizedExtractFirstRun,
            CurrentLocale = StormLocale.DEDE,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStylesVars = false,
                ReplaceFontConstantVars = false,
                PreserveFontConstantVars = false,
                PreserveFontStyleVars = false,
            },
            HeroesVersion = new HeroesVersionOptions
            {
                Major = 2,
                Minor = 23,
                Revision = 2345,
                Build = 34566,
                IsPtr = true,
            },
            AppVersion = "5.0.0",
            JsonIndent = true,
        };

        _options.Value.Returns(rootOptions);
        _extractedGameStringsService.DataGameStringItemDictionary.Returns([]);

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _extractedGameStringsService); // create real instance
        JsonDataFileWriterService service = new(_logger, _options, _console, _serializedDataStoreService, jsonSerializerOptionService, _resultSummaryService);

        SortedDictionary<string, Hero> heroesByElementId = [];
        heroesByElementId.Add("hero1", new Hero("hero1")
        {
            UnitId = "hero1",
            Description = new GameStringText("Hero description"),
        });

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "data", $"herodata_{rootOptions.BuildNumber}_dede.json");

        // act
        await service.Write(heroesByElementId);

        // assert
        _serializedDataStoreService.ReceivedWithAnyArgs(1).AddSerializedData(default!, default!);
        _ = _resultSummaryService.Received(1).JsonDataFilesWritten;
        _ = _resultSummaryService.Received(1).JsonDataFilesTotal;
        File.Exists(expectedFilePath).Should().BeTrue();
        File.ReadAllText(expectedFilePath).Should().Be(
        """
        {
          "meta": {
            "heroesVersion": "2.23.2345.34566_ptr",
            "hdpVersion": "5.0.0",
            "itemsType": "Data",
            "dataType": "HeroData",
            "localizedText": "Copy",
            "gameStringText": {
              "locale": "DEDE",
              "textType": "RawText",
              "replaceFontConstantVars": false,
              "replaceFontStylesVars": false,
              "preserveFontConstantVars": false,
              "preserveFontStyleVars": false
            },
            "totalItems": 1
          },
          "items": {
            "hero1": {
              "unitId": "hero1",
              "franchise": "Starcraft",
              "isMelee": false,
              "gender": "Male",
              "radius": 0,
              "innerRadius": 0,
              "sight": 0,
              "speed": 0,
              "roles": [],
              "ratings": {
                "complexity": 1,
                "damage": 1,
                "survivability": 1,
                "utility": 1
              },
              "portraits": {
                "heroSelect": "",
                "leaderboard": "",
                "loading": "",
                "partyPanel": "",
                "target": "",
                "draftScreen": "",
                "partyFrames": []
              },
              "description": "Hero description"
            }
          }
        }
        """.ReplaceLineEndings("\n"));
    }

    private static void AssertMapNormal(string expectedNormalFilePath)
    {
        File.Exists(expectedNormalFilePath).Should().BeTrue();

        File.ReadAllText(expectedNormalFilePath).Should().Be(
        """
        {
          "meta": {
            "heroesVersion": "2.23.2345.34566",
            "hdpVersion": "5.0.0",
            "itemsType": "Data",
            "dataType": "HeroData",
            "mapName": "map name!_yes?",
            "localizedText": "None",
            "gameStringText": {
              "locale": "ENUS",
              "textType": "RawText",
              "replaceFontConstantVars": false,
              "replaceFontStylesVars": false,
              "preserveFontConstantVars": false,
              "preserveFontStyleVars": false
            },
            "totalItems": 1
          },
          "items": {
            "hero1": {
              "unitId": "hero1",
              "franchise": "Starcraft",
              "isMelee": false,
              "gender": "Male",
              "radius": 0,
              "innerRadius": 0,
              "sight": 0,
              "speed": 0,
              "roles": [],
              "ratings": {
                "complexity": 1,
                "damage": 1,
                "survivability": 1,
                "utility": 1
              },
              "portraits": {
                "heroSelect": "",
                "leaderboard": "",
                "loading": "",
                "partyPanel": "",
                "target": "",
                "draftScreen": "",
                "partyFrames": []
              }
            }
          }
        }
        """.ReplaceLineEndings("\n"));
    }

    private static void AssertMapPatch(string expectedPatchFilePath)
    {
        File.Exists(expectedPatchFilePath).Should().BeTrue();

        File.ReadAllText(expectedPatchFilePath).Should().Be(
        """
        [
          {
            "op": "replace",
            "path": "/meta/totalItems",
            "value": 918
          },
          {
            "op": "add",
            "path": "/meta/mapName",
            "value": "Blackheart's Bay"
          },
          {
            "op": "add",
            "path": "/items/CatapultMinion/portraits/targetInfo",
            "value": "storm_ui_ingame_targetinfopanel_unit_bb_minion_catapult.png"
          },
          {
            "op": "add",
            "path": "/items/DocksTreasureChest/portraits/targetInfo",
            "value": "storm_ui_ingame_targetinfopanel_unit_bb_doodad_chest.png"
          }
        ]
        """.ReplaceLineEndings("\n"));
    }
}
