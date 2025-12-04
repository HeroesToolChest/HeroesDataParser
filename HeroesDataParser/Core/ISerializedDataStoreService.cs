namespace HeroesDataParser.Core;

public interface ISerializedDataStoreService
{
    JsonNode? GetJsonDataDiff(string dataType, byte[] bytesToCompare);

    void AddSerializedData(string dataType, byte[] bytes);

    bool TryGetSerializedData(string dataType, out byte[]? bytes);

    void ClearAllSerializedData();

    IEnumerable<string> GetDataTypesFromData();
}
