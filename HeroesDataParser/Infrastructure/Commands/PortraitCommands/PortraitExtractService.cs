using Heroes.Element;

namespace HeroesDataParser.Infrastructure.Commands.PortraitCommands;

public class PortraitExtractService : PortraitExtractBase, IPortraitExtractService
{
    private readonly PortraitExtractOptions _options;

    private List<RewardPortrait>? _rewardPortraits;

    public PortraitExtractService(ILogger<PortraitExtractService> logger, IOptions<PortraitExtractOptions> options, IAnsiConsole console)
        : base(logger, console)
    {
        _options = options.Value;
    }

    public void Extract()
    {
        OutputDirectory = _options.OutputDirectory;

        ExtractImageFiles(GetRewardPortraits(), _options.CacheTextureSheetImageFilePath, _options.RewardPortraitTextureSheetImage, _options.DeleteTextureSheet);
    }

    public void DisplayAvailablePortraits()
    {
        DisplayPortraitsFromTextureSheetImage(GetRewardPortraits(), _options.RewardPortraitTextureSheetImage);
    }

    private List<RewardPortrait> GetRewardPortraits()
    {
        if (_rewardPortraits is null)
        {
            using JsonDocument jsonDocument = JsonDocument.Parse(File.ReadAllBytes(_options.RewardPortraitDataFilePath));
            using RewardPortraitDataDocument rewardPortraitDataDocument = RewardPortraitDataDocument.Load(jsonDocument);

            _rewardPortraits = [.. rewardPortraitDataDocument.GetElements()];
        }

        return _rewardPortraits;
    }
}
