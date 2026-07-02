using System.Diagnostics.CodeAnalysis;

namespace HeroesDataParser.Core;

public interface ISerializedDataStoreService
{
    JsonPatch? GetJsonDataPatch(DataType dataType, byte[] bytesToCompare);

    void AddSerializedData(DataType dataType, byte[] bytes);

    bool TryGetSerializedData(DataType dataType, [NotNullWhen(true)] out byte[]? bytes);

    void ClearAllSerializedData();

    IEnumerable<DataType> GetDataTypesFromData();
}
