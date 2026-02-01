namespace HeroesDataParser.Infrastructure.Processors.Tests;

[TestClass]
public class ProcessorServiceTests
{
    private readonly ILogger<ProcessorService> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly IDataExtractorService _dataExtractorService;
    private readonly IJsonDataFileWriterService _jsonDataFileWriterService;
    private readonly IJsonGameStringFileWriterService _jsonGameStringFileWriterService;
    private readonly IImageWriterService _imageWriterService;

    public ProcessorServiceTests()
    {
        _logger = Substitute.For<ILogger<ProcessorService>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _serviceProvider = Substitute.For<IServiceProvider>();
        _dataExtractorService = Substitute.For<IDataExtractorService>();
        _jsonDataFileWriterService = Substitute.For<IJsonDataFileWriterService>();
        _jsonGameStringFileWriterService = Substitute.For<IJsonGameStringFileWriterService>();
        _imageWriterService = Substitute.For<IImageWriterService>();
    }
}
