using HeroesDataParser.Extensions;
using Polly;
using Polly.Registry;

namespace HeroesDataParser.Infrastructure.Tests;

[TestClass]
public class ImageWriterServiceTests
{
    private readonly ILogger<ImageWriterService> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IResultSummaryService _resultSummaryService;
    private readonly ResiliencePipelineProvider<string> _pipelineProvider;
    private readonly HeroesXmlLoader _heroesXmlLoader;

    public ImageWriterServiceTests()
    {
        _logger = Substitute.For<ILogger<ImageWriterService>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _resultSummaryService = Substitute.For<IResultSummaryService>();
        _pipelineProvider = Substitute.For<ResiliencePipelineProvider<string>>();

        _heroesXmlLoader = HeroesXmlLoader.LoadWithEmpty("TestImages");
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
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
            Extractors = new Dictionary<string, ExtractorOptions>()
            {
                {
                    "something1",
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

        ImageWriterService imageWriterService = new(_logger, _options, _heroesXmlLoaderService, _resultSummaryService, _pipelineProvider);
        imageWriterService.Save(new HashSet<ImageWriterPath>()
        {
            {
                new ImageWriterPath
                {
                    ElementId = "hero",
                    SubDirectoryPath = "heroes",
                    FileName = "heroImage1.png",
                    RelativeFilePath = "draft_screen1.dds",
                    RelativeMpqFilePath = null,
                }
            },
            {
                new ImageWriterPath
                {
                    ElementId = "map",
                    SubDirectoryPath = "maps",
                    FileName = "mapImage1.png",
                    RelativeFilePath = "hero_select_portrait1.dds",
                    RelativeMpqFilePath = null,
                }
            },
            {
                new ImageWriterPath
                {
                    ElementId = "map",
                    SubDirectoryPath = "maps",
                    FileName = "mapImage2.png",
                    RelativeFilePath = "leaderboard_portrait1.dds",
                    RelativeMpqFilePath = null,
                }
            },
            {
                new ImageWriterPath
                {
                    ElementId = "item",
                    SubDirectoryPath = "items",
                    FileName = "myItem.png",
                    RelativeFilePath = "not_a_dds_file.jpg",
                    RelativeMpqFilePath = null,
                }
            },
            {
                new ImageWriterPath
                {
                    ElementId = "map",
                    SubDirectoryPath = "maps",
                    FileName = "mapImage3.png",
                    RelativeFilePath = "this_doesnt_exists.dds",
                    RelativeMpqFilePath = null,
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
        File.Exists(Path.Combine(_options.Value.OutputDirectory, "images", "items", "myItem.png")).Should().BeTrue();

        _resultSummaryService.Received(1).AddSummaryImageItem("heroes", 1, 1);
        _resultSummaryService.Received(1).AddSummaryImageItem("maps", 2, 3);
        _resultSummaryService.Received(1).AddSummaryImageItem("items", 1, 1);
        _ = _resultSummaryService.Received(3).ImageFilesWritten;
        _ = _resultSummaryService.Received(3).ImageFilesTotal;
    }
}
