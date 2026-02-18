using System.Diagnostics.CodeAnalysis;

namespace HeroesDataParser.Core;

public interface ISerializedDataStoreService
{
    JsonPatch? GetJsonDataPatch(string dataType, byte[] bytesToCompare);

    void AddSerializedData(string dataType, byte[] bytes);

    bool TryGetSerializedData(string dataType, [NotNullWhen(true)] out byte[]? bytes);

    void ClearAllSerializedData();

    IEnumerable<string> GetDataTypesFromData();
}
