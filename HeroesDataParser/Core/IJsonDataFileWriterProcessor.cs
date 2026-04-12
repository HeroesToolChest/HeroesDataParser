namespace HeroesDataParser.Core;

public interface IJsonDataFileWriterProcessor
{
    void SaveJsonDataFileWrite<TElementObject>(SortedDictionary<string, TElementObject> itemsToSerialize, Map? map)
        where TElementObject : IElementObject;

    Task ExecuteJsonDataFileWriteTasks();
}
