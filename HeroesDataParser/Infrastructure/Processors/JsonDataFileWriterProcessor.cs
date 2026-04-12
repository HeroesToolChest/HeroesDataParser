namespace HeroesDataParser.Infrastructure.Processors;

public class JsonDataFileWriterProcessor : IJsonDataFileWriterProcessor
{
    private readonly ILogger<JsonDataFileWriterProcessor> _logger;
    private readonly IJsonDataFileWriterService _jsonDataFileWriterService;

    private readonly List<Func<Task>> _writerTasks = [];

    public JsonDataFileWriterProcessor(ILogger<JsonDataFileWriterProcessor> logger, IJsonDataFileWriterService jsonDataFileWriterService)
    {
        _logger = logger;
        _jsonDataFileWriterService = jsonDataFileWriterService;
    }

    public void SaveJsonDataFileWrite<TElementObject>(SortedDictionary<string, TElementObject> itemsToSerialize, Map? map)
        where TElementObject : IElementObject
    {
        if (map is null)
            _writerTasks.Add(() => _jsonDataFileWriterService.Write(itemsToSerialize));
        else
            _writerTasks.Add(() => _jsonDataFileWriterService.WriteToMapSpecific(map.Id, itemsToSerialize));
    }

    public async Task ExecuteJsonDataFileWriteTasks()
    {
        if (_writerTasks.Count == 0)
        {
            _logger.LogWarning("No data file write tasks to execute.");

            return;
        }

        _logger.LogDebug("Waiting for data file write tasks to complete...");

        foreach (Func<Task> task in _writerTasks)
        {
            await task();
        }

        _writerTasks.Clear();

        _logger.LogDebug("All data file write tasks complete.");
    }
}
