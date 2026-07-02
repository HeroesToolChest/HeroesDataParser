namespace HeroesDataParser.Cli.Commands.PortraitCommands;

public class PortraitInfoCommand : Command<PortraitInfoSettings>
{
    private readonly ILogger<PortraitInfoCommand> _logger;
    private readonly PortraitInfoOptions _options;
    private readonly IPortraitInfoService _portraitInfoService;

    public PortraitInfoCommand(ILogger<PortraitInfoCommand> logger, IOptions<PortraitInfoOptions> options, IPortraitInfoService portraitInfoService)
    {
        _logger = logger;
        _options = options.Value;
        _portraitInfoService = portraitInfoService;
    }

    protected override int Execute(CommandContext context, PortraitInfoSettings settings, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {CommandName}", nameof(PortraitInfoCommand));

        _options.RewardPortraitDataFilePath = settings.FilePath.FullName;
        _options.ShowTextureSheetsFileNames = settings.TextureSheets;
        _options.ShowIconSlotFileNames = settings.IconSlot;
        _options.TextureSheetImageName = settings.TextureSheetImage;

        _portraitInfoService.DisplayInfo();

        return 0;
    }
}
