using Polly;
using Polly.Registry;
using SixLabors.ImageSharp;

namespace HeroesDataParser.Infrastructure;

public class ImageWriterService : IImageWriterService
{
    private const string _imageDirectory = "images";

    private readonly ILogger<ImageWriterService> _logger;
    private readonly RootOptions _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IResultSummaryService _resultSummaryService;
    private readonly ResiliencePipeline _pipeline;

    private readonly HashSet<ImageWriterPath> _outputImagePaths = [];

    public ImageWriterService(
        ILogger<ImageWriterService> logger,
        IOptions<RootOptions> options,
        IHeroesXmlLoaderService heroesXmlLoaderService,
        IResultSummaryService resultSummaryService,
        ResiliencePipelineProvider<string> pipelineProvider)
    {
        _logger = logger;
        _options = options.Value;
        _heroesXmlLoaderService = heroesXmlLoaderService;
        _resultSummaryService = resultSummaryService;
        _pipeline = pipelineProvider.GetPipeline(Constants.ImageWriterPipeline);
    }

    public void Save(HashSet<ImageWriterPath> imagePaths)
    {
        _outputImagePaths.UnionWith(imagePaths);
    }

    public async Task Write()
    {
        if (_outputImagePaths.Count < 1)
        {
            _logger.LogInformation("No images to write");
            return;
        }

        if (!_options.Extractors.Any(x => x.Value.IsEnabled && x.Value.Images))
        {
            _logger.LogInformation("No image extractors are enabled, skipping writing images");
            return;
        }

        AnsiConsole.MarkupLine($"[lightskyblue1]Creating image files[/]...");

        var imagePathsBySubDirectoryGroups = _outputImagePaths.GroupBy(x => x.SubDirectoryPath);
        List<(ProgressTask ProgressTask, IGrouping<string, ImageWriterPath> ImagePathsBySubDirectory)> progressTasks = [];

        await AnsiConsole.Progress()
            .Columns(
            [
                new TaskDescriptionPathsColumn(),
                new ProgressBarColumn(),
                new ItemsProgressColumn(),
            ])
            .StartAsync(async ctx =>
            {
                foreach (IGrouping<string, ImageWriterPath> imagePathsBySubDirectoryGroup in imagePathsBySubDirectoryGroups)
                {
                    // Create a progress task for each sub-directory
                    progressTasks.Add((ctx.AddTask(Path.Join(_imageDirectory, imagePathsBySubDirectoryGroup.Key), maxValue: imagePathsBySubDirectoryGroup.Count()), imagePathsBySubDirectoryGroup));
                }

                await RunImageWriter(progressTasks);
            });

        // result
        foreach (var progressTask in progressTasks)
        {
            int success = (int)progressTask.ProgressTask.Value;
            int total = (int)progressTask.ProgressTask.MaxValue;

            _resultSummaryService.AddSummaryImageItem(progressTask.ImagePathsBySubDirectory.Key, success, total);
            _resultSummaryService.ImageFilesWritten += success;
            _resultSummaryService.ImageFilesTotal += total;
        }

        _outputImagePaths.Clear();
    }

    private async Task RunImageWriter(List<(ProgressTask ProgressTask, IGrouping<string, ImageWriterPath> ImagePathsBySubDirectory)> progressTasks)
    {
        using var cts = new CancellationTokenSource();

        ParallelOptions parallelOptions = new()
        {
            CancellationToken = cts.Token,
            MaxDegreeOfParallelism = _options.Threads,
        };

        await Parallel.ForEachAsync(progressTasks, parallelOptions, async (progressTask, cts) =>
        {
            progressTask.ProgressTask.StartTask();

            string directoryPath = Path.Combine(_options.OutputDirectory, _imageDirectory, progressTask.ImagePathsBySubDirectory.Key);
            Directory.CreateDirectory(directoryPath);

            foreach (ImageWriterPath imageParserPath in progressTask.ImagePathsBySubDirectory)
            {
                try
                {
                    // there is a very small chance an exception could be thrown, so we handle it via the pipeline for a retry
                    await _pipeline.ExecuteAsync(
                        async (_) =>
                        {
                            await WriteStaticImageFile(imageParserPath.FileName, directoryPath, imageParserPath);
                            progressTask.ProgressTask.Increment(1);
                        },
                        cts);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error writing image file {FileName} to directory {SubDirectory}", imageParserPath.FileName, directoryPath);
                }
            }

            progressTask.ProgressTask.StopTask();
        });
    }

    private Task WriteStaticImageFile(string fileName, string directoryPath, ImageWriterPath imageRelativeFilePath)
    {
        if (!_heroesXmlLoaderService.HeroesXmlLoader.FileExists(imageRelativeFilePath.RelativeFilePath, imageRelativeFilePath.RelativeMpqFilePath))
        {
            _logger.LogWarning("Unable to write {FileName} because {@ImageWriterPath} does not exist", fileName, imageRelativeFilePath);
            throw new FileNotFoundException("Image file not found.", imageRelativeFilePath.RelativeFilePath);
        }

        string filePath = Path.Combine(directoryPath, fileName);

        using Stream stream = _heroesXmlLoaderService.HeroesXmlLoader.GetFile(imageRelativeFilePath.RelativeFilePath, imageRelativeFilePath.RelativeMpqFilePath);

        _logger.LogTrace("Writing image file {@RelativeFilePath} to {OutputFilePath}", imageRelativeFilePath, filePath);

        if (imageRelativeFilePath.RelativeFilePath.EndsWith(".dds", StringComparison.OrdinalIgnoreCase))
        {
            using DDSImage ddsImage = new(stream);

            return ddsImage.Save(filePath);
        }
        else
        {
            using Image image = Image.Load(stream);

            return image.SaveAsync(filePath);
        }
    }
}
