using System.Text.Json.JsonDiffPatch;

namespace HeroesDataParser.Infrastructure;

public class SerializedElementsService : ISerializedElementsService
{
    private readonly ILogger<SerializedElementsService> _logger;

    private readonly Dictionary<string, byte[]> _serializedElementsByDataType = [];

    public SerializedElementsService(ILogger<SerializedElementsService> logger)
    {
        _logger = logger;
    }

    // dataType is for original, bytes is the new data to compare against
    public JsonNode? GetJsonNodeDiff(string dataType, byte[] bytes)
    {
        if (!_serializedElementsByDataType.TryGetValue(dataType, out byte[]? baseBytes))
        {
            _logger.LogWarning("No serialized data found for type {Type}", dataType);
            return null;
        }

        JsonNode? baseJson = JsonNode.Parse(baseBytes);
        JsonNode? newJson = JsonNode.Parse(bytes);

        return baseJson.Diff(newJson);
    }

    public void AddSerializedElements(string dataType, byte[] bytes)
    {
        _serializedElementsByDataType[dataType] = bytes;
    }

    public IEnumerable<string> GetDataTypes()
    {
        return _serializedElementsByDataType.Keys;
    }
}
