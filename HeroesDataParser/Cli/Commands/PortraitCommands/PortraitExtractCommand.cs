namespace HeroesDataParser.Cli.Commands.PortraitCommands;

public class PortraitExtractCommand : Command<PortraitExtractSettings>
{
    private readonly ILogger<PortraitExtractCommand> _logger;
    private readonly PortraitExtractOptions _options;
    private readonly IAnsiConsole _console;
    private readonly IPortraitExtractService _portraitExtractService;

    public PortraitExtractCommand(
        ILogger<PortraitExtractCommand> logger,
        IOptions<PortraitExtractOptions> options,
        IAnsiConsole console,
        IPortraitExtractService portraitExtractService)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;
        _portraitExtractService = portraitExtractService;
    }

    protected override int Execute(CommandContext context, PortraitExtractSettings settings, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {CommandName}", nameof(PortraitExtractCommand));

        string outputDirectory;
        if (settings.OutputDirectory is null)
            outputDirectory = Path.Combine(Path.GetFullPath("."), Constants.ImagesDirectory, Constants.PortraitRewardsDirectory);
        else
            outputDirectory = settings.OutputDirectory.FullName;

        _options.OutputDirectory = outputDirectory;
        _options.RewardPortraitDataFilePath = settings.FilePath.FullName;
        _options.DeleteTextureSheet = settings.DeleteTextureSheet;

        if (string.IsNullOrWhiteSpace(settings.TextureSheetImage))
            _options.RewardPortraitTextureSheetImage = _console.Ask<string>("Enter the texturesheet image (from rewardportraitdata): ").Trim('"');
        else
            _options.RewardPortraitTextureSheetImage = settings.TextureSheetImage;

        if (settings.CacheTextureSheetImage is null)
        {
            _portraitExtractService.DisplayAvailablePortraits();

            do
            {
                ReadOnlySpan<char> filePathSpan = _console.Ask<string>("Enter the texturesheet image file path (from battle.net cache): ").AsSpan().Trim('"');

                if (filePathSpan.StartsWith('\'') && filePathSpan.EndsWith('\''))
                    filePathSpan = filePathSpan[1..^1];

                string filePath = filePathSpan.ToString();

                if (!string.IsNullOrWhiteSpace(filePath) && !File.Exists(filePath))
                {
                    _console.MarkupLine($"[red]File does not exist at path: {filePath}[/]");

                    continue;
                }

                _options.CacheTextureSheetImageFilePath = filePath;
                _console.WriteLine();

                break;
            }
            while (true);
        }
        else
        {
            _options.CacheTextureSheetImageFilePath = settings.CacheTextureSheetImage.FullName;
        }

        _portraitExtractService.Extract();

        return 0;
    }
}
