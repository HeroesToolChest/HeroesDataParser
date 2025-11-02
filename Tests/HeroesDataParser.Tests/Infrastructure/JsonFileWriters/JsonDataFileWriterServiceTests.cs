using System.Text.Json.Nodes;

namespace HeroesDataParser.Infrastructure.JsonFileWriters.Tests;

[TestClass]
public class JsonDataFileWriterServiceTests
{
    private readonly ILogger<JsonDataFileWriterService> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly ISerializedElementsService _serializedElementsService;
    private readonly ISavedGameStringsService _savedGameStringsService;

    public JsonDataFileWriterServiceTests()
    {
        _logger = Substitute.For<ILogger<JsonDataFileWriterService>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _serializedElementsService = Substitute.For<ISerializedElementsService>();
        _savedGameStringsService = Substitute.For<ISavedGameStringsService>();
    }

    [TestMethod]
    public async Task Write_HasItems_CreatesJsonFile()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(Write_HasItems_CreatesJsonFile)),
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

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _savedGameStringsService);

        JsonDataFileWriterService service = new(_logger, _options, _serializedElementsService, jsonSerializerOptionService);

        Dictionary<string, Hero> heroesByElementId = [];
        heroesByElementId.Add("hero1", new Hero("hero1") { UnitId = "hero1" });
        heroesByElementId.Add("hero2", new Hero("hero2") { UnitId = "hero2" });

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "data", $"herodata_{rootOptions.BuildNumber}_enus.json");

        // act
        await service.Write(heroesByElementId);

        // assert
        _serializedElementsService.ReceivedWithAnyArgs(1).AddSerializedElements(default!, default!);
        File.Exists(expectedFilePath).Should().BeTrue();
        File.ReadAllText(expectedFilePath).Should().Be(
        """
        {
          "meta": {
            "heroesVersion": "2.23.2345.34566",
            "hdpVersion": "5.0.0",
            "dataType": "herodata",
            "localizedText": "None",
            "totalItems": 2
          },
          "items": {
            "hero1": {
              "name": null,
              "sortName": null,
              "unitId": "hero1",
              "title": null,
              "difficulty": null,
              "isMelee": false,
              "gender": "Male",
              "radius": 0,
              "innerRadius": 0,
              "sight": 0,
              "speed": 0,
              "expandedRole": null,
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
              "searchText": null,
              "description": null,
              "infoText": null
            },
            "hero2": {
              "name": null,
              "sortName": null,
              "unitId": "hero2",
              "title": null,
              "difficulty": null,
              "isMelee": false,
              "gender": "Male",
              "radius": 0,
              "innerRadius": 0,
              "sight": 0,
              "speed": 0,
              "expandedRole": null,
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
              "searchText": null,
              "description": null,
              "infoText": null
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

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _savedGameStringsService);

        JsonDataFileWriterService service = new(_logger, _options, _serializedElementsService, jsonSerializerOptionService);

        _options.Value.Returns(rootOptions);

        Dictionary<string, Hero> heroesByElementId = [];

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "data", $"herodata_{rootOptions.BuildNumber}_enus.json");

        // act
        await service.Write(heroesByElementId);

        // assert
        _serializedElementsService.DidNotReceiveWithAnyArgs().AddSerializedElements(default!, default!);
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
        _serializedElementsService.GetJsonNodeDiff(default!, default!).ReturnsForAnyArgs(jsonDiff);

        Dictionary<string, Hero> heroesByElementId = [];
        heroesByElementId.Add("hero1", new Hero("hero1") { UnitId = "hero1" });

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "data", "maps", "map_nameyes", $"herodata_{rootOptions.BuildNumber}_enus.json.diff");

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _savedGameStringsService);
        JsonDataFileWriterService service = new(_logger, _options, _serializedElementsService, jsonSerializerOptionService);

        // act
        await service.WriteToMaps("map name!_yes?", heroesByElementId);

        // assert
        _serializedElementsService.DidNotReceiveWithAnyArgs().AddSerializedElements(default!, default!);
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
        _serializedElementsService.GetJsonNodeDiff(default!, default!).ReturnsForAnyArgs(jsonDiff);

        Dictionary<string, Hero> heroesByElementId = [];
        heroesByElementId.Add("hero1", new Hero("hero1") { UnitId = "hero1" });

        string expectedNormalFilePath = Path.Combine(rootOptions.OutputDirectory, "data", "maps", "map_nameyes", $"herodata_{rootOptions.BuildNumber}_enus.json");
        string expectedDiffFilePath = $"{expectedNormalFilePath}.diff";

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _savedGameStringsService);
        JsonDataFileWriterService service = new(_logger, _options, _serializedElementsService, jsonSerializerOptionService);

        // act
        await service.WriteToMaps("map name!_yes?", heroesByElementId);

        // assert
        _serializedElementsService.DidNotReceiveWithAnyArgs().AddSerializedElements(default!, default!);

        if (mapWriterJsonOutputType.HasFlag(MapWriterJsonOutputType.Normal))
            AssertMapNormal(expectedNormalFilePath, expectedDiffFilePath);
        else
            File.Exists(expectedNormalFilePath).Should().BeFalse();

        if (mapWriterJsonOutputType.HasFlag(MapWriterJsonOutputType.Diff))
            AssertMapDiff(expectedNormalFilePath, expectedDiffFilePath);
        else
            File.Exists(expectedDiffFilePath).Should().BeFalse();
    }

    private static void AssertMapNormal(string expectedNormalFilePath, string expectedDiffFilePath)
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
            "totalItems": 1
          },
          "items": {
            "hero1": {
              "name": null,
              "sortName": null,
              "unitId": "hero1",
              "title": null,
              "difficulty": null,
              "isMelee": false,
              "gender": "Male",
              "radius": 0,
              "innerRadius": 0,
              "sight": 0,
              "speed": 0,
              "expandedRole": null,
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
              "searchText": null,
              "description": null,
              "infoText": null
            }
          }
        }
        """);
    }

    private static void AssertMapDiff(string expectedNormalFilePath, string expectedDiffFilePath)
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
