namespace HeroesDataParser.Infrastructure;

public class GameStringSerializerService : IGameStringSerializerService
{
    private readonly ILogger<GameStringSerializerService> _logger;

    private readonly GameStringItemDictionary _dataGameStringItemDictionary = [];

    public GameStringSerializerService(
        ILogger<GameStringSerializerService> logger)
    {
        _logger = logger;
    }

    public GameStringItemDictionary DataGameStringItemDictionary => _dataGameStringItemDictionary;

    public byte[] SerializeGameStrings(MetaGameStringProperties metaGameStringProperties, JsonSerializerOptions jsonSerializerOptions)
    {
        return Serialize(metaGameStringProperties, jsonSerializerOptions);
    }

    public void ClearStoredGameStrings()
    {
        _dataGameStringItemDictionary.Clear();
        _logger.LogInformation("Cleared all serialized game string data.");
    }

    private byte[] Serialize(MetaGameStringProperties metaProperties, JsonSerializerOptions jsonSerializerOptions)
    {
        byte[] metaJson = JsonSerializer.SerializeToUtf8Bytes(metaProperties, jsonSerializerOptions);

        using MemoryStream memoryStream = new();
        using Utf8JsonWriter utf8JsonWriter = new(memoryStream, new JsonWriterOptions()
        {
            Indented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        });

        utf8JsonWriter.WriteStartObject();

        utf8JsonWriter.WritePropertyName(nameof(RootGameStrings.Meta).ToLowerInvariant());

        // JsonDocument instead of WriteRawValue to format the meta json properly
        using JsonDocument jsonMetaDocument = JsonDocument.Parse(metaJson);
        jsonMetaDocument.RootElement.WriteTo(utf8JsonWriter);

        utf8JsonWriter.WriteStartObject(ElementConstants.ItemsPropertyName);

        foreach (KeyValuePair<string, GameStringFilePropertyName> elementName in _dataGameStringItemDictionary)
        {
            utf8JsonWriter.WriteStartObject(elementName.Key);

            foreach (KeyValuePair<string, GameStringFilePropertyId> propertyName in elementName.Value)
            {
                utf8JsonWriter.WriteStartObject(propertyName.Key);

                if (propertyName.Value.KeyArrayPairs.Count > 0)
                {
                    foreach (KeyValuePair<string, List<GameStringText>> arrayPropertyId in propertyName.Value.KeyArrayPairs)
                    {
                        utf8JsonWriter.WriteStartArray(arrayPropertyId.Key);

                        foreach (GameStringText arrayValue in arrayPropertyId.Value)
                        {
                            utf8JsonWriter.WriteStringValue(arrayValue.RawText);
                        }

                        utf8JsonWriter.WriteEndArray();
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, GameStringText> propertyId in propertyName.Value.KeyValuePairs)
                    {
                        utf8JsonWriter.WriteString(propertyId.Key, propertyId.Value.RawText);
                    }
                }

                utf8JsonWriter.WriteEndObject();
            }

            utf8JsonWriter.WriteEndObject();
        }

        utf8JsonWriter.WriteEndObject(); // end gamestrings
        utf8JsonWriter.WriteEndObject();

        utf8JsonWriter.Flush();

        return memoryStream.ToArray();
    }
}
