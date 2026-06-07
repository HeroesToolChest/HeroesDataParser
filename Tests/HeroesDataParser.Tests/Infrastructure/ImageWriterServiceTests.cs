using Polly;
using Polly.Registry;
using Spectre.Console;

namespace HeroesDataParser.Infrastructure.Tests;

[TestClass]
public class ImageWriterServiceTests
{
    private readonly ILogger<ImageWriterService> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IAnsiConsole _console;
    private readonly IResultSummaryService _resultSummaryService;
    private readonly ResiliencePipelineProvider<string> _pipelineProvider;

    public ImageWriterServiceTests()
    {
        _logger = Substitute.For<ILogger<ImageWriterService>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _console = new TestConsole();

        _resultSummaryService = Substitute.For<IResultSummaryService>();
        _pipelineProvider = Substitute.For<ResiliencePipelineProvider<string>>();
    }

    [TestMethod]
    public async Task Write_HasImagesToCreate_CreatesImages()
    {
        // arrange
        _pipelineProvider
            .GetPipeline(Constants.ImageWriterPipeline)
            .Returns(ResiliencePipeline.Empty);

        _options.Value.Returns(new RootOptions()
        {
            Extractors = new Dictionary<ExtractDataOptions, ExtractorOptions>()
            {
                {
                    ExtractDataOptions.AnnouncerPack,
                    new ExtractorOptions()
                    {
                        IsEnabled = true,
                        Images = true,
                    }
                },
            },
            OutputDirectory = "ImageWriterServiceTests",
            Threads = 1,
        });

        ImageWriterService imageWriterService = new(_logger, _options, _console, _resultSummaryService, _pipelineProvider);
        imageWriterService.Save(new HashSet<ImageWriterFile>()
        {
            {
                new ImageWriterFile
                {
                    ElementId = "hero",
                    SubDirectoryPath = "heroes",
                    FileName = "heroImage1.png",
                    ProcessImageFile = async (outputPath) =>
                    {
                        File.WriteAllText(Path.Combine(_options.Value.OutputDirectory, "images", "heroes", "heroImage1.png"), "This is a test image file for heroImage1.");
                    },
                }
            },
            {
                new ImageWriterFile
                {
                    ElementId = "map",
                    SubDirectoryPath = "maps",
                    FileName = "mapImage1.png",
                    ProcessImageFile = async (outputPath) =>
                    {
                        File.WriteAllText(Path.Combine(_options.Value.OutputDirectory, "images", "maps", "mapImage1.png"), "This is a test image file for mapImage1.");
                    },
                }
            },
            {
                new ImageWriterFile
                {
                    ElementId = "map",
                    SubDirectoryPath = "maps",
                    FileName = "mapImage2.png",
                    ProcessImageFile = async (outputPath) =>
                    {
                        File.WriteAllText(Path.Combine(_options.Value.OutputDirectory, "images", "maps", "mapImage2.png"), "This is a test image file for mapImage2.");
                    },
                }
            },
            {
                new ImageWriterFile
                {
                    ElementId = "item",
                    SubDirectoryPath = "items",
                    FileName = "myItem.png",
                    ProcessImageFile = async (outputPath) =>
                    {
                        File.WriteAllText(Path.Combine(_options.Value.OutputDirectory, "images", "items", "not_a_dds_file.jpg"), "This is a test image file for myItem.");
                    },
                }
            },
            {
                new ImageWriterFile
                {
                    ElementId = "map",
                    SubDirectoryPath = "maps",
                    FileName = "mapImage3.png",
                    ProcessImageFile = async (outputPath) =>
                    {
                        throw new Exception("Simulated image processing failure.");
                    },
                }
            },
        });

        // act
        await imageWriterService.Write();

        // assert
        File.Exists(Path.Combine(_options.Value.OutputDirectory, "images", "heroes", "heroImage1.png")).Should().BeTrue();
        File.Exists(Path.Combine(_options.Value.OutputDirectory, "images", "maps", "mapImage1.png")).Should().BeTrue();
        File.Exists(Path.Combine(_options.Value.OutputDirectory, "images", "maps", "mapImage2.png")).Should().BeTrue();
        File.Exists(Path.Combine(_options.Value.OutputDirectory, "images", "maps", "mapImage3.png")).Should().BeFalse();
        File.Exists(Path.Combine(_options.Value.OutputDirectory, "images", "items", "not_a_dds_file.jpg")).Should().BeTrue();

        _resultSummaryService.Received(1).AddSummaryImageItem("heroes", 1, 1);
        _resultSummaryService.Received(1).AddSummaryImageItem("maps", 2, 3);
        _resultSummaryService.Received(1).AddSummaryImageItem("items", 1, 1);
        _ = _resultSummaryService.Received(3).ImageFilesWritten;
        _ = _resultSummaryService.Received(3).ImageFilesTotal;
    }
}
