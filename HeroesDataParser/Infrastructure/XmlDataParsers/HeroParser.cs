namespace HeroesDataParser.Infrastructure.XmlDataParsers;

public class HeroParser : LoadoutItemParserBase<Hero>
{
    private readonly IUnitParser _unitParser;
    private readonly ITalentParser _talentParser;

    private readonly string _roleGameStringText;

    public HeroParser(ILogger<HeroParser> logger, IOptions<RootOptions> options, IHeroesXmlLoaderService heroesXmlLoaderService, IUnitParser unitParser, ITalentParser talentParser, IGameStringTextService gameStringTextService)
        : base(logger, options, heroesXmlLoaderService, gameStringTextService)
    {
        _unitParser = unitParser;
        _talentParser = talentParser;

        _roleGameStringText = GetRoleText();
    }

    public override string DataObjectType => "Hero";

    protected override void SetProperties(Hero collectionObject, StormElement stormElement)
    {
        if (stormElement.DataValues.TryGetElementDataAt("Unit", out StormElementData? unitData))
            collectionObject.UnitId = unitData.Value.GetString();

        // first thing to do is to set the unit data
        SetUnitData(collectionObject);

        // find additional (hero) units
        SetHeroUnits(collectionObject, stormElement);

        base.SetProperties(collectionObject, stormElement);

        SetSummonedUnitIds(collectionObject);
    }

    protected override void SetAdditionalProperties(Hero collectionObject, StormElement stormElement)
    {
        StormElementData elementData = stormElement.DataValues;

        if (elementData.TryGetElementDataAt("difficulty", out StormElementData? difficultyData))
            collectionObject.Difficulty = GameStringTextService.GetGameStringTextFromId(GameStringConstants.DifficultyGameString.Replace(GameStringConstants.IdPlaceHolder, difficultyData.Value.GetString()));

        if (elementData.TryGetElementDataAt("title", out StormElementData? titleData))
            collectionObject.Title = GameStringTextService.GetGameStringTextFromId(titleData.Value.GetString());

        if (elementData.TryGetElementDataAt("melee", out StormElementData? meleeData) && meleeData.Value.TryGetInt32(out int meleeValue) && meleeValue == 1)
            collectionObject.IsMelee = true;

        SetAlternateSearchText(collectionObject, elementData);

        if (stormElement.DataValues.TryGetElementDataAt("role", out StormElementData? roleData))
        {
            GameStringText? roleTooltipDescription = GameStringTextService.GetGameStringTextFromId(GameStringConstants.HeroRoleGameString.Replace(GameStringConstants.IdPlaceHolder, roleData.Value.GetString()));
            if (roleTooltipDescription is not null)
                collectionObject.Roles.Add(roleTooltipDescription);
        }

        if (stormElement.DataValues.TryGetElementDataAt("RolesMultiClass", out StormElementData? rolesMultiClassData))
        {
            IEnumerable<string> roleDataIndexes = rolesMultiClassData.GetElementDataIndexes();

            foreach (string index in roleDataIndexes)
            {
                GameStringText? roleTooltipDescription = GameStringTextService.GetGameStringTextFromId(GameStringConstants.HeroRoleGameString.Replace(GameStringConstants.IdPlaceHolder, rolesMultiClassData.GetElementDataAt(index).Value.GetString()));
                if (roleTooltipDescription is not null)
                    collectionObject.Roles.Add(roleTooltipDescription);
            }
        }

        if (elementData.TryGetElementDataAt("ExpandedRole", out StormElementData? expandedRoleData))
            collectionObject.ExpandedRole = GameStringTextService.GetGameStringTextFromId(_roleGameStringText.Replace(GameStringConstants.IdPlaceHolder, expandedRoleData.Value.GetString()));

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
            ImageFileNamePath? imageFilePath = GetImageFilePath(selectScreenButtonImageData);
            if (imageFilePath is not null)
            {
                collectionObject.HeroPortraits.HeroSelectPortrait = imageFilePath.Image;
                collectionObject.HeroPortraits.HeroSelectPortraitPath = imageFilePath.FilePath;
            }
        }

        if (elementData.TryGetElementDataAt("ScoreScreenImage", out StormElementData? scoreScreenImageData))
        {
            ImageFileNamePath? imageFilePath = GetImageFilePath(scoreScreenImageData);
            if (imageFilePath is not null)
            {
                collectionObject.HeroPortraits.LeaderboardPortrait = imageFilePath.Image;
                collectionObject.HeroPortraits.LeaderboardPortraitPath = imageFilePath.FilePath;
            }
        }

        if (elementData.TryGetElementDataAt("LoadingScreenImage", out StormElementData? loadingScreenImageData))
        {
            ImageFileNamePath? imageFilePath = GetImageFilePath(loadingScreenImageData);
            if (imageFilePath is not null)
            {
                collectionObject.HeroPortraits.LoadingScreenPortrait = imageFilePath.Image;
                collectionObject.HeroPortraits.LoadingScreenPortraitPath = imageFilePath.FilePath;
            }
        }

        if (elementData.TryGetElementDataAt("PartyPanelButtonImage", out StormElementData? partyPanelImageData))
        {
            ImageFileNamePath? imageFilePath = GetImageFilePath(partyPanelImageData);
            if (imageFilePath is not null)
            {
                collectionObject.HeroPortraits.PartyPanelPortrait = imageFilePath.Image;
                collectionObject.HeroPortraits.PartyPanelPortraitPath = imageFilePath.FilePath;
            }
        }

        if (elementData.TryGetElementDataAt("Portrait", out StormElementData? portraitData))
        {
            ImageFileNamePath? imageFilePath = GetImageFilePath(portraitData);
            if (imageFilePath is not null)
            {
                collectionObject.HeroPortraits.TargetPortrait = imageFilePath.Image;
                collectionObject.HeroPortraits.TargetPortraitPath = imageFilePath.FilePath;
            }
        }

        if (elementData.TryGetElementDataAt("DraftScreenPortrait", out StormElementData? draftScreenPortraitData))
        {
            ImageFileNamePath? imageFilePath = GetImageFilePath(draftScreenPortraitData);
            if (imageFilePath is not null)
            {
                collectionObject.HeroPortraits.DraftScreen = imageFilePath.Image;
                collectionObject.HeroPortraits.DraftScreenPath = imageFilePath.FilePath;
            }
        }

        if (elementData.TryGetElementDataAt("PartyFrameImage", out StormElementData? partyFrameData))
        {
            ImageFileNamePath? imageFilePath = GetImageFilePath(partyFrameData);
            if (imageFilePath is not null)
            {
                collectionObject.HeroPortraits.PartyFrames.Add(imageFilePath.Image);
                collectionObject.HeroPortraits.PartyFramePaths.Add(imageFilePath.FilePath);
            }
        }

        if (elementData.TryGetElementDataAt("hdp-PartyFrameImageArray", out StormElementData? hdpPartyFrameArrayData))
        {
            IEnumerable<string> hdpParyFrameArrayIndexes = hdpPartyFrameArrayData.GetElementDataIndexes();

            foreach (string index in hdpParyFrameArrayIndexes)
            {
                ImageFileNamePath? imageFilePath = GetImageFilePath(hdpPartyFrameArrayData.GetElementDataAt(index));
                if (imageFilePath is not null)
                {
                    collectionObject.HeroPortraits.PartyFrames.Add(imageFilePath.Image);
                    collectionObject.HeroPortraits.PartyFramePaths.Add(imageFilePath.FilePath);
                }
            }
        }

        if (elementData.TryGetElementDataAt("SkinArray", out StormElementData? skinArrayData))
        {
            foreach (string item in skinArrayData.GetElementDataIndexes())
            {
                string value = skinArrayData.GetElementDataAt(item).Value.GetString();

                if (HeroesData.StormElementExists("Skin", value))
                    collectionObject.SkinIds.Add(value);
            }
        }

        if (elementData.TryGetElementDataAt("VariationArray", out StormElementData? variationArrayData))
        {
            foreach (string item in variationArrayData.GetElementDataIndexes())
            {
                string value = variationArrayData.GetElementDataAt(item).Value.GetString();

                if (HeroesData.StormElementExists("Skin", value))
                    collectionObject.VariationSkinIds.Add(value);
            }
        }

        if (elementData.TryGetElementDataAt("VoiceLineArray", out StormElementData? voiceLineArrayData))
        {
            foreach (string item in voiceLineArrayData.GetElementDataIndexes())
            {
                string value = voiceLineArrayData.GetElementDataAt(item).Value.GetString();

                if (HeroesData.StormElementExists("VoiceLine", value))
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

        if (elementData.TryGetElementDataAt("Gender", out StormElementData? genderData))
        {
            string genderValue = genderData.Value.GetString();

            if (Enum.TryParse(genderValue, out Gender genderResult))
                collectionObject.Gender = genderResult;
            else
                Logger.LogWarning("Unknown gender {Gender}", genderValue);
        }

        // must be at end
        SetTalentData(collectionObject, stormElement);
        SetSubAbilities(collectionObject);
        SetTalentUpgradeLinkIds(collectionObject);
    }

    protected override void SetValidatedProperties(Hero collectionObject)
    {
        base.SetValidatedProperties(collectionObject);

        collectionObject.Difficulty ??= GameStringTextService.GetGameStringTextFromId(GameStringConstants.DifficultyGameString.Replace(GameStringConstants.IdPlaceHolder, "Easy"));
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
            foreach (AbilityLinkId abilityLinkId in abilityLinkIds)
            {
                talent.TooltipAbilityLinkIds.Add(abilityLinkId);
            }
        }
    }

    private static void AddAbilityByTooltipTalentElementIds(Hero collectionObject, Ability ability)
    {
        foreach (string talentElementId in ability.TooltipAppendersTalentElementIds)
        {
            collectionObject.AddAbilityByTooltipTalentElementId(talentElementId, ability);
        }
    }

    private static void SetSummonedUnitIds(Hero collectionObject)
    {
        collectionObject.SummonedUnitIds = new SortedSet<string>(
            collectionObject.SummonedUnitIds
                .Concat(collectionObject.Talents
                    .SelectMany(x => x.Value
                        .SelectMany(y => y.SummonedUnitIds))),
            StringComparer.Ordinal);
    }

    private void SetAlternateSearchText(Hero collectionObject, StormElementData elementData)
    {
        if (!elementData.TryGetElementDataAt("AlternateNameSearchText", out StormElementData? alternateNameSearchTextData))
            return;

        GameStringText? alternateNameSearchText = GameStringTextService.GetGameStringTextFromId(alternateNameSearchTextData.Value.GetString());
        if (alternateNameSearchText is null)
            return;

        if (collectionObject.SearchText is not null && !string.IsNullOrWhiteSpace(collectionObject.SearchText.RawText))
        {
            collectionObject.SearchText = GameStringTextService.GetGameStringTextFromGameString($"{alternateNameSearchText.RawText} {collectionObject.SearchText.RawText}");
        }
        else
        {
            collectionObject.SearchText = alternateNameSearchText;
        }
    }

    private void SetUnitData(Hero collectionObject)
    {
        if (string.IsNullOrEmpty(collectionObject.UnitId))
        {
            Logger.LogWarning("There is no unit id for hero {Id}", collectionObject.Id);
            return;
        }

        // when calling the unitparser, we need to use the unitId for the object's id
        string heroId = collectionObject.Id;
        collectionObject.Id = collectionObject.UnitId;

        _unitParser.AllowHiddenAbilities = Options.Hidden.AllowHeroHiddenAbilities;
        _unitParser.AllowSpecialAbilities = Options.Hidden.AllowHeroSpecialAbilities;
        _unitParser.Parse(collectionObject);

        // then set it back to the heroId
        collectionObject.Id = heroId;

        // copy over the unit portraits
        collectionObject.HeroPortraits.MiniMapIcon = collectionObject.UnitPortraits.MiniMapIcon ?? string.Empty;
        collectionObject.HeroPortraits.MiniMapIconPath = collectionObject.UnitPortraits.MiniMapIconPath;
        collectionObject.HeroPortraits.TargetInfoPanel = collectionObject.UnitPortraits.TargetInfoPanel ?? string.Empty;
        collectionObject.HeroPortraits.TargetInfoPanelPath = collectionObject.UnitPortraits.TargetInfoPanelPath;
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
            IEnumerable<string> talentTreeArrayIndexes = talentTreeArray.GetElementDataIndexes();

            foreach (string talentTreeArrayIndex in talentTreeArrayIndexes)
            {
                StormElementData talentTreeArrayData = talentTreeArray.GetElementDataAt(talentTreeArrayIndex);
                Talent? talent = _talentParser.GetTalent(collectionObject, talentTreeArrayData);

                if (talent is not null)
                {
                    AddTalent(collectionObject, talent);
                }
            }
        }
    }

    private void AddTalent(Hero collectionObject, Talent talent)
    {
        collectionObject.AddTalent(talent);

        List<Ability> behaviorAbilities = _talentParser.GetBehaviorAbilitiesFromTalent(talent);

        foreach (Ability behaviorAbility in behaviorAbilities)
        {
            if (behaviorAbility.ParentTalentElementIds.Count > 0 && behaviorAbility.ParentTalentElementIds.Any(x => x.Equals(talent.LinkId.ElementId, StringComparison.Ordinal)))
                collectionObject.AssignLayoutSubAbilityToLink(behaviorAbility, talent.LinkId);
            else if (behaviorAbility.ParentTalentLinkIds.Count > 0 && behaviorAbility.ParentTalentLinkIds.Any(x => x.Equals(talent.LinkId)))
                collectionObject.AssignLayoutSubAbilityToLink(behaviorAbility, talent.LinkId);
            else
                collectionObject.AddAsUnknownSubAbility(behaviorAbility);

            AddAbilityByTooltipTalentElementIds(collectionObject, behaviorAbility);
        }
    }

    private void SetSubAbilities(Hero collectionObject)
    {
        IEnumerable<Ability> unknownSubAbilities = collectionObject.UnknownSubAbilities.SelectMany(x => x.Value);

        foreach (Ability unknownSubAbility in unknownSubAbilities)
        {
            bool result = false;
            result |= collectionObject.AddAsSubAbilityToTalent(unknownSubAbility);
            result |= collectionObject.AddAsSubAbilityToSubAbility(unknownSubAbility);

            if (!result)
            {
                if (Logger.IsEnabled(LogLevel.Warning))
                    Logger.LogWarning("Could not add unknown sub ability {AbilityId} for hero {HeroId}", unknownSubAbility.LinkId.ToString(), collectionObject.Id);

                unknownSubAbility.Tier = AbilityTier.Unknown;
                collectionObject.AddAbility(unknownSubAbility);
            }
        }
    }

    private string GetRoleText()
    {
        StormElement? stormElement = HeroesData.GetStormElement("CHeroRole");
        if (stormElement is not null && stormElement.DataValues.TryGetElementDataAt("name", out StormElementData? nameData))
        {
            return nameData.Value.GetString();
        }

        Logger.LogWarning("Could not get role game string text from CHeroRole");
        return string.Empty;
    }
}
