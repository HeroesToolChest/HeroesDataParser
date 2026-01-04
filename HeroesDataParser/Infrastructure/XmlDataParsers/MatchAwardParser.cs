namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class MatchAwardParser : DataParser<MatchAward>
{
    public MatchAwardParser(ILogger<MatchAwardParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IGameStringTextService gameStringTextService)
        : base(logger, options, heroesXmlLoaderService, gameStringTextService)
    {
    }

    public override string DataObjectType => "ScoreValue";

    protected override string[] AllowedElementTypes => ["CScoreValueCustom"];

    protected override void SetProperties(MatchAward elementObject, StormElement stormElement)
    {
        // set gamelink to the original id before modifying it, the new id should be the id found in the replays
        elementObject.GameLink = elementObject.Id;

        if (stormElement.DataValues.TryGetElementDataAt("hdp-Id", out StormElementData? hdpIdData))
            elementObject.Id = hdpIdData.Value.GetString();
        else
            elementObject.Id = GetId(elementObject.Id);

        if (stormElement.DataValues.TryGetElementDataAt("UniqueTag", out StormElementData? uniqueTagData))
            elementObject.Tag = uniqueTagData.Value.GetString();

        if (stormElement.DataValues.TryGetElementDataAt("Name", out StormElementData? nameData))
            elementObject.ScoreScreenName = GameStringTextService.GetGameStringTextFromId(nameData.Value.GetString());

        if (stormElement.DataValues.TryGetElementDataAt("Tooltip", out StormElementData? tooltipData))
            elementObject.ScoreScreenDescription = GameStringTextService.GetGameStringTextFromId(tooltipData.Value.GetString());

        SetIcons(stormElement, elementObject);
        SetEndOfMatchProperties(elementObject);
    }

    private static string GetId(ReadOnlySpan<char> gamelink)
    {
        if (gamelink.StartsWith("EndOfMatchAward", StringComparison.OrdinalIgnoreCase))
            gamelink = gamelink["EndOfMatchAward".Length..];
        if (gamelink.EndsWith("Boolean", StringComparison.OrdinalIgnoreCase))
            gamelink = gamelink[..gamelink.IndexOf("Boolean", StringComparison.OrdinalIgnoreCase)];

        if (gamelink[0] == '0')
            return $"Zero{gamelink[1..]}";

        return gamelink.ToString();
    }

    private static string GetMVPScreenImageName(ReadOnlySpan<char> name) => $"storm_ui_mvp_{name}_%color%.png";

    private void SetIcons(StormElement stormElement, MatchAward matchAward)
    {
        if (!stormElement.DataValues.TryGetElementDataAt("Icon", out StormElementData? iconData))
            return;

        string iconPath = iconData.Value.GetString();
        ImageFilePath? imageFileBluePath = GetImageFilePath(iconPath.Replace("%team%", "blue"));
        ImageFilePath? imageFileRedPath = GetImageFilePath(iconPath.Replace("%team%", "red"));

        matchAward.ScoreScreenImageBluePath = imageFileBluePath?.FilePath;
        matchAward.ScoreScreenImageRedPath = imageFileRedPath?.FilePath;
        matchAward.ScoreScreenImage = GetImageOutputFileName(iconPath);

        if (stormElement.DataValues.TryGetElementDataAt("hdp-MvpIcon", out StormElementData? hdpMvpIconData))
        {
            string mvpImagePath = hdpMvpIconData.Value.GetString();
            ImageFilePath? imageFilePath = GetImageFilePath(mvpImagePath);

            if (imageFilePath is not null)
            {
                matchAward.MVPScreenImage = GetMVPScreenImageName(hdpMvpIconData["Name"].Value.GetString());
                matchAward.MVPScreenImagePath = imageFilePath.FilePath;
            }
        }
        else
        {
            SetMVPIcon(matchAward);
        }
    }

    private void SetMVPIcon(MatchAward matchAward)
    {
        int index = 0;
        ReadOnlySpan<char> scoreScreenImageSpan = matchAward.ScoreScreenImage;

        foreach (Range imageNameSegment in scoreScreenImageSpan.Split('_'))
        {
            if (index < 4)
            {
                index++;
                continue;
            }

            ReadOnlySpan<char> nameSegment = scoreScreenImageSpan[imageNameSegment];

            string imagePath = $"Assets\\Textures\\storm_ui_mvp_icons_rewards_{nameSegment}.dds";

            ImageFilePath? imageFilePath = GetImageFilePath(imagePath);
            if (imageFilePath is null)
                return;

            matchAward.MVPScreenImage = GetMVPScreenImageName(nameSegment); // custom set for json output
            matchAward.MVPScreenImagePath = imageFilePath.FilePath;

            return;
        }
    }

    private void SetEndOfMatchProperties(MatchAward elementObject)
    {
        if (string.IsNullOrEmpty(elementObject.GameLink))
            return;

        ProcessEndOfMatchAward(elementObject, "EndOfMatchGeneralAward");
        ProcessEndOfMatchAward(elementObject, "EndOfMatchMapSpecificAward");
    }

    private void ProcessEndOfMatchAward(MatchAward elementObject, string elementUserId)
    {
        StormElement? userStormElement = HeroesData.GetCompleteStormElement("User", elementUserId);

        if (userStormElement is null || !userStormElement.DataValues.TryGetElementDataAt("instances", out StormElementData? instancesData))
            return;

        IEnumerable<string> instancesIndexes = instancesData.GetElementDataIndexes();

        foreach (string instanceIndex in instancesIndexes)
        {
            StormElementData currentInstanceData = instancesData[instanceIndex];

            if (!currentInstanceData.TryGetElementDataAt("gamelink", out StormElementData? gamelinkData) || !gamelinkData.TryGetElementDataAt("gamelink", out StormElementData? innerGamelinkData))
                continue;

            string gamelink = innerGamelinkData.Value.GetString();

            // check for the correct gamelink
            if (string.IsNullOrEmpty(gamelink) || !elementObject.GameLink!.Equals(gamelink, StringComparison.Ordinal))
                continue;

            SetTextFieldProperties(elementObject, currentInstanceData);
        }
    }

    private void SetTextFieldProperties(MatchAward elementObject, StormElementData currentInstanceData)
    {
        if (!currentInstanceData.TryGetElementDataAt("Text", out StormElementData? textData))
            return;

        IEnumerable<string> textIndexes = textData.GetElementDataIndexes();

        foreach (string textIndex in textIndexes)
        {
            StormElementData currentTextData = textData[textIndex];

            if (currentTextData.TryGetElementDataAt("Field", out StormElementData? fieldData))
            {
                if (fieldData.TryGetElementDataAt("id", out StormElementData? idData))
                {
                    string idValue = idData.Value.GetString();
                    if (idValue.Equals("Award Name", StringComparison.Ordinal))
                    {
                        if (currentTextData.TryGetElementDataAt("text", out StormElementData? innerTextData))
                        {
                            elementObject.EndOfMatchName = GameStringTextService.GetGameStringTextFromId(innerTextData.Value.GetString());
                        }
                    }
                    else if (idValue.Equals("Description", StringComparison.Ordinal))
                    {
                        if (currentTextData.TryGetElementDataAt("text", out StormElementData? innerTextData))
                        {
                            elementObject.EndOfMatchDescription = GameStringTextService.GetGameStringTextFromId(innerTextData.Value.GetString());
                        }
                    }
                    else if (idValue.Equals("Tooltip Text", StringComparison.Ordinal))
                    {
                        if (currentTextData.TryGetElementDataAt("text", out StormElementData? innerTextData))
                        {
                            elementObject.EndOfMatchTooltipText = GameStringTextService.GetGameStringTextFromId(innerTextData.Value.GetString());
                        }
                    }
                }
            }
        }
    }
}
