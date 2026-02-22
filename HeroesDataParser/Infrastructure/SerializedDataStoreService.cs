using System.Diagnostics.CodeAnalysis;

namespace HeroesDataParser.Infrastructure;

public class SerializedDataStoreService : ISerializedDataStoreService
{
    private readonly ILogger<SerializedDataStoreService> _logger;

    private readonly Dictionary<DataType, byte[]> _serializedDataByDataType = [];

    public SerializedDataStoreService(ILogger<SerializedDataStoreService> logger)
    {
        _logger = logger;
    }

    // dataType is for looking up the original (existing) data, bytes is the new data to compare against
    public JsonPatch? GetJsonDataPatch(DataType dataType, byte[] bytesToCompare)
    {
        if (!_serializedDataByDataType.TryGetValue(dataType, out byte[]? baseBytes))
        {
            _logger.LogWarning("No serialized data found for type {Type}", dataType);
            return null;
        }

        return GetJsonPatchFromBytes(bytesToCompare, baseBytes);
    }

    public void AddSerializedData(DataType dataType, byte[] bytes)
    {
        _serializedDataByDataType[dataType] = bytes;
    }

    public bool TryGetSerializedData(DataType dataType, [NotNullWhen(true)] out byte[]? bytes)
    {
        return _serializedDataByDataType.TryGetValue(dataType, out bytes);
    }

    public void ClearAllSerializedData()
    {
        _serializedDataByDataType.Clear();
    }

    public IEnumerable<DataType> GetDataTypesFromData() => _serializedDataByDataType.Keys;

    private static JsonPatch GetJsonPatchFromBytes(byte[] bytesToCompare, byte[] baseBytes)
    {
        JsonNode? baseJson = JsonNode.Parse(baseBytes);
        JsonNode? newJson = JsonNode.Parse(bytesToCompare);

        return baseJson.CreatePatch(newJson);
    }
}
