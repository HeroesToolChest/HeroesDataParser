namespace HeroesDataParser.Infrastructure.XmlDataParsers.Tests;

[TestClass]
public class HeroParserTests
{
    private readonly ILogger<HeroParser> _logger;
    private readonly ILogger<TooltipDescriptionService> _tooltipLogger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IUnitParser _unitParser;
    private readonly ITalentParser _talentParser;
    private readonly ITooltipDescriptionService _tooltipDescriptionService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public HeroParserTests()
    {
        _logger = Substitute.For<ILogger<HeroParser>>();
        _tooltipLogger = Substitute.For<ILogger<TooltipDescriptionService>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _unitParser = Substitute.For<IUnitParser>();
        _talentParser = Substitute.For<ITalentParser>();
        _tooltipDescriptionService = Substitute.For<ITooltipDescriptionService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
        _options.Value.Returns(new RootOptions()
        {
            DescriptionText = new DescriptionTextOptions()
            {
                Type = DescriptionType.RawDescription,
            },
        });

        // allow the real instance
        _tooltipDescriptionService = new TooltipDescriptionService(_tooltipLogger, _options, _heroesXmlLoaderService);
    }

    [TestMethod]
    public void Parse_UnitParsingForHeroUnit_UnitParsed()
    {
        // arrange
        string heroUnit = "HpTESTHero";
        string unitId = "HeroHpTESTHero";

        HeroParser heroParser = new(_logger, _options, _heroesXmlLoaderService, _unitParser, _talentParser, _tooltipDescriptionService);

        Hero hero = new(heroUnit)
        {
            UnitId = unitId,
        };

        // act
        _ = heroParser.Parse(hero.Id);

        // assert
        _unitParser.Received().Parse(hero);
    }

    [TestMethod]
    public void Parse_CustomPartyFrameImageArray_ReceivesImageFilePath()
    {
        // arrange
        string heroUnit = "HpTESTHero";

        HeroParser heroParser = new(_logger, _options, _heroesXmlLoaderService, _unitParser, _talentParser, _tooltipDescriptionService);

        // act
        Hero? hero = heroParser.Parse(heroUnit);

        // assert
        hero!.HeroPortraits.PartyFrames.Should().HaveCount(4)
            .And.ContainInConsecutiveOrder("test-default.png", "test-image1.png", "test-image2.png", "test-image3.png");
    }

    [TestMethod]
    public void Parse_HiddenOptions_OptionsReceived()
    {
        // arrange
        string heroUnit = "HpTESTHero";
        string unitId = "HeroHpTESTHero";

        _options.Value.Returns(new RootOptions()
        {
            Hidden = new HiddenOptions()
            {
                AllowHeroHiddenAbilities = true,
                AllowHeroSpecialAbilities = true,
            },
        });

        HeroParser heroParser = new(_logger, _options, _heroesXmlLoaderService, _unitParser, _talentParser, _tooltipDescriptionService);

        Hero hero = new(heroUnit)
        {
            UnitId = unitId,
        };

        // act
        _ = heroParser.Parse(hero.Id);

        // assert
        _unitParser.AllowHiddenAbilities.Should().BeTrue();
        _unitParser.AllowSpecialAbilities.Should().BeTrue();
    }

    [TestMethod]
    public void Parse_AbathurDataHeroDataOnly_ReturnHeroData()
    {
        // arrange
        string heroUnit = "Abathur";

        HeroParser heroParser = new(_logger, _options, _heroesXmlLoaderService, _unitParser, _talentParser, _tooltipDescriptionService);

        _unitParser.Parse("AbathurSymbiote").Returns(new Unit("AbathurSymbiote"));

        // act
        Hero? hero = heroParser.Parse(heroUnit);

        // assert
        hero.Should().NotBeNull();
        hero.Name!.RawDescription.Should().Be("Abathur");
        hero.SortName.Should().BeNull();
        hero.HyperlinkId.Should().Be("Abathur");
        hero.AttributeId.Should().Be("Abat");
        hero.Franchise.Should().Be(Franchise.Starcraft);
        hero.Rarity.Should().Be(Rarity.Legendary);
        hero.ReleaseDate.Should().Be(new DateOnly(2014, 3, 13));
        hero.Category.Should().BeNull();
        hero.Event.Should().BeNull();
        hero.SearchText!.RawDescription.Should().Be("Abathur Zerg Swarm HotS Heart of the Swarm StarCraft II 2 SC2 Star2 Starcraft2 SC slug Double Soak");
        hero.Description!.RawDescription.Should().Be("A unique Hero that can manipulate the battle from anywhere on the map.");
        hero.UnitId.Should().Be("HeroAbathur");
        hero.Title!.RawDescription.Should().Be("Evolution Master");
        hero.Difficulty!.RawDescription.Should().Be("Very Hard");
        hero.IsMelee.Should().BeTrue();
        hero.DefaultMountId.Should().BeNull();
        hero.Roles.Should().ContainSingle().And.Contain(new TooltipDescription("Specialist"));
        hero.ExpandedRole!.RawDescription.Should().Be("Support");
        hero.Ratings.Damage.Should().Be(3);
        hero.Ratings.Complexity.Should().Be(9);
        hero.Ratings.Survivability.Should().Be(1);
        hero.Ratings.Utility.Should().Be(7);
        hero.SkinIds.Should().HaveCount(4).And
            .SatisfyRespectively(
                first => first.Should().Be("AbathurMecha"),
                second => second.Should().Be("AbathurPajamathur"),
                third => third.Should().Be("AbathurSkelethur"),
                fourth => fourth.Should().Be("AbathurUltimate"));
        hero.VariationSkinIds.Should().HaveCount(3).And
            .SatisfyRespectively(
                first => first.Should().Be("AbathurBaseVar3"),
                second => second.Should().Be("AbathurBone"),
                third => third.Should().Be("AbathurChar"));
        hero.VoiceLineIds.Should().HaveCount(5).And
            .SatisfyRespectively(
                first => first.Should().Be("AbathurBase_VoiceLine01"),
                second => second.Should().Be("AbathurBase_VoiceLine02"),
                third => third.Should().Be("AbathurBase_VoiceLine03"),
                fourth => fourth.Should().Be("AbathurBase_VoiceLine04"),
                fifth => fifth.Should().Be("AbathurBase_VoiceLine05"));
        hero.MountCategoryIds.Should().BeEmpty();
        hero.InfoText!.RawDescription.Should().Be("Abathur, the Evolution Master of Kerrigan's Swarm, works ceaselessly...");
        hero.HeroUnits.Should().ContainSingle();
        hero.HeroPortraits.DraftScreen.Should().Be("storm_ui_glues_draft_portrait_abathur.png");
        hero.HeroPortraits.DraftScreenPath!.FilePath.Should().StartWith("Assets").And.EndWith("storm_ui_glues_draft_portrait_Abathur.dds");
        hero.HeroPortraits.HeroSelectPortrait.Should().Be("storm_ui_ingame_heroselect_btn_infestor.png");
        hero.HeroPortraits.HeroSelectPortraitPath!.FilePath.Should().StartWith("Assets").And.EndWith("storm_ui_ingame_heroselect_btn_infestor.dds");
        hero.HeroPortraits.LeaderboardPortrait.Should().Be("storm_ui_ingame_hero_leaderboard_abathur.png");
        hero.HeroPortraits.LeaderboardPortraitPath!.FilePath.Should().StartWith("Assets").And.EndWith("storm_ui_ingame_hero_leaderboard_Abathur.dds");
        hero.HeroPortraits.LoadingScreenPortrait.Should().Be("storm_ui_ingame_hero_loadingscreen_abathur.png");
        hero.HeroPortraits.LoadingScreenPortraitPath!.FilePath.Should().StartWith("Assets").And.EndWith("storm_ui_ingame_hero_loadingscreen_Abathur.dds");
        hero.HeroPortraits.PartyPanelPortrait.Should().Be("storm_ui_ingame_partypanel_btn_abathur.png");
        hero.HeroPortraits.PartyPanelPortraitPath!.FilePath.Should().StartWith("Assets").And.EndWith("storm_ui_ingame_partypanel_btn_Abathur.dds");
        hero.HeroPortraits.TargetPortrait.Should().Be("ui_targetportrait_hero_abathur.png");
        hero.HeroPortraits.TargetPortraitPath!.FilePath.Should().StartWith("Assets").And.EndWith("UI_targetportrait_Hero_Abathur.dds");
        hero.HeroPortraits.PartyFrames.Should().ContainSingle().And
            .Contain("storm_ui_ingame_partyframe_abathur.png");
        hero.HeroPortraits.PartyFramePaths.Should().ContainSingle();
        hero.HeroPortraits.PartyFramePaths[0].FilePath.Should().StartWith("Assets").And.EndWith("storm_ui_ingame_partyframe_Abathur.dds");
        hero.HeroPortraits.MiniMapIcon.Should().BeEmpty();
        hero.HeroPortraits.TargetInfoPanel.Should().BeEmpty();
    }

    [TestMethod]
    public void Parse_BarbarianDataHeroDataOnly_ReturnHeroData()
    {
        // arrange
        string heroUnit = "Barbarian";

        HeroParser heroParser = new(_logger, _options, _heroesXmlLoaderService, _unitParser, _talentParser, _tooltipDescriptionService);

        // act
        Hero? hero = heroParser.Parse(heroUnit);

        // assert
        hero.Should().NotBeNull();
        hero.Gender.Should().Be(Gender.Female);
    }

    [TestMethod]
    public void Parse_WithUnitData_ReturnsHeroData()
    {
        // arrange
        string heroUnit = "Abathur";

        HeroParser heroParser = new(_logger, _options, _heroesXmlLoaderService, _unitParser, _talentParser, _tooltipDescriptionService);

        // unit part for abathur hero
        _unitParser.When(x => x.Parse(Arg.Any<Unit>()))
            .Do(x =>
            {
                Unit argUnit = x.Arg<Unit>();
                argUnit.Id = "HeroAbathur";
                argUnit.Description = null;
                argUnit.UnitPortraits.MiniMapIcon = "minimap_icon.png";
                argUnit.UnitPortraits.MiniMapIconPath = new RelativeFilePath()
                {
                    FilePath = Path.Join("Assets", "minimap_icon.dds"),
                };
                argUnit.UnitPortraits.TargetInfoPanel = "target_info_panel.png";
                argUnit.UnitPortraits.TargetInfoPanelPath = new RelativeFilePath()
                {
                    FilePath = Path.Join("Assets", "target_info_panel.dds"),
                };
            });

        // act
        Hero? hero = heroParser.Parse(heroUnit);

        // assert
        hero.Should().NotBeNull();
        hero.Description!.RawDescription.Should().Be("A unique Hero that can manipulate the battle from anywhere on the map.");
        hero.HeroPortraits.MiniMapIcon.Should().Be("minimap_icon.png");
        hero.HeroPortraits.MiniMapIconPath!.FilePath.Should().StartWith("Assets").And.EndWith("minimap_icon.dds");
        hero.HeroPortraits.TargetInfoPanel.Should().Be("target_info_panel.png");
        hero.HeroPortraits.TargetInfoPanelPath!.FilePath.Should().StartWith("Assets").And.EndWith("target_info_panel.dds");
    }

    [TestMethod]
    public void Parse_AbathurSettingTalentUpgradeLinks_TalentUpgradeLinksAreSet()
    {
        // arrange
        string heroUnit = "Abathur";

        HeroParser heroParser = new(_logger, _options, _heroesXmlLoaderService, _unitParser, _talentParser, _tooltipDescriptionService);

        Ability abathurSymbioteAbility = new()
        {
            AbilityElementId = "AbathurSymbiote",
            ButtonElementId = "AbathurSymbiote",
            Tier = AbilityTier.Basic,
            AbilityType = AbilityType.Q,
        };
        abathurSymbioteAbility.TooltipAppendersTalentElementIds.Add("AbathurMasteryPressurizedGlands");

        Ability envenomedNestAbility = new()
        {
            AbilityElementId = "AbathurToxicNest",
            ButtonElementId = "AbathurToxicNest",
            Tier = AbilityTier.Basic,
            AbilityType = AbilityType.W,
        };
        abathurSymbioteAbility.TooltipAppendersTalentElementIds.Add("AbathurMasteryEnvenomedNestsToxicNest");

        // unit part for abathur hero
        _unitParser.When(x => x.Parse(Arg.Any<Unit>()))
            .Do(x =>
            {
                Unit argUnit = x.Arg<Unit>();
                argUnit.Id = "HeroAbathur";
                argUnit.AddAbility(abathurSymbioteAbility);
                argUnit.AddAbility(envenomedNestAbility);
                argUnit.AddAbilityByTooltipTalentElementId("AbathurMasteryPressurizedGlands", abathurSymbioteAbility);
                argUnit.AddAbilityByTooltipTalentElementId("AbathurMasteryEnvenomedNestsToxicNest", envenomedNestAbility);
            });

        _talentParser.GetTalent(Arg.Any<Hero>(), Arg.Is<StormElementData>(x => x.Field == "TalentTreeArray[0]")).Returns(new Talent()
        {
            TalentElementId = "AbathurMasteryPressurizedGlands",
            ButtonElementId = "AbathurSymbiotePressurizedGlandsTalent",
            Tier = TalentTier.Level1,
            AbilityType = AbilityType.W,
        });
        _talentParser.GetTalent(Arg.Any<Hero>(), Arg.Is<StormElementData>(x => x.Field == "TalentTreeArray[1]")).Returns(new Talent()
        {
            TalentElementId = "AbathurMasteryEnvenomedNestsToxicNest",
            ButtonElementId = "AbathurToxicNestEnvenomedNestTalent",
            Tier = TalentTier.Level1,
            AbilityType = AbilityType.W,
        });

        // hero unit Symbiote
        Unit abathurSymbioteUnit = new("AbathurSymbiote");

        Ability abathurSymbioteSpikeBurstAbility = new()
        {
            AbilityElementId = "AbathurSymbioteSpikeBurst",
            ButtonElementId = "AbathurSymbioteSpikeBurst",
            Tier = AbilityTier.Basic,
            AbilityType = AbilityType.W,
        };
        abathurSymbioteAbility.TooltipAppendersTalentElementIds.Add("AbathurMasteryPressurizedGlands");

        abathurSymbioteUnit.AddAbility(abathurSymbioteSpikeBurstAbility);
        abathurSymbioteUnit.AddAbilityByTooltipTalentElementId("AbathurMasteryPressurizedGlands", abathurSymbioteSpikeBurstAbility);

        _unitParser.Parse("AbathurSymbiote").Returns(abathurSymbioteUnit);

        // act
        Hero? hero = heroParser.Parse(heroUnit);

        // assert
        hero.Should().NotBeNull();
        hero.HeroUnits.Should().ContainSingle();
        hero.Talents[TalentTier.Level1].Should().HaveCount(2);
        hero.Talents[TalentTier.Level1][0].LinkId.ToString().Should().Be("AbathurMasteryPressurizedGlands|AbathurSymbiotePressurizedGlandsTalent|W");
        hero.Talents[TalentTier.Level1][0].UpgradeLinkIds.Should()
            .ContainInConsecutiveOrder([new LinkId("AbathurSymbiote", "AbathurSymbiote", AbilityType.Q), new LinkId("AbathurSymbioteSpikeBurst", "AbathurSymbioteSpikeBurst", AbilityType.W)]);
        hero.Talents[TalentTier.Level1][1].LinkId.ToString().Should().Be("AbathurMasteryEnvenomedNestsToxicNest|AbathurToxicNestEnvenomedNestTalent|W");
        hero.Talents[TalentTier.Level1][1].UpgradeLinkIds.Should()
            .ContainInConsecutiveOrder([new LinkId("AbathurToxicNest", "AbathurToxicNest", AbilityType.W)]);
    }

    [TestMethod]
    public void Parse_HasSubAbilitiesWithTalentParents_ReturnsSubAbilities()
    {
        // arrange
        string heroUnit = "Alarak";

        HeroParser heroParser = new(_logger, _options, _heroesXmlLoaderService, _unitParser, _talentParser, _tooltipDescriptionService);

        // unit part for alarak hero
        _unitParser.When(x => x.Parse(Arg.Any<Unit>()))
            .Do(x =>
            {
                Unit argUnit = x.Arg<Unit>();
                argUnit.Id = "HeroAlarak";
                argUnit.AddAbility(new Ability()
                {
                    AbilityElementId = "someability",
                    ButtonElementId = "someability",
                    Tier = AbilityTier.Trait,
                    AbilityType = AbilityType.Trait,
                });
                argUnit.AddSubAbility(new Ability()
                {
                    AbilityElementId = "AlarakDeadlyChargeExecute2ndHeroic",
                    ButtonElementId = "AlarakUnleashDeadlyCharge",
                    Tier = AbilityTier.Trait,
                    AbilityType = AbilityType.Trait,
                    ParentAbilityElementId = "AlarakDeadlyChargeActivate2ndHeroic",
                });
                argUnit.AddSubAbility(new Ability()
                {
                    AbilityElementId = "AlarakDeadlyChargeActivate2ndHeroic",
                    ButtonElementId = "AlarakDeadlyCharge2ndHeroicSadism",
                    Tier = AbilityTier.Trait,
                    AbilityType = AbilityType.Trait,
                    ParentAbilityElementId = "AlarakDeadlyChargeSecondHeroic",
                });
            });

        _talentParser.GetTalent(Arg.Any<Hero>(), Arg.Is<StormElementData>(x => x.Field == "TalentTreeArray[0]")).Returns(new Talent()
        {
            TalentElementId = "AlarakDeadlyChargeSecondHeroic",
            ButtonElementId = "AlarakDeadlyCharge",
            Tier = TalentTier.Level20,
            AbilityType = AbilityType.Heroic,
        });

        // act
        Hero? hero = heroParser.Parse(heroUnit);

        // assert
        hero.Should().NotBeNull();
        hero.SubAbilities.Should().HaveCount(2)
            .And.ContainKeys(
                new LinkId("AlarakDeadlyChargeActivate2ndHeroic", "AlarakDeadlyCharge2ndHeroicSadism", AbilityType.Trait),
                new LinkId("AlarakDeadlyChargeSecondHeroic", "AlarakDeadlyCharge", AbilityType.Heroic));
    }

    [TestMethod]
    public void Parse_AlarakShowofForceTalentUpgradeLinkIds_TalentUpgradeLinksAreSet()
    {
        // arrange
        string heroUnit = "Alarak";

        HeroParser heroParser = new(_logger, _options, _heroesXmlLoaderService, _unitParser, _talentParser, _tooltipDescriptionService);

        Ability sadismAbility = new()
        {
            AbilityElementId = AbilityTalentParserBase.PassiveAbilityElementId,
            ButtonElementId = "AlarakSadism",
            Tier = AbilityTier.Trait,
            AbilityType = AbilityType.Trait,
            TooltipAppendersTalentElementIds =
            {
                "AlarakShowofForce",
            },
        };

        Ability alarakCounterStrikeTargeted2ndHeroicAbility = new()
        {
            AbilityElementId = "AlarakCounterStrikeTargeted2ndHeroic",
            ButtonElementId = "AlarakCounterStrike2ndHeroicSadism",
            Tier = AbilityTier.Trait,
            AbilityType = AbilityType.Trait,
            TooltipAppendersTalentElementIds =
            {
                "AlarakShowofForce",
            },
        };

        Ability alarakDeadlyChargeActivate2ndHeroicmAbility = new()
        {
            AbilityElementId = "AlarakDeadlyChargeActivate2ndHeroic",
            ButtonElementId = "AlarakDeadlyCharge2ndHeroicSadism",
            Tier = AbilityTier.Trait,
            AbilityType = AbilityType.Trait,
            TooltipAppendersTalentElementIds =
            {
                "AlarakShowofForce",
            },
        };

        // unit part for alark hero
        _unitParser.When(x => x.Parse(Arg.Any<Unit>()))
            .Do(x =>
            {
                Unit argUnit = x.Arg<Unit>();
                argUnit.Id = "HeroAlarak";
                argUnit.AddAbility(sadismAbility);
                argUnit.AddAbility(alarakCounterStrikeTargeted2ndHeroicAbility);
                argUnit.AddAbility(alarakDeadlyChargeActivate2ndHeroicmAbility);
                argUnit.AddAbilityByTooltipTalentElementId("AlarakShowofForce", sadismAbility);
                argUnit.AddAbilityByTooltipTalentElementId("AlarakShowofForce", alarakCounterStrikeTargeted2ndHeroicAbility);
                argUnit.AddAbilityByTooltipTalentElementId("AlarakShowofForce", alarakDeadlyChargeActivate2ndHeroicmAbility);
            });

        _talentParser.GetTalent(Arg.Any<Hero>(), Arg.Is<StormElementData>(x => x.Field == "TalentTreeArray[5]")).Returns(new Talent()
        {
            TalentElementId = "AlarakShowofForce",
            ButtonElementId = "AlarakShowofForce",
            Tier = TalentTier.Level4,
            AbilityType = AbilityType.Trait,
        });

        // act
        Hero? hero = heroParser.Parse(heroUnit);

        // assert
        hero.Should().NotBeNull();
        hero.Talents[TalentTier.Level4].Should().ContainSingle();
        hero.Talents[TalentTier.Level4][0].UpgradeLinkIds.Should()
            .ContainInConsecutiveOrder(
                [
                    new LinkId(":PASSIVE:", "AlarakSadism", AbilityType.Trait),
                    new LinkId("AlarakCounterStrikeTargeted2ndHeroic", "AlarakCounterStrike2ndHeroicSadism", AbilityType.Trait),
                    new LinkId("AlarakDeadlyChargeActivate2ndHeroic", "AlarakDeadlyCharge2ndHeroicSadism", AbilityType.Trait),
                ]);
    }

    [TestMethod]
    public void Parse_TalentUpgradeLinksShouldOnlyBeAddedIfAbilityorSubAbilityAvailable_ReturnsCorrectUpgradeTalentLinks()
    {
        // arrange
        string heroUnit = "Deathwing";

        HeroParser heroParser = new(_logger, _options, _heroesXmlLoaderService, _unitParser, _talentParser, _tooltipDescriptionService);

        Ability deathwingIncinerateAbility = new()
        {
            AbilityElementId = "DeathwingIncinerate",
            ButtonElementId = "DeathwingIncinerate",
            AbilityType = AbilityType.W,
            Tier = AbilityTier.Basic,
            TooltipAppendersTalentElementIds =
            {
                "DeathwingDragonSoul",
            },
        };

        Ability deathwingLavaBurstAbility = new()
        {
            AbilityElementId = "DeathwingLavaBurst",
            ButtonElementId = "DeathwingLavaBurst",
            AbilityType = AbilityType.W,
            Tier = AbilityTier.Basic,
            ParentAbilityElementId = "DeathwingFormSwitch",
            TooltipAppendersTalentElementIds =
            {
                "DeathwingDragonSoul",
            },
        };

        _unitParser.When(x => x.Parse(Arg.Any<Unit>()))
            .Do(x =>
            {
                Unit argUnit = x.Arg<Unit>();
                argUnit.Id = "HeroDeathwing";
                argUnit.AddAbility(deathwingIncinerateAbility);
                argUnit.AddSubAbility(deathwingLavaBurstAbility);
                argUnit.AddAbilityByTooltipTalentElementId("DeathwingDragonSoul", deathwingIncinerateAbility);
                argUnit.AddAbilityByTooltipTalentElementId("DeathwingDragonSoul", deathwingLavaBurstAbility);
            });

        _talentParser.GetTalent(Arg.Any<Hero>(), Arg.Is<StormElementData>(x => x.Field == "TalentTreeArray[0]")).Returns(new Talent()
        {
            TalentElementId = "DeathwingDragonSoul",
            ButtonElementId = "DeathwingDragonSoul",
            Tier = TalentTier.Level1,
            AbilityType = AbilityType.W,
        });

        // act
        Hero? hero = heroParser.Parse(heroUnit);

        // assert
        hero.Should().NotBeNull();
        hero.Talents[TalentTier.Level1][0].UpgradeLinkIds.Should().HaveCount(1);
        hero.Talents[TalentTier.Level1][0].UpgradeLinkIds.First().ToString().Should().Be("DeathwingIncinerate|DeathwingIncinerate|W");
    }

    [TestMethod]
    public void Parse_GuldanSubAbilitiesToTalents_ReturnsTalentUpgradeLinks()
    {
        // arrange
        string heroUnit = "Guldan";

        HeroParser heroParser = new(_logger, _options, _heroesXmlLoaderService, _unitParser, _talentParser, _tooltipDescriptionService);

        Ability guldanLifeTapAbility = new()
        {
            AbilityElementId = "GuldanLifeTap",
            ButtonElementId = "GuldanLifeTap",
            AbilityType = AbilityType.Trait,
            Tier = AbilityTier.Trait,
            TooltipAppendersTalentElementIds =
            {
                "GuldanLifeTapImprovedLifeTap",
                "GuldanLifeTapDarknessWithin",
            },
        };

        Ability guldanLifeTapFreeSubAbility = new()
        {
            AbilityElementId = "GuldanLifeTapFree",
            ButtonElementId = "GuldanLifeTapFree",
            AbilityType = AbilityType.Trait,
            Tier = AbilityTier.Trait,
            TooltipAppendersTalentElementIds =
            {
                "GuldanLifeTapImprovedLifeTap",
                "GuldanLifeTapDarknessWithin",
            },
            ParentAbilityElementId = "GuldanLifeTapDarknessWithin",
        };

        _unitParser.When(x => x.Parse(Arg.Any<Unit>()))
            .Do(x =>
            {
                Unit argUnit = x.Arg<Unit>();
                argUnit.Id = "HeroGuldan";
                argUnit.AddAbility(guldanLifeTapAbility);
                argUnit.AddSubAbility(guldanLifeTapFreeSubAbility);
                argUnit.AddAbilityByTooltipTalentElementId("GuldanLifeTapImprovedLifeTap", guldanLifeTapAbility);
                argUnit.AddAbilityByTooltipTalentElementId("GuldanLifeTapDarknessWithin", guldanLifeTapAbility);
                argUnit.AddAbilityByTooltipTalentElementId("GuldanLifeTapImprovedLifeTap", guldanLifeTapFreeSubAbility);
                argUnit.AddAbilityByTooltipTalentElementId("GuldanLifeTapDarknessWithin", guldanLifeTapFreeSubAbility);
            });

        _talentParser.GetTalent(Arg.Any<Hero>(), Arg.Is<StormElementData>(x => x.Field == "TalentTreeArray[4]")).Returns(new Talent()
        {
            TalentElementId = "GuldanLifeTapImprovedLifeTap",
            ButtonElementId = "GuldanLifeTapImprovedLifeTap",
            Tier = TalentTier.Level4,
            AbilityType = AbilityType.Trait,
        });
        _talentParser.GetTalent(Arg.Any<Hero>(), Arg.Is<StormElementData>(x => x.Field == "TalentTreeArray[17]")).Returns(new Talent()
        {
            TalentElementId = "GuldanLifeTapDarknessWithin",
            ButtonElementId = "GuldanLifeTapDarknessWithin",
            Tier = TalentTier.Level16,
            AbilityType = AbilityType.Trait,
        });

        // act
        Hero? hero = heroParser.Parse(heroUnit);

        // assert
        hero.Should().NotBeNull();
        hero.Talents[TalentTier.Level4][0].UpgradeLinkIds.Should().HaveCount(2);
        hero.Talents[TalentTier.Level4][0].UpgradeLinkIds.Should().ContainInConsecutiveOrder(new LinkId("GuldanLifeTap", "GuldanLifeTap", AbilityType.Trait), new LinkId("GuldanLifeTapFree", "GuldanLifeTapFree", AbilityType.Trait));
        hero.Talents[TalentTier.Level16][0].UpgradeLinkIds.Should().HaveCount(2);
        hero.Talents[TalentTier.Level16][0].UpgradeLinkIds.Should().ContainInConsecutiveOrder(new LinkId("GuldanLifeTap", "GuldanLifeTap", AbilityType.Trait), new LinkId("GuldanLifeTapFree", "GuldanLifeTapFree", AbilityType.Trait));
    }
}