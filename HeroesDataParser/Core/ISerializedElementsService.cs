using System.Text.Json.Nodes;

namespace HeroesDataParser.Core;

public interface ISerializedElementsService
{
    JsonNode? GetJsonNodeDiff(string dataType, byte[] bytes);

    void AddSerializedElements(string dataType, byte[] bytes);
}
