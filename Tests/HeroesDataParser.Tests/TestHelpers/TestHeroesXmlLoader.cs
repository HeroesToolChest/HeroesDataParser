namespace HeroesDataParser.Tests.TestHelpers;

public static class TestHeroesXmlLoader
{
    private const string _xmlTestDirectory = "TestXmlFiles";

    public static HeroesXmlLoader GetArrangedHeroesXmlLoader()
    {
        XDocument unitDocument = GetXDocument("Unit.xml");
        XDocument heroDocument = GetXDocument("Hero.xml");
        XDocument buttonDocument = GetXDocument("Button.xml");
        XDocument abilEffectTargetDocument = GetXDocument("Abil.xml");
        XDocument behaviorDocument = GetXDocument("Behavior.xml");
        XDocument actorDocument = GetXDocument("Actor.xml");
        XDocument weaponDocument = GetXDocument("Weapon.xml");
        XDocument effectDocument = GetXDocument("Effect.xml");
        XDocument skinDocument = GetXDocument("Skin.xml");
        XDocument voiceLineDocument = GetXDocument("VoiceLine.xml");
        XDocument talentDocument = GetXDocument("Talent.xml");
        XDocument validatorDocument = GetXDocument("Validator.xml");
        XDocument stormStyleDocument = GetXDocument("StormStyle.xml");

        return HeroesXmlLoader.LoadWithEmpty()
            .LoadCustomMod(new ManualModLoader("test")
                .AddBaseElementTypes([
                    ("Abil", "CAbilEffectTarget"),
                    ("Abil", "CAbilEffectInstant"),
                    ("Unit", "CUnit"),
                    ("Hero", "CHero"),
                    ("Button", "CButton"),
                    ("Actor", "CActorUnit"),
                    ("Weapon", "CWeaponLegacy"),
                    ("Effect", "CEffectDamage"),
                    ("Skin", "CSkin"),
                    ("VoiceLine", "CVoiceLine"),
                    ("Talent", "CTalent"),
                    ("Validator", "CValidatorPlayerTalent"),
                ])
                .AddElements(unitDocument.Root!.Elements())
                .AddElements(heroDocument.Root!.Elements())
                .AddElements(buttonDocument.Root!.Elements())
                .AddElements(abilEffectTargetDocument.Root!.Elements())
                .AddElements(behaviorDocument.Root!.Elements())
                .AddElements(actorDocument.Root!.Elements())
                .AddElements(weaponDocument.Root!.Elements())
                .AddElements(effectDocument.Root!.Elements())
                .AddElements(skinDocument.Root!.Elements())
                .AddElements(voiceLineDocument.Root!.Elements())
                .AddElements(talentDocument.Root!.Elements())
                .AddElements(validatorDocument.Root!.Elements())
                .AddGameStrings(
                    [
                        "test_for_tooltip_decription_service=Instantly boost an allied Hero, restoring <c val=\"#TooltipNumbers\">200~~0.045~~</c> Mana <c val=\"#TooltipNumbersNoVal\">250</c>;<s val=\"StandardTooltipDetails\">Mana: 50</s>;<s val=\"StandardTooltipDetailsNoTextColor\">Mana: 100</s>",
                        "e_gameUIStringChargeCooldownColon=Charge Cooldown: ",
                        "e_gameUIStringCooldownColon=Cooldown: ",
                        "UI/AbilTooltipCooldown=Cooldown: %1 second",
                        "UI/AbilTooltipCooldownPlural=Cooldown: %1 seconds",
                        "Abil/Name/attack=Storm Attack",
                        "Abil/Name/AbathurSymbiote=Symbiote",
                        "Abil/Name/AbathurToxicNest=Toxic Nest",
                        "Abil/AlarakDeadlyChargeButtonVitalCostOverride=60",
                        "Abil/AlarakDeadlyChargeButtonCooldownCostOverride=45 seconds",
                        "Button/Name/HearthstoneNoMana=Hearthstone",
                        "Button/Name/LootSpray=Quick Spray Expression",
                        "Button/Name/LootYellVoiceLine=Quick Voice Line Expression",
                        "Button/Name/Tease=Taunt",
                        "Button/Name/Dance=Dance",
                        "Button/Name/GenericCalldownMule=Calldown: MULE",
                        "Button/Name/AbathurSymbiote=Symbiote",
                        "Button/Name/AbathurToxicNest=Toxic Nest",
                        "Button/Name/AbathurLocustStrain=Locust Strain",
                        "Button/Name/AbathurDeepTunnel=Deep Tunnel",
                        "Button/Name/AbathurSymbiotePressurizedGlandsTalent=Pressurized Glands",
                        "Button/Name/AbathurToxicNestEnvenomedNestTalent=Envenomed Nest",
                        "Button/Name/AbathurLocustStrainSurvivalInstinctsTalent=Survival Instincts",
                        "Button/Name/AlarakDeadlyCharge=Deadly Charge",
                        "Button/Name/AlarakSadism=Sadism",
                        "Button/Name/AlexstraszaGiftOfLife=Gift of Life",
                        "Button/Name/AlexstraszaAbundance=Abundance",
                        "Button/Name/BarbarianSeismicSlam=Seismic Slam",
                        "Button/Name/GuldanLifeTap=Life Tap",
                        "Button/Name/AbathurSymbioteCancel=Cancel Symbiote",
                        "Button/Name/UseVehicle=Use Vehicle",
                        "Button/Name/SamuroIllusionMaster=Illusion Master",
                        "Button/Name/SamuroAdvancingStrikes=Image Transmission",
                        "Button/SimpleDisplayText/GenericCalldownMule=Activate to heal Structures",
                        "Button/SimpleDisplayText/AbathurSymbiote=Assist an ally and gain new abilities",
                        "Button/SimpleDisplayText/AbathurToxicNest=Spawn a mine",
                        "Button/SimpleDisplayText/AbathurLocustStrain=Spawn locusts that attack down the nearest lane",
                        "Button/SimpleDisplayText/AbathurDeepTunnel=Tunnel to a location.",
                        "Button/SimpleDisplayText/AbathurSymbiotePressurizedGlandsTalent=Increases Spike Burst range and decreases cooldown",
                        "Button/SimpleDisplayText/AbathurToxicNestEnvenomedNestTalent=Toxic Nests deal more damage, reduce Armor",
                        "Button/SimpleDisplayText/AbathurLocustStrainSurvivalInstinctsTalent=Increases Locust Health and damage",
                        "Button/SimpleDisplayText/AlarakDeadlyCharge=Channel to charge a long distance",
                        "Button/SimpleDisplayText/AlarakSadism=Each point of Sadism increases Alarak's Ability damage...",
                        "Button/SimpleDisplayText/AlexstraszaGiftOfLife=Give a portion of Health to an allied Hero",
                        "Button/SimpleDisplayText/AlexstraszaAbundance=Heal allied Heroes in an area",
                        "Button/SimpleDisplayText/GuldanLifeTap=Restore Mana at the cost of Health",
                        "Button/SimpleDisplayText/BarbarianSeismicSlam=Damage an enemy and splash damage behind them",
                        "Button/SimpleDisplayText/SamuroIllusionMasterTalent=Mirror Images can be controlled",
                        "Button/SimpleDisplayText/SamuroAdvancingStrikes=Increase Movement Speed when attacking Heroes",
                        "Button/Tooltip/SummonMount=After Channeling for 1 second, gain 30% Movement Speed",
                        "Button/Tooltip/Attack=Attacks using the Hero's weapon",
                        "Button/Tooltip/HearthstoneNoMana=After Channeling for...",
                        "Button/Tooltip/LootSpray=Express yourself to other players by marking the ground with your selected spray.",
                        "Button/Tooltip/LootYellVoiceLine=Express yourself to other players by playing your selected Voice Line.",
                        "Button/Tooltip/GenericCalldownMule=Activate to calldown a Mule that repairs Structures",
                        "Button/Tooltip/AbathurSymbiote=Spawn and attach a Symbiote...",
                        "Button/Tooltip/AbathurToxicNest=Spawn a mine that becomes active...",
                        "Button/Tooltip/AbathurLocustStrain=Spawns a Locust to attack down the nearest lane...",
                        "Button/Tooltip/AbathurDeepTunnel=Quickly tunnel to a visible location.",
                        "Button/Tooltip/AbathurSymbiotePressurizedGlandsTalent=Increases the range of Symbiote's Spike Burst by",
                        "Button/Tooltip/AbathurToxicNestEnvenomedNestTalent=Toxic Nests deal",
                        "Button/Tooltip/AbathurLocustStrainSurvivalInstinctsTalent=Increases Locust's Health by",
                        "Button/Tooltip/AlarakDeadlyCharge=After channeling, Alarak charges forward...",
                        "Button/Tooltip/AlarakSadism=Alarak's Ability damage and self-healing are increased...",
                        "Button/Tooltip/AlexstraszaGiftOfLife=Sacrifice...",
                        "Button/Tooltip/AlexstraszaAbundance=Plant a seed of healing that blooms after...",
                        "Button/Tooltip/GuldanLifeTap=Gul'dan does not regenerate Mana...",
                        "Button/Tooltip/BarbarianSeismicSlam=Deals deals damage to...",
                        "Button/Tooltip/AbathurSymbioteCancel=Cancels the Symbiote ability.",
                        "Button/Tooltip/SamuroIllusionMasterTalent=Mirror Images can be controlled individually or as...",
                        "Button/Tooltip/SamuroAdvancingStrikes=Activate to switch places with a target",
                        "Button/Tooltip/DehakaEssenceCollectionCooldownOverride=5 seconds",
                        "Button/OverrideText/DVaMechBoosters=<d ref=\"Effect,DVaBoostersApplyCooldown,Cost[0].CooldownTimeUse\"/> seconds",
                        "Hero/AdditionalSearchText/Abathur=Zerg Swarm HotS Heart of the Swarm StarCraft II 2 SC2 Star2 Starcraft2 SC slug Double Soak",
                        "Hero/AlternateNameSearchText/Abathur=Abathur",
                        "Hero/Description/Abathur=A unique Hero that can manipulate the battle from anywhere on the map.",
                        "Hero/Info/Abathur=Abathur, the Evolution Master of Kerrigan's Swarm, works ceaselessly...",
                        "Hero/Name/Abathur=Abathur",
                        "Hero/Title/Abathur=Evolution Master",
                        "UI/HeroUtil/Difficulty/Easy=Easy",
                        "UI/HeroUtil/Difficulty/Hard=Hard",
                        "UI/HeroUtil/Difficulty/Medium=Medium",
                        "UI/HeroUtil/Difficulty/VeryHard=Very Hard",
                        "UI/HeroUtil/Role/Bruiser=Bruiser",
                        "UI/HeroUtil/Role/Damage=Assassin",
                        "UI/HeroUtil/Role/Healer=Healer",
                        "UI/HeroUtil/Role/Specialist=Specialist",
                        "UI/HeroUtil/Role/MeleeAssassin=Melee Assassin",
                        "UI/HeroUtil/Role/Multiclass=Multiclass",
                        "UI/HeroUtil/Role/RangedAssassin=Ranged Assassin",
                        "UI/HeroUtil/Role/Support=Support",
                        "UI/HeroUtil/Role/Tank=Tank",
                        "UI/HeroUtil/Role/Warrior=Warrior",
                        "UI/HeroLife=Health",
                        "UI/HeroShields=Shields",
                        "UI/HeroEnergyType/Mana=Mana",
                        "UI/Tooltip/Abil/AlexstraszaGiftOfLifeVitalCostOverride=<s val=\"StandardTooltipDetails\">15%</s>",
                        "UI/Tooltip/Abil/Mana=<s val=\"StandardTooltipDetails\">Mana: %1</s>",
                        "UI/Tooltip/Abil/CurrentLife=<s val=\"StandardTooltipDetails\">Health: %1</s>",
                        "UI/Tooltip/Abil/Life=<s val=\"StandardTooltipDetails\">Life: %1</s>",
                        "UI/Tooltip/Abil/Fury=<s val=\"StandardTooltipDetails\">Fury: %1</s>",
                        "UI/Tooltip/Abil/GuldanLifeTapVitalCostOverride=<s val=\"StandardTooltipDetails\"><d ref=\"Abil,GuldanLifeTap,Cost.Vital[Life]\"/></s>",
                        "Weapon/Name/HeroAbathur=Hero Abathur",
                        "Unit/Name/HeroAbathur=Abathur",
                        "Unit/Name/AbathurSymbiote=Symbiote",
                        "SamuroIllusionMaster=8 seconds",
                        "SamuroImageTransmission=14 seconds",
                    ],
                    StormLocale.ENUS)
                .AddAssetFilePaths([
                    Path.Join("Assets", "Textures", "storm_ui_icon_miscrune_1.dds"),
                    Path.Join("Assets", "Textures", "storm_temp_war3_btnhealingspray.dds"),
                    Path.Join("Assets", "Textures", "btn-command-stop.dds"),
                    Path.Join("Assets", "Textures", "storm_btn_d3_barbarian_threateningshout.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_icon_talent_mule.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_icon_abathur_symbiote.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_icon_abathur_toxicnest.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_icon_abathur_spawnlocust.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_icon_abathur_mount.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_icon_alarak_recklesscharge.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_icon_alarak_sadism.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_icon_alexstrasza_gift_of_life.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_icon_alexstrasza_abundance.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_icon_guldan_lifetap.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_icon_sonya_seismicslam.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_minimapicon_heros_infestor.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_ingame_partyframe_abathur.dds"),
                    Path.Join("Assets", "Textures", "hud_btn_bg_ability_cancel.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_icon_talent_autoattack_base.dds"),
                    Path.Join("Assets", "Textures", "storm_temp_war3_btnloaddwarf.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_temp_icon_cheatdeath.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_icon_abathur_spikeburst.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_icon_samuro_illusiondancer.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_icon_samuro_flowingstrikes.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_icon_Muradin_SecondWind.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_ingame_heroselect_btn_infestor.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_ingame_hero_leaderboard_Abathur.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_ingame_hero_loadingscreen_Abathur.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_ingame_partypanel_btn_Abathur.dds"),
                    Path.Join("Assets", "Textures", "UI_targetportrait_Hero_Abathur.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_glues_draft_portrait_Abathur.dds"),
                    Path.Join("Assets", "Textures", "storm_ui_ingame_partyframe_Abathur.dds"),
                    ])
                .AddLevelScalingArrayElements(heroDocument.Root.Descendants("LevelScalingArray"))
                .AddStormStyleElements(stormStyleDocument.Root!.Elements()))
            .LoadGameStrings();
    }

    private static XDocument GetXDocument(string file)
    {
        return XDocument.Load(File.OpenRead(Path.Join(_xmlTestDirectory, file)));
    }
}
