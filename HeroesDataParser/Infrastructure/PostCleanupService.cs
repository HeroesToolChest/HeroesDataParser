namespace HeroesDataParser.Infrastructure;

public class PostCleanupService : IPostCleanupService
{
    private readonly ILogger<PostCleanupService> _logger;
    private readonly RootOptions _options;

    public PostCleanupService(ILogger<PostCleanupService> logger, IOptions<RootOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public void Start()
    {
        CleanDataMapDirectories();
        CleanGameStringDirectories();
    }

    private void CleanDataMapDirectories()
    {
        if (_options.AllowEmptyMapSpecificDirectories)
            return;

        string dataMapsDirectory = Path.Combine(_options.OutputDirectory, Constants.JsonDataDirectory, Constants.MapDirectory);

        _logger.LogInformation("Cleaning up empty map data directories...");
        DeleteEmtpyDiretories(dataMapsDirectory);
    }

    private void CleanGameStringDirectories()
    {
        if (_options.AllowEmptyMapSpecificDirectories)
            return;

        string gamestringsMapsDirectory = Path.Combine(_options.OutputDirectory, Constants.JsonGameStringsDirectory, Constants.MapDirectory);

        _logger.LogInformation("Cleaning up empty map gamestring directories...");
        DeleteEmtpyDiretories(gamestringsMapsDirectory);
    }

    private void DeleteEmtpyDiretories(string rootDirectory)
    {
        if (!Directory.Exists(rootDirectory))
            return;

        IEnumerable<string> directories = Directory.EnumerateDirectories(rootDirectory);
        foreach (string directory in directories)
        {
            if (!Directory.EnumerateFileSystemEntries(directory).Any())
            {
                try
                {
                    Directory.Delete(directory);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Could not delete empty directory {Directory}", directory);
                }
            }
        }

        if (!Directory.EnumerateFileSystemEntries(rootDirectory).Any())
        {
            try
            {
                Directory.Delete(rootDirectory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not delete empty root directory {Directory}", rootDirectory);
            }
        }
    }
}
