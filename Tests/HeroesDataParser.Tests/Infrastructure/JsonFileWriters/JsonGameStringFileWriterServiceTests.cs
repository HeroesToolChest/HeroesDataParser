namespace HeroesDataParser.Infrastructure.JsonFileWriters.Tests;

[TestClass]
public class JsonGameStringFileWriterServiceTests
{
    private readonly ILogger<JsonGameStringFileWriterService> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly ISavedGameStringsService _savedGameStringsService;
    private readonly IResultSummaryService _resultSummaryService;

    public JsonGameStringFileWriterServiceTests()
    {
        _logger = Substitute.For<ILogger<JsonGameStringFileWriterService>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _savedGameStringsService = Substitute.For<ISavedGameStringsService>();
        _resultSummaryService = Substitute.For<IResultSummaryService>();
    }

    [TestMethod]
    public async Task Write_HasGameStringElements_CreatesJsonFile()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(Write_HasGameStringElements_CreatesJsonFile)),
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

        List<string> dataTypes =
        [
            "AbilTalent",
            "Hero",
        ];

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "gamestrings", $"gamestrings_{rootOptions.BuildNumber}_enus.json");

        JsonSerializerOptionService jsonSerializerOptionService = new(_options, _savedGameStringsService);

        GameStringItemDictionary gameStringItemDictionary = new()
        {
            {
                "abiltalent", new GameStringFilePropertyName()
                {
                    {
                        "cooldownText", new GameStringFilePropertyId()
                        {
                            KeyValuePairs =
                            {
                                { ":PASSIVE:|ArtanisTwinBladesPrimed|W", new GameStringText("Cooldown: 4 seconds") },
                                { "AbathurAssumingDirectControlCancel|AbathurSymbioteCancel|Heroic", new GameStringText("Cooldown: 1.5 seconds") },
                            },
                        }
                    },
                    {
                        "energyText", new GameStringFilePropertyId()
                        {
                            KeyValuePairs =
                            {
                                { "AlarakDiscordStrike|AlarakDiscordStrike|Q", new GameStringText("<s val=\"StandardTooltipDetails\">Mana: 55</s>") },
                                { "AlarakTelekinesis|AlarakTelekinesis|W", new GameStringText("<s val=\"StandardTooltipDetails\">Mana: 30</s>") },
                            },
                        }
                    },
                }
            },
            {
                "Hero", new GameStringFilePropertyName()
                {
                    {
                        "difficulty", new GameStringFilePropertyId()
                        {
                            KeyValuePairs =
                            {
                                { "Abathur", new GameStringText("Very Hard") },
                            },
                        }
                    },
                    {
                        "roles", new GameStringFilePropertyId()
                        {
                            KeyArrayPairs =
                            {
                                { "Abathur", [new GameStringText("Role1")] },
                                { "Varian", [new GameStringText("Role1"), new GameStringText("Role2")] },
                            },
                        }
                    },
                }
            },
        };

        JsonGameStringFileWriterService jsonGameStringFileWriterService = new(_logger, _options, jsonSerializerOptionService, _resultSummaryService);

        // act
        await jsonGameStringFileWriterService.Write(gameStringItemDictionary, dataTypes);

        // assert
        _ = _resultSummaryService.Received(1).GameStringFilesWritten;
        _ = _resultSummaryService.Received(1).GameStringFilesTotal;
        File.Exists(expectedFilePath).Should().BeTrue();
        File.ReadAllText(expectedFilePath).Should().Be(
        """
        {
          "meta": {
            "heroesVersion": "2.23.2345.34566",
            "hdpVersion": "5.0.0",
            "dataTypes": [
              "AbilTalent",
              "Hero"
            ],
            "descriptionText": {
              "locale": "ENUS",
              "gameStringTextType": "RawText",
              "replaceFontStyles": false,
              "preserveFontStyleConstantVars": false,
              "preserveFontStyleVars": false
            }
          },
          "gamestrings": {
            "abiltalent": {
              "cooldownText": {
                ":PASSIVE:|ArtanisTwinBladesPrimed|W": "Cooldown: 4 seconds",
                "AbathurAssumingDirectControlCancel|AbathurSymbioteCancel|Heroic": "Cooldown: 1.5 seconds"
              },
              "energyText": {
                "AlarakDiscordStrike|AlarakDiscordStrike|Q": "<s val=\"StandardTooltipDetails\">Mana: 55</s>",
                "AlarakTelekinesis|AlarakTelekinesis|W": "<s val=\"StandardTooltipDetails\">Mana: 30</s>"
              }
            },
            "Hero": {
              "difficulty": {
                "Abathur": "Very Hard"
              },
              "roles": {
                "Abathur": [
                  "Role1"
                ],
                "Varian": [
                  "Role1",
                  "Role2"
                ]
              }
            }
          }
        }
        """);
    }

    [TestMethod]
    public async Task Write_NoBuildNumber_CreatesJsonFile()
    {
        // arrange
        RootOptions rootOptions = new()
        {
            OutputDirectory = Path.Combine("tests", nameof(Write_HasGameStringElements_CreatesJsonFile)),
            CurrentLocale = StormLocale.DEDE,
            HeroesVersion = new HeroesVersionOptions
            {
                Major = 2,
                Minor = 23,
                Revision = 2345,
                Build = -1,
                IsPtr = false,
            },
            AppVersion = "4.10.5",
        };

        string expectedFilePath = Path.Combine(rootOptions.OutputDirectory, "gamestrings", "gamestrings_0_dede.json");

        JsonSerializerOptionService jsonSerializerOptionService = new(new OptionsWrapper<RootOptions>(rootOptions), _savedGameStringsService);

        _options.Value.Returns(rootOptions);

        JsonGameStringFileWriterService jsonGameStringFileWriterService = new(_logger, _options, jsonSerializerOptionService, _resultSummaryService);

        // act
        await jsonGameStringFileWriterService.Write([], []);

        // assert
        _ = _resultSummaryService.Received(1).GameStringFilesWritten;
        _ = _resultSummaryService.Received(1).GameStringFilesTotal;
        File.Exists(expectedFilePath).Should().BeTrue();
        File.ReadAllText(expectedFilePath).Should().Be(
        """
        {
          "meta": {
            "heroesVersion": "2.23.2345.-1",
            "hdpVersion": "4.10.5",
            "dataTypes": [],
            "descriptionText": {
              "locale": "DEDE",
              "gameStringTextType": "RawText",
              "replaceFontStyles": false,
              "preserveFontStyleConstantVars": false,
              "preserveFontStyleVars": false
            }
          },
          "gamestrings": {}
        }
        """);
    }
}