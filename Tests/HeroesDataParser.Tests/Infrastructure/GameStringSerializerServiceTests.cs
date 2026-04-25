namespace HeroesDataParser.Infrastructure.Tests;

[TestClass]
public class GameStringSerializerServiceTests
{
    private readonly ILogger<GameStringSerializerService> _logger;

    public GameStringSerializerServiceTests()
    {
        _logger = Substitute.For<ILogger<GameStringSerializerService>>();
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

        GameStringSerializerService service = new(_logger);

        // act
        byte[] result = service.SerializeGameStrings(metaProperties, JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions());

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
              "replaceFontConstantVars": false,
              "replaceFontStylesVars": false,
              "preserveFontConstantVars": false,
              "preserveFontStyleVars": false
            }
          },
          "items": {}
        }
        """.ReplaceLineEndings("\n"));
    }

    [TestMethod]
    public void SerializeGameStrings_WithPopulatedDictionary_ReturnsJsonWithGameStringData()
    {
        // arrange
        GameStringSerializerService service = new(_logger);

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

        JsonSerializerOptions jsonSerializerOptions = JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions();
        jsonSerializerOptions.Converters.Add(new GameStringTextConverter(new GameStringTextConverterOptions() { GameStringTextType = GameStringTextType.RawText }));
        jsonSerializerOptions.Converters.Add(new GameStringItemDictionaryConverter());

        // act
        byte[] result = service.SerializeGameStrings(metaProperties, jsonSerializerOptions);

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
              "replaceFontConstantVars": false,
              "replaceFontStylesVars": false,
              "preserveFontConstantVars": false,
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
        """.ReplaceLineEndings("\n"));
    }

    [TestMethod]
    public void SerializeGameStrings_DoesNotClearDictionaryAfterSerialization_DictionaryRetainsData()
    {
        // arrange
        GameStringSerializerService service = new(_logger);

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

        JsonSerializerOptions jsonSerializerOptions = JsonGeneralSerializerOptions.GetGeneralJsonSerializerOptions();
        jsonSerializerOptions.Converters.Add(new GameStringTextConverter(new GameStringTextConverterOptions() { GameStringTextType = GameStringTextType.RawText }));

        // act
        service.SerializeGameStrings(metaProperties, jsonSerializerOptions);

        // assert
        service.DataGameStringItemDictionary.Should().NotBeEmpty();
        service.DataGameStringItemDictionary.Should().ContainKey("hero");
    }

    [TestMethod]
    public void ClearStoredGameStrings_WithPopulatedDictionary_ClearsDictionary()
    {
        // arrange
        GameStringSerializerService service = new(_logger);

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
        GameStringSerializerService service = new(_logger);

        // act
        service.ClearStoredGameStrings();

        // assert
        service.DataGameStringItemDictionary.Should().BeEmpty();
    }
}
