using Polly;
using Polly.Registry;

namespace HeroesDataParser.Infrastructure;

public class ImageWriterService : IImageWriterService
{
    private const string _imageDirectory = "images";

    private readonly ILogger<ImageWriterService> _logger;
    private readonly RootOptions _options;
    private readonly IAnsiConsole _console;
    private readonly IResultSummaryService _resultSummaryService;
    private readonly ResiliencePipeline _pipeline;

    private readonly HashSet<ImageWriterFile> _outputImagePaths = [];

    public ImageWriterService(
        ILogger<ImageWriterService> logger,
        IOptions<RootOptions> options,
        IAnsiConsole console,
        IResultSummaryService resultSummaryService,
        ResiliencePipelineProvider<string> pipelineProvider)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;
        _resultSummaryService = resultSummaryService;
        _pipeline = pipelineProvider.GetPipeline(Constants.ImageWriterPipeline);
    }

    public void Save(HashSet<ImageWriterFile> imagePaths)
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

        _console.MarkupLine($"[lightskyblue1]Creating image files[/]...");

        var imagePathsBySubDirectoryGroups = _outputImagePaths.GroupBy(x => x.SubDirectoryPath);
        List<(ProgressTask ProgressTask, IGrouping<string, ImageWriterFile> ImagePathsBySubDirectory)> progressTasks = [];

        await _console.Progress()
            .Columns(
            [
                new TaskDescriptionPathsColumn(),
                new ProgressBarColumn(),
                new ItemsProgressColumn(),
            ])
            .StartAsync(async ctx =>
            {
                foreach (IGrouping<string, ImageWriterFile> imagePathsBySubDirectoryGroup in imagePathsBySubDirectoryGroups)
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

    private async Task RunImageWriter(List<(ProgressTask ProgressTask, IGrouping<string, ImageWriterFile> ImagePathsBySubDirectory)> progressTasks)
    {
        ParallelOptions parallelOptions = new()
        {
            MaxDegreeOfParallelism = _options.Threads,
        };

        await Parallel.ForEachAsync(progressTasks, parallelOptions, async (progressTask, cts) =>
        {
            progressTask.ProgressTask.StartTask();

            string directoryPath = Path.Combine(_options.OutputDirectory, _imageDirectory, progressTask.ImagePathsBySubDirectory.Key);
            Directory.CreateDirectory(directoryPath);

            await Parallel.ForEachAsync(progressTask.ImagePathsBySubDirectory, parallelOptions, async (imageWriterFile, cts) =>
            {
                try
                {
                    // there is a very small chance an exception could be thrown, so we handle it via the pipeline for a retry
                    await _pipeline.ExecuteAsync(
                        async (_) =>
                        {
                            await imageWriterFile.ProcessImageFile(directoryPath);

                            progressTask.ProgressTask.Increment(1);
                        },
                        cts);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error writing image file {FileName} to directory {SubDirectory}", imageWriterFile.FileName, directoryPath);
                }
            });

            progressTask.ProgressTask.StopTask();
        });
    }
}
