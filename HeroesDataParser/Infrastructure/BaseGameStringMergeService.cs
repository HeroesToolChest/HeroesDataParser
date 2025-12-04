namespace HeroesDataParser.Infrastructure;

public class BaseGameStringMergeService : IBaseGameStringMergeService
{
    private readonly ILogger<BaseGameStringMergeService> _logger;
    private readonly ISerializedDataStoreService _serializedDataStoreService;
    private readonly IGameStringSerializerService _gameStringSerializerService;
    private readonly IJsonSerializerOptionService _jsonSerializerOptionService;

    private readonly string _mapPropertyName = JsonNamingPolicy.CamelCase.ConvertName(nameof(Map));

    public BaseGameStringMergeService(
        ILogger<BaseGameStringMergeService> logger,
        ISerializedDataStoreService serializedDataStoreService,
        IGameStringSerializerService gameStringSerializerService,
        IJsonSerializerOptionService jsonSerializerOptionService)
    {
        _logger = logger;
        _serializedDataStoreService = serializedDataStoreService;
        _gameStringSerializerService = gameStringSerializerService;
        _jsonSerializerOptionService = jsonSerializerOptionService;
    }

    public byte[]? MergeWithMap()
    {
        // get existing base game strings
        if (!_serializedDataStoreService.TryGetSerializedData(Constants.GameStringFilePrefix, out byte[]? baseBytes))
        {
            _logger.LogError("Base game strings not found in serialized data store.");
            return null;
        }

        // serialize and get the bytes of the current extracted gamestrings (maps)
        byte[] currentBytes = _gameStringSerializerService.SerializeGameStrings(new MetaGameStringProperties(), _jsonSerializerOptionService.JsonSerializerGameStringOptions);

        JsonNode? baseNode = JsonNode.Parse(baseBytes);
        if (baseNode is not JsonObject baseObject)
        {
            _logger.LogError("Failed to parse base game strings as JSON object.");
            return null;
        }

        using JsonDocument currentDocument = JsonDocument.Parse(currentBytes);

        if (baseObject[ElementConstants.RootMetaPropertyName] is not JsonObject baseMeta)
        {
            _logger.LogError("Missing '{MetaPropertyName}' property in base game strings.", ElementConstants.RootMetaPropertyName);
            return null;
        }

        // check if both have "items" property
        if (baseObject[ElementConstants.ItemsPropertyName] is not JsonObject baseItems)
        {
            _logger.LogError("Missing '{ItemsPropertyName}' property in base game strings.", ElementConstants.ItemsPropertyName);
            return null;
        }

        if (!currentDocument.RootElement.TryGetProperty(ElementConstants.ItemsPropertyName, out JsonElement currentItems))
        {
            _logger.LogError("Missing '{ItemsPropertyName}' property in current game strings.", ElementConstants.ItemsPropertyName);
            return null;
        }

        // check if current has "map" object
        if (!currentItems.TryGetProperty(_mapPropertyName, out JsonElement mapElement))
        {
            _logger.LogError("No '{MapPropertyName}' object found in current game strings.", _mapPropertyName);
            return null;
        }

        AddMapDataToDataTypes(baseMeta);
        MergeMapIntoBaseItems(baseItems, mapElement);

        _logger.LogInformation("Successfully merged '{MapPropertyName}' object into base game strings.", _mapPropertyName);

        // serialize the modified base back to bytes
        return JsonSerializer.SerializeToUtf8Bytes(baseObject, _jsonSerializerOptionService.JsonSerializerGameStringOptions);
    }

    private void AddMapDataToDataTypes(JsonObject baseMeta)
    {
        if (baseMeta[ElementConstants.MetaDataTypesPropertyName] is not JsonArray dataTypesArray)
        {
            // create the array if it doesn't exist
            dataTypesArray = [];
            baseMeta[ElementConstants.MetaDataTypesPropertyName] = dataTypesArray;
        }

        dataTypesArray.Add($"{_mapPropertyName}{Constants.ElementDataSuffix}");
    }

    private void MergeMapIntoBaseItems(JsonObject baseItems, JsonElement mapElement)
    {
        using MemoryStream memoryStream = new();
        using Utf8JsonWriter utf8JsonWriter = new(memoryStream);
        mapElement.WriteTo(utf8JsonWriter);
        utf8JsonWriter.Flush();
        memoryStream.Position = 0;

        // copy the map object to base items
        baseItems[_mapPropertyName] = JsonNode.Parse(memoryStream);
    }
}
