namespace HeroesDataParser.Infrastructure.Tests;

[TestClass]
public class GameStringSerializerServiceTests
{
    private readonly ILogger<GameStringSerializerService> _logger;
    private readonly IJsonSerializerOptionService _jsonSerializerOptionService;

    public GameStringSerializerServiceTests()
    {
        _logger = Substitute.For<ILogger<GameStringSerializerService>>();
        _jsonSerializerOptionService = Substitute.For<IJsonSerializerOptionService>();
    }

    [TestMethod]
    public void SerializeGameStrings_WithEmptyDictionary_ReturnsJsonWithMetaAndEmptyItems()
    {
        // arrange
        MetaGameStringProperties metaProperties = new()
        {
            HeroesVersion = new HeroesDataVersion(2, 55, 3, 96477),
            HdpVersion = "5.0.0",
        };

        _jsonSerializerOptionService.JsonSerializerGameStringOptions.Returns(new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters =
            {
                new JsonStringEnumConverter(),
                new HeroesDataVersionConverter(),
            },
        });

        GameStringSerializerService service = new(_logger, _jsonSerializerOptionService);

        // act
        byte[] result = service.SerializeGameStrings(metaProperties);

        // assert
        string json = System.Text.Encoding.UTF8.GetString(result);

        json.Should().Be(
        """
        {
          "meta": {
            "heroesVersion": "2.55.3.96477",
            "hdpVersion": "5.0.0",
            "itemsType": "Other",
            "dataTypes": [],
            "gameStringText": {
              "locale": "ENUS",
              "replaceFontStyles": false,
              "preserveFontStyleConstantVars": false,
              "preserveFontStyleVars": false
            }
          },
          "items": {}
        }
        """);
    }

    [TestMethod]
    public void SerializeGameStrings_WithPopulatedDictionary_ReturnsJsonWithGameStringData()
    {
        // arrange
        GameStringSerializerService service = new(_logger, _jsonSerializerOptionService);

        GameStringFilePropertyId propertyId = new();
        propertyId.KeyValuePairs["HeroAbathur"] = new GameStringText("Abathur");

        service.DataGameStringItemDictionary["hero"] = new GameStringFilePropertyName()
        {
            ["name"] = propertyId,
        };

        MetaGameStringProperties metaProperties = new()
        {
            HeroesVersion = new HeroesDataVersion(2, 55, 3, 96477),
            HdpVersion = "5.0.0",
        };

        _jsonSerializerOptionService.JsonSerializerGameStringOptions.Returns(new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters =
            {
                new JsonStringEnumConverter(),
                new GameStringTextConverter(gameStringTextType: GameStringTextType.RawText),
                new HeroesDataVersionConverter(),
                new GameStringItemDictionaryConverter(),
            },
        });

        // act
        byte[] result = service.SerializeGameStrings(metaProperties);

        // assert
        string json = System.Text.Encoding.UTF8.GetString(result);

        json.Should().Be(
        """
        {
          "meta": {
            "heroesVersion": "2.55.3.96477",
            "hdpVersion": "5.0.0",
            "itemsType": "Other",
            "dataTypes": [],
            "gameStringText": {
              "locale": "ENUS",
              "replaceFontStyles": false,
              "preserveFontStyleConstantVars": false,
              "preserveFontStyleVars": false
            }
          },
          "items": {
            "hero": {
              "name": {
                "HeroAbathur": "Abathur"
              }
            }
          }
        }
        """);
    }

    [TestMethod]
    public void SerializeGameStrings_DoesNotClearDictionaryAfterSerialization_DictionaryRetainsData()
    {
        // arrange
        GameStringSerializerService service = new(_logger, _jsonSerializerOptionService);

        GameStringFilePropertyId propertyId = new();
        propertyId.KeyValuePairs["HeroAbathur"] = new GameStringText("Abathur");

        service.DataGameStringItemDictionary["hero"] = new GameStringFilePropertyName()
        {
            ["name"] = propertyId,
        };

        MetaGameStringProperties metaProperties = new()
        {
            HeroesVersion = new HeroesDataVersion(2, 55, 3, 96477),
            HdpVersion = "5.0.0",
        };

        _jsonSerializerOptionService.JsonSerializerGameStringOptions.Returns(new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters =
            {
                new JsonStringEnumConverter(),
                new GameStringTextConverter(gameStringTextType: GameStringTextType.RawText),
                new HeroesDataVersionConverter(),
                new GameStringItemDictionaryConverter(),
            },
        });

        // act
        service.SerializeGameStrings(metaProperties);

        // assert
        service.DataGameStringItemDictionary.Should().NotBeEmpty();
        service.DataGameStringItemDictionary.Should().ContainKey("hero");
    }

    [TestMethod]
    public void ClearStoredGameStrings_WithPopulatedDictionary_ClearsDictionary()
    {
        // arrange
        GameStringSerializerService service = new(_logger, _jsonSerializerOptionService);

        GameStringFilePropertyId propertyId = new();
        propertyId.KeyValuePairs["HeroAbathur"] = new GameStringText("Abathur");

        service.DataGameStringItemDictionary["hero"] = new GameStringFilePropertyName()
        {
            ["name"] = propertyId,
        };

        // act
        service.ClearStoredGameStrings();

        // assert
        service.DataGameStringItemDictionary.Should().BeEmpty();
    }

    [TestMethod]
    public void ClearStoredGameStrings_WithEmptyDictionary_RemainsEmpty()
    {
        // arrange
        GameStringSerializerService service = new(_logger, _jsonSerializerOptionService);

        // act
        service.ClearStoredGameStrings();

        // assert
        service.DataGameStringItemDictionary.Should().BeEmpty();
    }
}
