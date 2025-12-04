using NSubstitute.ReceivedExtensions;
using System.Text.Json.Nodes;

namespace HeroesDataParser.Infrastructure.JsonFileWriters.Tests;

[TestClass]
public class JsonDataFileWriterServiceTests
{
    private readonly ILogger<JsonDataFileWriterService> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly ISerializedDataStoreService _serializedDataStoreService;
    private readonly IResultSummaryService _resultSummaryService;
    private readonly IGameStringSerializerService _extractedGameStringsService;

    public JsonDataFileWriterServiceTests()
    {
        _logger = Substitute.For<ILogger<JsonDataFileWriterService>>();
        _options = Substitute.For<IOptions<RootOptions>>();
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
            OutputDirectory = Path.Combine("tests", nameof(Write_HasItems_CreatesJsonFile)),
            LocalizedText = LocalizedTextOption.None,
            CurrentLocale = StormLocale.ENUS,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStyles = false,
                PreserveFont = new PreserveFontOptions
                {
                    PreserveFontStyleConstantVars = false,
                    PreserveFontStyleVars = false,
                },
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
        };

        _options.Value.Returns(rootOptions);

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _extractedGameStringsService); // create real instance
        JsonDataFileWriterService service = new(_logger, _options, _serializedDataStoreService, jsonSerializerOptionService, _resultSummaryService);

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
            "dataType": "herodata",
            "localizedText": "None",
            "descriptionText": {
              "locale": "ENUS",
              "gameStringTextType": "RawText",
              "replaceFontStyles": false,
              "preserveFontStyleConstantVars": false,
              "preserveFontStyleVars": false
            },
            "totalItems": 2
          },
          "items": {
            "hero1": {
              "unitId": "hero1",
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
        """);
    }

    [TestMethod]
    public async Task Write_NoItems_NoFilesCreated()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(Write_NoItems_NoFilesCreated)),
        };

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _extractedGameStringsService); // create real instance
        JsonDataFileWriterService service = new(_logger, _options, _serializedDataStoreService, jsonSerializerOptionService, _resultSummaryService);

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
    public async Task WriteToMaps_HasItemsWithNoDiffFound_CreatesJsonFile()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(WriteToMaps_HasItemsWithNoDiffFound_CreatesJsonFile)),
            CurrentLocale = StormLocale.ENUS,
            MapWriterJsonOutputType = MapWriterJsonOutputType.Diff,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStyles = false,
                PreserveFont = new PreserveFontOptions
                {
                    PreserveFontStyleConstantVars = false,
                    PreserveFontStyleVars = false,
                },
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
            AllowEmptyDiffFiles = true,
        };

        _options.Value.Returns(rootOptions);

        JsonNode jsonDiff = JsonNode.Parse(
        """
        {
          "meta": {
            "mapName": [
              "Blackheart's Bay"
            ]
          }
        }
        """)!;
        _serializedDataStoreService.GetJsonDataDiff(default!, default!).ReturnsForAnyArgs(jsonDiff);

        SortedDictionary<string, Hero> heroesByElementId = [];
        heroesByElementId.Add("hero1", new Hero("hero1") { UnitId = "hero1" });

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "data", "maps", "map_nameyes", $"herodata_{rootOptions.BuildNumber}_enus.json.diff");

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _extractedGameStringsService);
        JsonDataFileWriterService service = new(_logger, _options, _serializedDataStoreService, jsonSerializerOptionService, _resultSummaryService);

        // act
        await service.WriteToMaps("map name!_yes?", heroesByElementId);

        // assert
        _serializedDataStoreService.DidNotReceiveWithAnyArgs().AddSerializedData(default!, default!);
        _ = _resultSummaryService.Received(1).JsonDataFilesWritten;
        _ = _resultSummaryService.Received(1).JsonDataFilesTotal;
        File.Exists(expectedFilePath).Should().BeTrue();
        File.ReadAllText(expectedFilePath).Should().Be(
        """
        {
          "meta": {
            "mapName": [
              "Blackheart's Bay"
            ]
          }
        }
        """);
    }

    [TestMethod]
    public async Task WriteToMaps_HasItemsWithNoDiffFound_NoJsonFileCreated()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(WriteToMaps_HasItemsWithNoDiffFound_NoJsonFileCreated)),
            CurrentLocale = StormLocale.ENUS,
            MapWriterJsonOutputType = MapWriterJsonOutputType.Diff,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStyles = false,
                PreserveFont = new PreserveFontOptions
                {
                    PreserveFontStyleConstantVars = false,
                    PreserveFontStyleVars = false,
                },
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
            AllowEmptyDiffFiles = false,
        };

        _options.Value.Returns(rootOptions);

        JsonNode jsonDiff = JsonNode.Parse(
        """
        {
          "meta": {
            "mapName": [
              "Blackheart's Bay"
            ]
          }
        }
        """)!;
        _serializedDataStoreService.GetJsonDataDiff(default!, default!).ReturnsForAnyArgs(jsonDiff);

        SortedDictionary<string, Hero> heroesByElementId = [];
        heroesByElementId.Add("hero1", new Hero("hero1") { UnitId = "hero1" });

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "data", "maps", "map_nameyes", $"herodata_{rootOptions.BuildNumber}_enus.json.diff");

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _extractedGameStringsService);
        JsonDataFileWriterService service = new(_logger, _options, _serializedDataStoreService, jsonSerializerOptionService, _resultSummaryService);

        // act
        await service.WriteToMaps("map name!_yes?", heroesByElementId);

        // assert
        _serializedDataStoreService.DidNotReceiveWithAnyArgs().AddSerializedData(default!, default!);
        _ = _resultSummaryService.DidNotReceive().JsonDataFilesWritten;
        _ = _resultSummaryService.DidNotReceive().JsonDataFilesTotal;
        File.Exists(expectedFilePath).Should().BeFalse();
    }

    [TestMethod]
    [DataRow(MapWriterJsonOutputType.Normal)]
    [DataRow(MapWriterJsonOutputType.Diff)]
    [DataRow(MapWriterJsonOutputType.Normal | MapWriterJsonOutputType.Diff)]
    [DataRow(MapWriterJsonOutputType.None)]
    public async Task WriteToMaps_HasItemsWithDiffWasFound_CreatesJsonFile(MapWriterJsonOutputType mapWriterJsonOutputType)
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", $"{nameof(WriteToMaps_HasItemsWithDiffWasFound_CreatesJsonFile)}_{mapWriterJsonOutputType}"),
            CurrentLocale = StormLocale.ENUS,
            MapWriterJsonOutputType = mapWriterJsonOutputType,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStyles = false,
                PreserveFont = new PreserveFontOptions
                {
                    PreserveFontStyleConstantVars = false,
                    PreserveFontStyleVars = false,
                },
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
        };

        _options.Value.Returns(rootOptions);

        JsonNode jsonDiff = JsonNode.Parse(
        """
        {
          "meta": {
            "totalItems": [
              972,
              973
            ],
            "mapName": [
              "Blackheart's Bay"
            ]
          },
          "items": {
            "CatapultMinion": {
              "portraits": {
                "targetInfo": [
                  "storm_ui_ingame_targetinfopanel_unit_bb_minion_catapult.png"
                ]
              }
            },
            "BlackheartsCoreBombardMissile": [
              {
                "radius": 0,
                "innerRadius": 0,
                "sight": 0,
                "speed": 0,
                "life": {
                  "amount": 10,
                  "scale": 0,
                  "regenRate": 0,
                  "regenScale": 0
                },
                "portraits": {}
              }
            ]
          }
        }
        """)!;
        _serializedDataStoreService.GetJsonDataDiff(default!, default!).ReturnsForAnyArgs(jsonDiff);

        SortedDictionary<string, Hero> heroesByElementId = [];
        heroesByElementId.Add("hero1", new Hero("hero1") { UnitId = "hero1" });

        string expectedNormalFilePath = Path.Combine(rootOptions.OutputDirectory, "data", "maps", "map_nameyes", $"herodata_{rootOptions.BuildNumber}_enus.json");
        string expectedDiffFilePath = $"{expectedNormalFilePath}.diff";

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _extractedGameStringsService); // create real instance
        JsonDataFileWriterService service = new(_logger, _options, _serializedDataStoreService, jsonSerializerOptionService, _resultSummaryService);

        // act
        await service.WriteToMaps("map name!_yes?", heroesByElementId);

        // assert
        _serializedDataStoreService.DidNotReceiveWithAnyArgs().AddSerializedData(default!, default!);

        if (mapWriterJsonOutputType == MapWriterJsonOutputType.All)
        {
            _ = _resultSummaryService.Received(2).JsonDataFilesWritten;
            _ = _resultSummaryService.Received(2).JsonDataFilesTotal;
        }
        else if (mapWriterJsonOutputType == MapWriterJsonOutputType.None)
        {
            _ = _resultSummaryService.DidNotReceive().JsonDataFilesWritten;
            _ = _resultSummaryService.DidNotReceive().JsonDataFilesTotal;
        }
        else
        {
            _ = _resultSummaryService.Received(1).JsonDataFilesWritten;
            _ = _resultSummaryService.Received(1).JsonDataFilesTotal;
        }

        if (mapWriterJsonOutputType.HasFlag(MapWriterJsonOutputType.Normal))
            AssertMapNormal(expectedNormalFilePath);
        else
            File.Exists(expectedNormalFilePath).Should().BeFalse();

        if (mapWriterJsonOutputType.HasFlag(MapWriterJsonOutputType.Diff))
            AssertMapDiff(expectedDiffFilePath);
        else
            File.Exists(expectedDiffFilePath).Should().BeFalse();
    }

    [TestMethod]
    public async Task Write_LocalizedTextNone_CreatesWithGamestrings()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(Write_LocalizedTextNone_CreatesWithGamestrings)),
            LocalizedText = LocalizedTextOption.None,
            CurrentLocale = StormLocale.DEDE,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStyles = false,
                PreserveFont = new PreserveFontOptions
                {
                    PreserveFontStyleConstantVars = false,
                    PreserveFontStyleVars = false,
                },
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
        };

        _options.Value.Returns(rootOptions);

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _extractedGameStringsService); // create real instance
        JsonDataFileWriterService service = new(_logger, _options, _serializedDataStoreService, jsonSerializerOptionService, _resultSummaryService);

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
            "dataType": "herodata",
            "localizedText": "None",
            "descriptionText": {
              "locale": "DEDE",
              "gameStringTextType": "RawText",
              "replaceFontStyles": false,
              "preserveFontStyleConstantVars": false,
              "preserveFontStyleVars": false
            },
            "totalItems": 1
          },
          "items": {
            "hero1": {
              "unitId": "hero1",
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
        """);
    }

    [TestMethod]
    public async Task Write_LocalizedTextExtract_CreatesWithoutGamestrings()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(Write_LocalizedTextExtract_CreatesWithoutGamestrings)),
            LocalizedText = LocalizedTextOption.Extract,
            CurrentLocale = StormLocale.DEDE,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStyles = false,
                PreserveFont = new PreserveFontOptions
                {
                    PreserveFontStyleConstantVars = false,
                    PreserveFontStyleVars = false,
                },
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
        };

        _options.Value.Returns(rootOptions);
        _extractedGameStringsService.DataGameStringItemDictionary.Returns([]);

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _extractedGameStringsService); // create real instance
        JsonDataFileWriterService service = new(_logger, _options, _serializedDataStoreService, jsonSerializerOptionService, _resultSummaryService);

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
            "dataType": "herodata",
            "localizedText": "Extract",
            "totalItems": 1
          },
          "items": {
            "hero1": {
              "unitId": "hero1",
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
        """);
    }

    [TestMethod]
    public async Task Write_LocalizedTextCopy_CreatesWithGamestrings()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(Write_LocalizedTextCopy_CreatesWithGamestrings)),
            LocalizedText = LocalizedTextOption.Copy,
            CurrentLocale = StormLocale.DEDE,
            GameStringText = new GameStringTextOptions
            {
                Type = GameStringTextType.RawText,
                ReplaceFontStyles = false,
                PreserveFont = new PreserveFontOptions
                {
                    PreserveFontStyleConstantVars = false,
                    PreserveFontStyleVars = false,
                },
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
        };

        _options.Value.Returns(rootOptions);
        _extractedGameStringsService.DataGameStringItemDictionary.Returns([]);

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _extractedGameStringsService); // create real instance
        JsonDataFileWriterService service = new(_logger, _options, _serializedDataStoreService, jsonSerializerOptionService, _resultSummaryService);

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
            "dataType": "herodata",
            "localizedText": "Copy",
            "descriptionText": {
              "locale": "DEDE",
              "gameStringTextType": "RawText",
              "replaceFontStyles": false,
              "preserveFontStyleConstantVars": false,
              "preserveFontStyleVars": false
            },
            "totalItems": 1
          },
          "items": {
            "hero1": {
              "unitId": "hero1",
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
        """);
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
            "dataType": "herodata",
            "mapName": "map name!_yes?",
            "localizedText": "None",
            "descriptionText": {
              "locale": "ENUS",
              "gameStringTextType": "RawText",
              "replaceFontStyles": false,
              "preserveFontStyleConstantVars": false,
              "preserveFontStyleVars": false
            },
            "totalItems": 1
          },
          "items": {
            "hero1": {
              "unitId": "hero1",
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
        """);
    }

    private static void AssertMapDiff(string expectedDiffFilePath)
    {
        File.Exists(expectedDiffFilePath).Should().BeTrue();

        File.ReadAllText(expectedDiffFilePath).Should().Be(
        """
        {
          "meta": {
            "totalItems": [
              972,
              973
            ],
            "mapName": [
              "Blackheart's Bay"
            ]
          },
          "items": {
            "CatapultMinion": {
              "portraits": {
                "targetInfo": [
                  "storm_ui_ingame_targetinfopanel_unit_bb_minion_catapult.png"
                ]
              }
            },
            "BlackheartsCoreBombardMissile": [
              {
                "radius": 0,
                "innerRadius": 0,
                "sight": 0,
                "speed": 0,
                "life": {
                  "amount": 10,
                  "scale": 0,
                  "regenRate": 0,
                  "regenScale": 0
                },
                "portraits": {}
              }
            ]
          }
        }
        """);
    }
}
