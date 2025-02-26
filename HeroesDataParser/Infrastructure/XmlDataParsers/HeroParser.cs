namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class HeroParser : CollectionParserBase<Hero>
{
    private readonly ILogger<HeroParser> _logger;
    private readonly HeroesData _heroesData;

    private readonly string _roleGameStringText;

    public HeroParser(ILogger<HeroParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService)
        : base(logger, heroesXmlLoaderService)
    {
        _logger = logger;
        _heroesData = heroesXmlLoaderService.HeroesXmlLoader.HeroesData;

        _roleGameStringText = GetRoleText();
    }

    public override string DataObjectType => "Hero";

    protected override void SetAdditionalProperties(Hero collectionObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("difficulty", out StormElementData? difficultyData))
            collectionObject.Difficulty = GetTooltipDescriptionFromId(GameStringConstants.DifficultyGameString.Replace(GameStringConstants.IdPlaceHolder, difficultyData.Value.GetString()));

        SetFranchiseProperty(collectionObject, stormElement);

        if (stormElement.DataValues.TryGetElementDataAt("title", out StormElementData? titleData))
            collectionObject.Title = GetTooltipDescriptionFromId(titleData.Value.GetString());

        if (stormElement.DataValues.TryGetElementDataAt("melee", out StormElementData? meleeData) && meleeData.Value.TryGetInt32(out int meleeValue) && meleeValue == 1)
            collectionObject.IsMelee = true;

        if (stormElement.DataValues.TryGetElementDataAt("AlternateNameSearchText", out StormElementData? alternateNameSearchTextData))
            collectionObject.SearchText = GetStormGameString(alternateNameSearchTextData.Value.GetString());

        if (stormElement.DataValues.TryGetElementDataAt("AdditionalSearchText", out StormElementData? additionalSearchTextData))
        {
            if (!string.IsNullOrWhiteSpace(collectionObject.SearchText))
            {
                if (collectionObject.SearchText[^1] != ' ')
                    collectionObject.SearchText += ' ';

                collectionObject.SearchText += GetStormGameString(additionalSearchTextData.Value.GetString());
            }
            else
            {
                collectionObject.SearchText = GetStormGameString(additionalSearchTextData.Value.GetString());
            }
        }

        SetRoleProperty(collectionObject, stormElement);

        if (stormElement.DataValues.TryGetElementDataAt("ExpandedRole", out StormElementData? expandedRoleData))
            collectionObject.ExpandedRole = GetTooltipDescriptionFromId(_roleGameStringText.Replace(GameStringConstants.IdPlaceHolder, expandedRoleData.Value.GetString()));

        if (stormElement.DataValues.TryGetElementDataAt("ratings", out StormElementData? ratingsData))
        {
            if (ratingsData.TryGetElementDataAt("damage", out StormElementData? damageData))
                collectionObject.Ratings.Damage = damageData.Value.GetDouble();
            if (ratingsData.TryGetElementDataAt("utility", out StormElementData? utilityData))
                collectionObject.Ratings.Utility = utilityData.Value.GetDouble();
            if (ratingsData.TryGetElementDataAt("survivability", out StormElementData? survivabilityData))
                collectionObject.Ratings.Survivability = survivabilityData.Value.GetDouble();
            if (ratingsData.TryGetElementDataAt("complexity", out StormElementData? complexityData))
                collectionObject.Ratings.Complexity = complexityData.Value.GetDouble();
        }

        if (stormElement.DataValues.TryGetElementDataAt("SelectScreenButtonImage", out StormElementData? selectScreenButtonImageData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(selectScreenButtonImageData);
            if (imageFilePath is not null)
            {
                collectionObject.Portraits.HeroSelectPortrait = imageFilePath.Image;
                collectionObject.Portraits.HeroSelectPortraitPath = imageFilePath.FilePath;
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("ScoreScreenImage", out StormElementData? scoreScreenImageData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(scoreScreenImageData);
            if (imageFilePath is not null)
            {
                collectionObject.Portraits.LeaderboardPortrait = imageFilePath.Image;
                collectionObject.Portraits.LeaderboardPortraitPath = imageFilePath.FilePath;
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("LoadingScreenImage", out StormElementData? loadingScreenImageData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(loadingScreenImageData);
            if (imageFilePath is not null)
            {
                collectionObject.Portraits.LoadingScreenPortrait = imageFilePath.Image;
                collectionObject.Portraits.LoadingScreenPortraitPath = imageFilePath.FilePath;
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("PartyPanelButtonImage", out StormElementData? partyPanelImageData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(partyPanelImageData);
            if (imageFilePath is not null)
            {
                collectionObject.Portraits.PartyPanelPortrait = imageFilePath.Image;
                collectionObject.Portraits.PartyPanelPortraitPath = imageFilePath.FilePath;
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("Portrait", out StormElementData? portraitData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(portraitData);
            if (imageFilePath is not null)
            {
                collectionObject.Portraits.TargetPortrait = imageFilePath.Image;
                collectionObject.Portraits.TargetPortraitPath = imageFilePath.FilePath;
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("DraftScreenPortrait", out StormElementData? draftScreenPortraitData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(draftScreenPortraitData);
            if (imageFilePath is not null)
            {
                collectionObject.Portraits.DraftScreen = imageFilePath.Image;
                collectionObject.Portraits.DraftScreenPath = imageFilePath.FilePath;
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("PartyFrameImage", out StormElementData? partyFrameData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(partyFrameData);
            if (imageFilePath is not null)
            {
                collectionObject.Portraits.PartyFrames.Add(imageFilePath.Image);
                collectionObject.Portraits.PartyFramePaths.Add(imageFilePath.FilePath);
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("SkinArray", out StormElementData? skinArrayData))
        {
            foreach (string item in skinArrayData.GetElementDataIndexes())
            {
                collectionObject.SkinIds.Add(skinArrayData.GetElementDataAt(item).Value.GetString());
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("VariationArray", out StormElementData? variationArrayData))
        {
            foreach (string item in variationArrayData.GetElementDataIndexes())
            {
                collectionObject.VariationSkinIds.Add(variationArrayData.GetElementDataAt(item).Value.GetString());
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("VoiceLineArray", out StormElementData? voiceLineArrayData))
        {
            foreach (string item in voiceLineArrayData.GetElementDataIndexes())
            {
                collectionObject.VoiceLineIds.Add(voiceLineArrayData.GetElementDataAt(item).Value.GetString());
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("AllowedMountCategoryArray", out StormElementData? allowedMountCategoryArrayData))
        {
            foreach (string item in allowedMountCategoryArrayData.GetElementDataIndexes())
            {
                collectionObject.MountCategoryIds.Add(allowedMountCategoryArrayData.GetElementDataAt(item).Value.GetString());
            }
        }

        if (stormElement.DataValues.TryGetElementDataAt("DefaultMount", out StormElementData? defaultMountData))
            collectionObject.DefaultMountId = defaultMountData.Value.GetString();
    }

    protected override void SetValidatedProperties(Hero collectionObject)
    {
        base.SetValidatedProperties(collectionObject);

        collectionObject.Difficulty ??= GetTooltipDescriptionFromId(GameStringConstants.DifficultyGameString.Replace(GameStringConstants.IdPlaceHolder, "Easy"));
    }

    private string GetRoleText()
    {
        StormElement? stormElement = _heroesData.GetStormElement("CHeroRole");
        if (stormElement is not null && stormElement.DataValues.TryGetElementDataAt("name", out StormElementData? nameData))
        {
            return nameData.Value.GetString();
        }

        _logger.LogWarning("Could not get role game string text from CHeroRole");
        return string.Empty;
    }

    private void SetRoleProperty(Hero collectionObject, StormElement stormElement)
    {
        AddRole("role");
        AddRole("RolesMultiClass");

        void AddRole(string index)
        {
            if (stormElement.DataValues.TryGetElementDataAt(index, out StormElementData? roleData))
            {
                string? roleGameString = roleData.Value.GetString();

                if (!string.IsNullOrWhiteSpace(roleGameString))
                {
                    StormGameString? stormGameString = _heroesData.GetStormGameString(roleGameString);

                    if (stormGameString is not null)
                        collectionObject.Roles.Add(stormGameString.Value);
                    else
                        _logger.LogWarning("Could not get storm game string from {RoleGameString}", roleGameString);
                }
            }
        }
    }
}

