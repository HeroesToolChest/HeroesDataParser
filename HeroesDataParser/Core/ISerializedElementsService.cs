namespace HeroesDataParser.Core;

public interface ISerializedElementsService
{
    JsonNode? GetJsonNodeDiff(string dataType, byte[] bytes);

    void AddSerializedElements(string dataType, byte[] bytes);

    void ClearSerializedElements();

    IEnumerable<string> GetDataTypes();
}
