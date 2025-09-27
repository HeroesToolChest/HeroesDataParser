using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;

namespace HeroesDataParser.Infrastructure;

public class SerializedElementsService : ISerializedElementsService
{
    private readonly ILogger<SerializedElementsService> _logger;

    private readonly Dictionary<string, byte[]> _serializedElementsByDataType = [];

    public SerializedElementsService(ILogger<SerializedElementsService> logger)
    {
        _logger = logger;
    }

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
}
