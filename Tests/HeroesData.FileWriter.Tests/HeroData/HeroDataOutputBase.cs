using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Heroes.Models.AbilityTalents.Tooltip;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace HeroesData.FileWriter.Tests.HeroData
{
    public class HeroDataOutputBase : FileOutputTestBase<Hero>
    {
        public HeroDataOutputBase()
            : base(nameof(HeroData))
        {
        }

        public virtual void WriterFileSplitNoBuildNumberTest()
        {
            FileOutputOptions options = new FileOutputOptions()
            {
                IsFileSplit = true,
                DescriptionType = DescriptionType.RawDescription,
            };

            FileOutput fileOutput = new FileOutput(options);
            fileOutput.Create(TestData, FileOutputType);

            string directory = GetSplitFilePath(null, false);
            Assert.IsTrue(Directory.Exists(directory));

            CompareFile(Path.Combine(directory, $"alarak.{FileOutputTypeFileName}"), $"alarak.{FileOutputTypeFileName}");
            CompareFile(Path.Combine(directory, $"alexstrasza.{FileOutputTypeFileName}"), $"alexstrasza.{FileOutputTypeFileName}");
        }

        public virtual void WriterFileSplitHasBuildNumberTest()
        {
            FileOutputOptions options = new FileOutputOptions()
            {
                IsFileSplit = true,
                DescriptionType = DescriptionType.RawDescription,
            };

            FileOutput fileOutput = new FileOutput(BuildNumber, options);
            fileOutput.Create(TestData, FileOutputType);

            string directory = GetSplitFilePath(BuildNumber, false);
            Assert.IsTrue(Directory.Exists(directory));

            CompareFile(Path.Combine(directory, $"alarak.{FileOutputTypeFileName}"), $"alarak.{FileOutputTypeFileName}");
            CompareFile(Path.Combine(directory, $"alexstrasza.{FileOutputTypeFileName}"), $"alexstrasza.{FileOutputTypeFileName}");
        }

        public virtual void WriterFileSplitMinifiedHasBuildNumberTest()
        {
            FileOutputOptions options = new FileOutputOptions()
            {
                IsFileSplit = true,
                IsMinifiedFiles = true,
            };

            FileOutput fileOutput = new FileOutput(SplitMinifiedBuildNumber, options);
            fileOutput.Create(TestData, FileOutputType);

            string directory = GetSplitFilePath(SplitMinifiedBuildNumber, true);
            Assert.IsTrue(Directory.Exists(directory));

            Assert.IsTrue(File.Exists(Path.Combine(directory, $"alarak.min.{FileOutputTypeFileName}")));
            Assert.IsTrue(File.Exists(Path.Combine(directory, $"alexstrasza.min.{FileOutputTypeFileName}")));
        }

        protected override void SetTestData()
        {
            Hero alarakHero = new Hero
            {
                Id = "Alarak",
                HyperlinkId = "AlarakId",
                Name = "Alarak",
                CHeroId = "HeroAlarak",
                CUnitId = "Alar",
                Difficulty = "Hard",
                Franchise = HeroFranchise.Starcraft,
                Gender = UnitGender.Male,
                Title = "Highlord",
                Radius = 0.875,
                ReleaseDate = new DateTime(2016, 9, 13),
                Sight = 12,
                Speed = 4.3984,
                Type = "Melee",
                Rarity = Rarity.Legendary,
                Description = new TooltipDescription("A Tank who specializes against Mages thanks in part to his innate Spell Armor.<n/><n/><img path=\"@UI / StormTalentInTextArmorIcon\" alignment=\"uppermiddle\" color=\"e12bfc\" width=\"20\" height=\"22\"/><c val=\"#TooltipNumbers\">20 Spell Armor</c>"),
                SearchText = "Alarak highlord protoss",
                HeroPortrait = new HeroPortrait()
                {
                    HeroSelectPortraitFileName = "storm_ui_ingame_heroselect_btn_alarak.png",
                    LeaderboardPortraitFileName = "storm_ui_ingame_hero_leaderboard_alarak.png",
                    LoadingScreenPortraitFileName = "storm_ui_ingame_hero_loadingscreen_alarak.png",
                    PartyPanelPortraitFileName = "storm_ui_ingame_partypanel_btn_alarak.png",
                    TargetPortraitFileName = "ui_targetportrait_hero_alarak.png",
                    DraftScreenFileName = "draftscreen.png",
                },
                UnitPortrait = new UnitPortrait()
                {
                    MiniMapIconFileName = "minimap.png",
                    TargetInfoPanelFileName = "targetInfo.png",
                },
                Life = new UnitLife
                {
                    LifeMax = 1900,
                    LifeScaling = 0.04,
                    LifeRegenerationRate = 3.957,
                    LifeRegenerationRateScaling = 0.04,
                    LifeType = "Life",
                },
                Energy = new UnitEnergy
                {
                    EnergyMax = 500,
                    EnergyType = "Mana",
                    EnergyRegenerationRate = 3,
                },
                Shield = new UnitShield
                {
                    ShieldMax = 300,
                    ShieldRegenerationDelay = 4.5,
                    ShieldRegenerationRate = 10,
                    ShieldRegenerationRateScaling = 0.04,
                    ShieldScaling = 0.04,
                    ShieldType = "Shield",
                },
                ExpandedRole = "Melee Assassin",
                Ratings = new HeroRatings()
                {
                    Complexity = 8,
                    Damage = 7,
                    Survivability = 6,
                    Utility = 7,
                },
            };

            alarakHero.HeroPortrait.PartyFrameFileName.Add("partyframe.png");
            alarakHero.HeroPortrait.PartyFrameFileName.Add("partyFrame2.png");

            alarakHero.Weapons.Add(new UnitWeapon
            {
                WeaponNameId = "HeroWeaponAlarak",
                Range = 1.5,
                Period = 1.2,
                Damage = 140,
                DamageScaling = 0.04,
            });
            alarakHero.Weapons.Add(new UnitWeapon
            {
                WeaponNameId = "HeroWeaponDestructionAlarak",
                Range = 2,
                Period = 1.2,
                Damage = 340,
                DamageScaling = 0.05,
            });

            alarakHero.Armor.Add(new UnitArmor()
            {
                Type = "Hero",
                AbilityArmor = 5,
                BasicArmor = 10,
                SplashArmor = 15,
            });
            alarakHero.Armor.Add(new UnitArmor()
            {
                Type = "Merc",
                AbilityArmor = 25,
                BasicArmor = 50,
                SplashArmor = 75,
            });
            alarakHero.HeroDescriptors.Add("EnergyImportant");
            alarakHero.HeroDescriptors.Add("WaveClearer");
            alarakHero.HeroDescriptors.Add("Overconfident");
            alarakHero.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("AlarakDiscordStrike", "AlarakDiscordStrike")
                {
                    AbilityType = AbilityTypes.Q,
                },
                Name = "Discord Strike",
                IconFileName = "storm_ui_icon_alarak_discordstrike.png",
                Tier = AbilityTiers.Basic,
                Tooltip = new AbilityTalentTooltip()
                {
                    Energy = new TooltipEnergy
                    {
                        EnergyTooltip = new TooltipDescription("45"),
                    },
                    Cooldown = new TooltipCooldown()
                    {
                        ToggleCooldown = 2.5,
                        CooldownTooltip = new TooltipDescription("8 seconds"),
                    },
                    ShortTooltip = new TooltipDescription("Damage and silence enemies in an area"),
                    FullTooltip = new TooltipDescription("After a <c val=\"#TooltipNumbers\">0.5</c> second delay, enemies in front of Alarak take <c val=\"#TooltipNumbers\">175</c> damage and are silenced for <c val=\"#TooltipNumbers\">1.5</c> seconds."),
                },
            });
            alarakHero.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("AlarakSadismDummyUI", "AlarakSadismDummyUI")
                {
                    AbilityType = AbilityTypes.Trait,
                },
                Name = "Sadism",
                IconFileName = "storm_ui_icon_alarak_sadism.png",
                Tier = AbilityTiers.Trait,
                Tooltip = new AbilityTalentTooltip()
                {
                    ShortTooltip = new TooltipDescription("Alarak deals increased damage and has increased self-healing against enemy Heroes"),
                    FullTooltip = new TooltipDescription("Alarak's Ability damage and self-healing are increased by <c val=\"#TooltipNumbers\">100%</c> against enemy Heroes.<n/><n/><img path=\"@UI/StormTalentInTextQuestIcon\" alignment=\"uppermiddle\" color=\"B48E4C\" width=\"20\" height=\"22\"/><c val=\"#TooltipQuest\">Repeatable Quest:</c> Takedowns increase Sadism by <c val=\"#TooltipNumbers\">3%</c>, up to <c val=\"#TooltipNumbers\">30%</c>. Sadism gained from Takedowns is lost on death."),
                },
                IsActive = false,
            });
            alarakHero.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("HeroicAbility", "HeroicAbility")
                {
                    AbilityType = AbilityTypes.Heroic,
                },
                Name = "Heroic",
                Tier = AbilityTiers.Heroic,
            });
            alarakHero.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("MountAbility", "MountAbility")
                {
                    AbilityType = AbilityTypes.Z,
                },
                Name = "Mount",
                Tier = AbilityTiers.Mount,
            });
            alarakHero.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("ActivableAbility", "ActivableAbility")
                {
                    AbilityType = AbilityTypes.Active,
                },
                Name = "Activable",
                Tier = AbilityTiers.Activable,
            });
            alarakHero.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("HearthAbility", "HearthAbility")
                {
                    AbilityType = AbilityTypes.B,
                },
                Name = "Hearth",
                Tier = AbilityTiers.Hearth,
            });
            alarakHero.Roles.Add("Assassin");
            alarakHero.Roles.Add("Warrior");
            alarakHero.AddTalent(new Talent
            {
                AbilityTalentId = new AbilityTalentId("AlarakSustainingPower", "ButtonId")
                {
                    AbilityType = AbilityTypes.Q,
                },
                Name = "Sustaining Power",
                IconFileName = "storm_ui_icon_alarak_lightningsurge_a.png",
                Tooltip = new AbilityTalentTooltip()
                {
                    ShortTooltip = new TooltipDescription("Increase Lightning Surge healing"),
                    FullTooltip = new TooltipDescription("Increase the healing received from damaging Heroes with Lightning Surge by <c val=\"#TooltipNumbers\">40%</c>."),
                },
                Column = 1,
                Tier = TalentTiers.Level1,
                IsActive = true,
                IsQuest = true,
            });
            alarakHero.AddTalent(new Talent
            {
                AbilityTalentId = new AbilityTalentId("Level4Talent", "Level4Talent")
                {
                    AbilityType = AbilityTypes.Q,
                },
                Name = "Level4Talent",
                Tier = TalentTiers.Level4,
                Tooltip = new AbilityTalentTooltip()
                {
                    FullTooltip = new TooltipDescription("Burrow to the target location, dealing <c val=\"#TooltipNumbers\">96~~0.04~~</c> damage and briefly stunning enemies in a small area upon surfacing, slowing them by <c val=\"#TooltipNumbers\">25%</c> for <c val=\"#TooltipNumbers\">2.5</c> seconds.<n/><n/>Burrow Charge can be reactivated to surface early."),
                },
            });
            alarakHero.AddTalent(new Talent
            {
                AbilityTalentId = new AbilityTalentId("Level7Talent", "Level7Talent")
                {
                    AbilityType = AbilityTypes.Q,
                },
                Name = "Level4Talent",
                Tier = TalentTiers.Level7,
            });
            alarakHero.AddTalent(new Talent
            {
                AbilityTalentId = new AbilityTalentId("AlarakHeroicAbilityDeadlyCharge", "AlarakHeroicAbilityDeadlyCharge")
                {
                    AbilityType = AbilityTypes.Heroic,
                },
                Name = "Deadly Charge",
                IconFileName = "storm_ui_icon_alarak_recklesscharge.png",
                Tooltip = new AbilityTalentTooltip()
                {
                    Cooldown = new TooltipCooldown()
                    {
                        CooldownTooltip = new TooltipDescription("45"),
                    },
                    Energy = new TooltipEnergy()
                    {
                        EnergyTooltip = new TooltipDescription("8"),
                    },
                    ShortTooltip = new TooltipDescription("Channel to charge a long distance"),
                    FullTooltip = new TooltipDescription("After channeling, Alarak charges forward dealing <c val=\"#TooltipNumbers\">200</c> damage to all enemies in his path. Distance is increased based on the amount of time channeled, up to <c val=\"#TooltipNumbers\">1.6</c> seconds.<n/><n/>Issuing a Move order while this is channeling will cancel it at no cost. Taking damage will interrupt the channeling."),
                },
                Column = 1,
                Tier = TalentTiers.Level10,
            });
            alarakHero.AddTalent(new Talent
            {
                AbilityTalentId = new AbilityTalentId("AlarakHeroicAbilityCounterStrike", "AlarakCounterStrikeTargeted")
                {
                    AbilityType = AbilityTypes.Heroic,
                },
                Name = "Counter-Strike",
                IconFileName = "storm_ui_icon_alarak_counterstrike.png",
                Tooltip = new AbilityTalentTooltip()
                {
                    Energy = new TooltipEnergy()
                    {
                        EnergyTooltip = new TooltipDescription("50"),
                    },
                    Cooldown = new TooltipCooldown()
                    {
                        CooldownTooltip = new TooltipDescription("30 seconds"),
                    },
                    ShortTooltip = new TooltipDescription("Prevents damage to deal damage in a large area"),
                    FullTooltip = new TooltipDescription("Alarak targets an area and channels for <c val=\"#TooltipNumbers\">1</c> second, becoming Protected and Unstoppable. After, if he took damage from an enemy Hero, he sends a shockwave that deals <c val=\"#TooltipNumbers\">275</c> damage."),
                },
                Column = 2,
                Tier = TalentTiers.Level10,
            });
            alarakHero.AddTalent(new Talent
            {
                AbilityTalentId = new AbilityTalentId("Leve13Talent", "Leve13Talent")
                {
                    AbilityType = AbilityTypes.W,
                },
                Name = "Leve13Talent",
                Tier = TalentTiers.Level13,
            });
            alarakHero.AddTalent(new Talent
            {
                AbilityTalentId = new AbilityTalentId("Level16Talent", "Level16Talent")
                {
                    AbilityType = AbilityTypes.W,
                },
                Name = "Level16Talent",
                Tier = TalentTiers.Level16,
            });

            Talent level20Talent = new Talent
            {
                AbilityTalentId = new AbilityTalentId("Level20Talent", "Level20Talent")
                {
                    AbilityType = AbilityTypes.W,
                },
                Name = "Level20Talent",
                Tier = TalentTiers.Level20,
            };

            level20Talent.PrerequisiteTalentIds.Add("AlarakSustainingPower");
            level20Talent.PrerequisiteTalentIds.Add("Level4Talent");
            alarakHero.AddTalent(level20Talent);

            TestData.Add(alarakHero);

            Hero alexstraszaHero = new Hero
            {
                Id = "Alexstrasza",
                HyperlinkId = "AlexstraszaId",
                Name = "Alexstrasza",
                CHeroId = "Alexstrasza",
                CUnitId = "HeroAlexstrasza",
                AttributeId = "Alex",
                Difficulty = "Medium",
                Franchise = HeroFranchise.Warcraft,
                Gender = UnitGender.Female,
                InnerRadius = 0.75,
                Radius = 0.75,
                ReleaseDate = new DateTime(2017, 11, 14),
                Sight = 12,
                Speed = 4.3984,
                Type = "Ranged",
                Rarity = Rarity.Legendary,
                Description = new TooltipDescription("A Healer who shares her Health with allies and can transform into a Dragon to empower her Abilities."),
                Life = new UnitLife
                {
                    LifeMax = -1,
                    LifeScaling = 0.04,
                    LifeRegenerationRate = 3.957,
                    LifeRegenerationRateScaling = 0.04,
                },
                Energy = new UnitEnergy
                {
                    EnergyMax = -1,
                    EnergyType = "Mana",
                    EnergyRegenerationRate = 3,
                },
            };
            alexstraszaHero.AddAbility(new Ability()
            {
                AbilityTalentId = new AbilityTalentId("TychusOdinAnnihilate", "TychusOdinAnnihilate")
                {
                    AbilityType = AbilityTypes.Q,
                },
                Name = "Annihilate",
                IconFileName = "storm_ui_icon_tychus_annihilate.png",
                Tier = AbilityTiers.Basic,
                Tooltip = new AbilityTalentTooltip()
                {
                    FullTooltip = new TooltipDescription("Burrow to the target location, dealing <c val=\"#TooltipNumbers\">96~~0.04~~</c> damage and briefly stunning enemies in a small area upon surfacing, slowing them by <c val=\"#TooltipNumbers\">25%</c> for <c val=\"#TooltipNumbers\">2.5</c> seconds.<n/><n/>Burrow Charge can be reactivated to surface early."),
                },
            });
            alexstraszaHero.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("SubAbilHeroic", "SubAbilHeroic")
                {
                    AbilityType = AbilityTypes.Heroic,
                },
                Name = "SubAbilHeroic",
                Tier = AbilityTiers.Heroic,
            });
            alexstraszaHero.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("SubAbilMount", "SubAbilMount")
                {
                    AbilityType = AbilityTypes.Z,
                },
                Name = "SubAbilMount",
                Tier = AbilityTiers.Mount,
            });
            alexstraszaHero.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("SubAbilTrait", "SubAbilTrait")
                {
                    AbilityType = AbilityTypes.Trait,
                },
                Name = "SubAbilTrait",
                Tier = AbilityTiers.Trait,
            });
            alexstraszaHero.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("SubAbilActivable", "SubAbilActivable")
                {
                    AbilityType = AbilityTypes.Active,
                },
                Name = "SubAbilActivable",
                Tier = AbilityTiers.Activable,
            });

            Hero alexHeroUnit = new Hero
            {
                Id = "AlexstraszaDragon",
                HyperlinkId = "AlexstraszaDragonId",
                Name = "Alexstrasza",
                CUnitId = "HeroAlexstraszaDragon",
                InnerRadius = 1,
                Radius = 1.25,
                Sight = 12,
                Speed = 4.3984,
                Life = new UnitLife
                {
                    LifeMax = 1787,
                    LifeScaling = 0.04,
                    LifeRegenerationRate = 3.7226,
                    LifeRegenerationRateScaling = 0.04,
                },
                UnitPortrait = new UnitPortrait()
                {
                    MiniMapIconFileName = "minimap.png",
                    TargetInfoPanelFileName = "targetInfo.png",
                },
            };

            alexHeroUnit.Armor.Add(new UnitArmor
            {
                Type = "Minion",
                AbilityArmor = 50,
            });
            alexHeroUnit.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("DragonAbilHeroic", "DragonAbilHeroic")
                {
                    AbilityType = AbilityTypes.Heroic,
                },
                Name = "DragonAbilHeroic",
                Tier = AbilityTiers.Heroic,
                ParentLink = new AbilityTalentId("SubAbilHeroic", "SubAbilHeroic")
                {
                    AbilityType = AbilityTypes.Heroic,
                },
            });
            alexHeroUnit.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("DragonAbilMount", "DragonAbilMount")
                {
                    AbilityType = AbilityTypes.Z,
                },
                Name = "DragonAbilMount",
                Tier = AbilityTiers.Mount,
                ParentLink = new AbilityTalentId("SubAbilMount", "SubAbilMount")
                {
                    AbilityType = AbilityTypes.Z,
                },
            });
            alexHeroUnit.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("DragonAbilTrait", "DragonAbilTrait")
                {
                    AbilityType = AbilityTypes.Trait,
                },
                Name = "DragonAbilTrait",
                Tier = AbilityTiers.Trait,
                Tooltip = new AbilityTalentTooltip()
                {
                    FullTooltip = new TooltipDescription("Burrow to the target location, dealing <c val=\"#TooltipNumbers\">96~~0.04~~</c> damage and briefly stunning enemies in a small area upon surfacing, slowing them by <c val=\"#TooltipNumbers\">25%</c> for <c val=\"#TooltipNumbers\">2.5</c> seconds.<n/><n/>Burrow Charge can be reactivated to surface early."),
                },
            });
            alexHeroUnit.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("DragonAbilActivable", "DragonAbilActivable")
                {
                    AbilityType = AbilityTypes.Trait,
                },
                Name = "DragonAbilActivable",
                Tier = AbilityTiers.Activable,
            });

            alexstraszaHero.HeroUnits.Add(alexHeroUnit);

            TestData.Add(alexstraszaHero);
        }
    }
}
