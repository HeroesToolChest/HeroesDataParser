namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class HeroParser : CollectionParserBase<Hero>
{
    private readonly ILogger<HeroParser> _logger;
    private readonly HeroesData _heroesData;
    private readonly IUnitParser _unitParser;
    private readonly ITalentParser _talentParser;

    private readonly string _roleGameStringText;

    public HeroParser(ILogger<HeroParser> logger, IHeroesXmlLoaderService heroesXmlLoaderService, IUnitParser unitParser, ITalentParser talentParser)
        : base(logger, heroesXmlLoaderService)
    {
        _logger = logger;
        _heroesData = heroesXmlLoaderService.HeroesXmlLoader.HeroesData;
        _unitParser = unitParser;
        _talentParser = talentParser;

        _roleGameStringText = GetRoleText();
    }

    public override string DataObjectType => "Hero";

    protected override void SetAdditionalProperties(Hero collectionObject, StormElement stormElement)
    {
        StormElementData elementData = stormElement.DataValues;

        if (elementData.TryGetElementDataAt("difficulty", out StormElementData? difficultyData))
            collectionObject.Difficulty = GetTooltipDescriptionFromId(GameStringConstants.DifficultyGameString.Replace(GameStringConstants.IdPlaceHolder, difficultyData.Value.GetString()));

        SetFranchiseProperty(collectionObject, stormElement);

        if (elementData.TryGetElementDataAt("title", out StormElementData? titleData))
            collectionObject.Title = GetTooltipDescriptionFromId(titleData.Value.GetString());

        if (elementData.TryGetElementDataAt("melee", out StormElementData? meleeData) && meleeData.Value.TryGetInt32(out int meleeValue) && meleeValue == 1)
            collectionObject.IsMelee = true;

        if (elementData.TryGetElementDataAt("AlternateNameSearchText", out StormElementData? alternateNameSearchTextData))
            collectionObject.SearchText = GetTooltipDescriptionFromId(alternateNameSearchTextData.Value.GetString());

        if (elementData.TryGetElementDataAt("AdditionalSearchText", out StormElementData? additionalSearchTextData))
        {
            if (collectionObject.SearchText is not null && !string.IsNullOrWhiteSpace(collectionObject.SearchText.RawDescription))
            {
                string currentSearchText = collectionObject.SearchText.RawDescription;

                if (currentSearchText[^1] != ' ')
                    currentSearchText += ' ';

                currentSearchText += GetStormGameString(additionalSearchTextData.Value.GetString());

                collectionObject.SearchText = GetTooltipDescriptionFromGameString(currentSearchText);
            }
            else
            {
                collectionObject.SearchText = GetTooltipDescriptionFromId(additionalSearchTextData.Value.GetString());
            }
        }

        SetRoleProperty(collectionObject, stormElement);

        if (elementData.TryGetElementDataAt("ExpandedRole", out StormElementData? expandedRoleData))
            collectionObject.ExpandedRole = GetTooltipDescriptionFromId(_roleGameStringText.Replace(GameStringConstants.IdPlaceHolder, expandedRoleData.Value.GetString()));

        if (elementData.TryGetElementDataAt("ratings", out StormElementData? ratingsData))
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

        if (elementData.TryGetElementDataAt("SelectScreenButtonImage", out StormElementData? selectScreenButtonImageData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(selectScreenButtonImageData);
            if (imageFilePath is not null)
            {
                collectionObject.Portraits.HeroSelectPortrait = imageFilePath.Image;
                collectionObject.Portraits.HeroSelectPortraitPath = imageFilePath.FilePath;
            }
        }

        if (elementData.TryGetElementDataAt("ScoreScreenImage", out StormElementData? scoreScreenImageData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(scoreScreenImageData);
            if (imageFilePath is not null)
            {
                collectionObject.Portraits.LeaderboardPortrait = imageFilePath.Image;
                collectionObject.Portraits.LeaderboardPortraitPath = imageFilePath.FilePath;
            }
        }

        if (elementData.TryGetElementDataAt("LoadingScreenImage", out StormElementData? loadingScreenImageData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(loadingScreenImageData);
            if (imageFilePath is not null)
            {
                collectionObject.Portraits.LoadingScreenPortrait = imageFilePath.Image;
                collectionObject.Portraits.LoadingScreenPortraitPath = imageFilePath.FilePath;
            }
        }

        if (elementData.TryGetElementDataAt("PartyPanelButtonImage", out StormElementData? partyPanelImageData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(partyPanelImageData);
            if (imageFilePath is not null)
            {
                collectionObject.Portraits.PartyPanelPortrait = imageFilePath.Image;
                collectionObject.Portraits.PartyPanelPortraitPath = imageFilePath.FilePath;
            }
        }

        if (elementData.TryGetElementDataAt("Portrait", out StormElementData? portraitData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(portraitData);
            if (imageFilePath is not null)
            {
                collectionObject.Portraits.TargetPortrait = imageFilePath.Image;
                collectionObject.Portraits.TargetPortraitPath = imageFilePath.FilePath;
            }
        }

        if (elementData.TryGetElementDataAt("DraftScreenPortrait", out StormElementData? draftScreenPortraitData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(draftScreenPortraitData);
            if (imageFilePath is not null)
            {
                collectionObject.Portraits.DraftScreen = imageFilePath.Image;
                collectionObject.Portraits.DraftScreenPath = imageFilePath.FilePath;
            }
        }

        if (elementData.TryGetElementDataAt("PartyFrameImage", out StormElementData? partyFrameData))
        {
            ImageFilePath? imageFilePath = GetImageFilePath(partyFrameData);
            if (imageFilePath is not null)
            {
                collectionObject.Portraits.PartyFrames.Add(imageFilePath.Image);
                collectionObject.Portraits.PartyFramePaths.Add(imageFilePath.FilePath);
            }
        }

        if (elementData.TryGetElementDataAt("SkinArray", out StormElementData? skinArrayData))
        {
            foreach (string item in skinArrayData.GetElementDataIndexes())
            {
                string value = skinArrayData.GetElementDataAt(item).Value.GetString();

                if (_heroesData.StormElementExists("Skin", value))
                    collectionObject.SkinIds.Add(value);
            }
        }

        if (elementData.TryGetElementDataAt("VariationArray", out StormElementData? variationArrayData))
        {
            foreach (string item in variationArrayData.GetElementDataIndexes())
            {
                string value = variationArrayData.GetElementDataAt(item).Value.GetString();

                if (_heroesData.StormElementExists("Skin", value))
                    collectionObject.VariationSkinIds.Add(value);
            }
        }

        if (elementData.TryGetElementDataAt("VoiceLineArray", out StormElementData? voiceLineArrayData))
        {
            foreach (string item in voiceLineArrayData.GetElementDataIndexes())
            {
                string value = voiceLineArrayData.GetElementDataAt(item).Value.GetString();

                if (_heroesData.StormElementExists("VoiceLine", value))
                    collectionObject.VoiceLineIds.Add(value);
            }
        }

        if (elementData.TryGetElementDataAt("AllowedMountCategoryArray", out StormElementData? allowedMountCategoryArrayData))
        {
            foreach (string item in allowedMountCategoryArrayData.GetElementDataIndexes())
            {
                collectionObject.MountCategoryIds.Add(allowedMountCategoryArrayData.GetElementDataAt(item).Value.GetString());
            }
        }

        if (elementData.TryGetElementDataAt("DefaultMount", out StormElementData? defaultMountData))
            collectionObject.DefaultMountId = defaultMountData.Value.GetString();

        SetInfoTextProperty(collectionObject, stormElement);

        SetOtherProperties(collectionObject, stormElement);
    }

    protected override void SetValidatedProperties(Hero collectionObject)
    {
        base.SetValidatedProperties(collectionObject);

        collectionObject.Difficulty ??= GetTooltipDescriptionFromId(GameStringConstants.DifficultyGameString.Replace(GameStringConstants.IdPlaceHolder, "Easy"));
    }

    private static void SetTalentUpgradeLinkIds(Hero collectionObject)
    {
        foreach (Talent talent in collectionObject.Talents.Values.SelectMany(x => x))
        {
            List<AbilityLinkId> abilityLinkIds = collectionObject.GetTooltipAbilityLinkIdsByTalentElementId(talent.TalentElementId);

            // also search the hero units
            foreach (Unit heroUnit in collectionObject.HeroUnits.Values)
            {
                abilityLinkIds.AddRange(heroUnit.GetTooltipAbilityLinkIdsByTalentElementId(talent.TalentElementId));
            }

            // for each ability link id, add it to the talent
            foreach (AbilityLinkId abilityLInkId in abilityLinkIds)
            {
                talent.UpgradeLinkIds.AbilityLinkIds.Add(abilityLInkId);
            }
        }
    }

    private void SetOtherProperties(Hero collectionObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("Unit", out StormElementData? unitData))
            collectionObject.UnitId = unitData.Value.GetString();

        // first thing to do is to set the unit data
        SetUnitData(collectionObject);

        //// TODO: FindUnits ? e.g dva pilot

        // find additional (hero) units
        SetHeroUnits(collectionObject, stormElement);

        SetTalentData(collectionObject, stormElement);

        SetTalentUpgradeLinkIds(collectionObject);
    }

    private void SetUnitData(Hero collectionObject)
    {
        if (string.IsNullOrEmpty(collectionObject.UnitId))
        {
            _logger.LogWarning("There is no unit id for hero {Id}", collectionObject.Id);
            return;
        }

        // when calling the unitparser, we need to use the unitId for the object's id
        string heroId = collectionObject.Id;
        collectionObject.Id = collectionObject.UnitId;

        _unitParser.Parse(collectionObject);

        // then set it back to the heroId
        collectionObject.Id = heroId;
    }

    private void SetHeroUnits(Hero collectionObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("AlternateUnitArray", out StormElementData? alternateUnitArrayData))
        {
            foreach (string item in alternateUnitArrayData.GetElementDataIndexes())
            {
                string value = alternateUnitArrayData.GetElementDataAt(item).Value.GetString();
                Unit? unit = _unitParser.Parse(value);

                if (unit is not null)
                    collectionObject.HeroUnits.Add(unit.Id, unit);
            }
        }
    }

    private void SetTalentData(Hero collectionObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("TalentTreeArray", out StormElementData? talentTreeArray))
        {
            foreach (string item in talentTreeArray.GetElementDataIndexes())
            {
                StormElementData talentTreeArrayData = talentTreeArray.GetElementDataAt(item);
                Talent? talent = _talentParser.GetTalent(collectionObject, talentTreeArrayData);

                if (talent is not null)
                    collectionObject.AddTalent(talent);
            }
        }
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
                TooltipDescription? roleTooltipDescription = GetTooltipDescriptionFromId(GameStringConstants.HeroRoleGameString.Replace(GameStringConstants.IdPlaceHolder, roleData.Value.GetString()));
                if (roleTooltipDescription is not null)
                    collectionObject.Roles.Add(roleTooltipDescription);
            }
        }
    }
}

