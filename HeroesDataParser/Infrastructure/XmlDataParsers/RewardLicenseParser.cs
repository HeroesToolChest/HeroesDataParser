namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class RewardLicenseParser : DataParser<RewardLicense>
{
    public RewardLicenseParser(ILogger<RewardLicenseParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
        : base(logger, options, heroesXmlLoaderService, gameStringTextService)
    {
    }

    public override string DataObjectType => "Reward";

    public override RewardLicense? Parse(string id)
    {
        RewardLicense? rewardLicense = base.Parse(id);

        // rewards without a license are never in a RequiredRewardArray, so they are not needed
        if (rewardLicense is null || rewardLicense.LicenseId < 1)
        {
            Logger.LogTrace("Reward {Id} has no license", id);
            return null;
        }

        return rewardLicense;
    }

    protected override void SetProperties(RewardLicense elementObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("License", out StormElementData? licenseIdData) && long.TryParse(licenseIdData.Value.GetString(), out long licenseId))
            elementObject.LicenseId = licenseId;
    }
}
