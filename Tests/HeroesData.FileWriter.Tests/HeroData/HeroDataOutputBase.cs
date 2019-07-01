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
                },
                Life = new UnitLife
                {
                    LifeMax = 1900,
                    LifeScaling = 0.04,
                    LifeRegenerationRate = 3.957,
                    LifeRegenerationRateScaling = 0.04,
                },
                Energy = new UnitEnergy
                {
                    EnergyMax = 500,
                    EnergyType = "Mana",
                    EnergyRegenerationRate = 3,
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

            alarakHero.AddUnitWeapon(new UnitWeapon
            {
                WeaponNameId = "HeroWeaponAlarak",
                Range = 1.5,
                Period = 1.2,
                Damage = 140,
                DamageScaling = 0.04,
            });
            alarakHero.AddUnitWeapon(new UnitWeapon
            {
                WeaponNameId = "HeroWeaponDestructionAlarak",
                Range = 2,
                Period = 1.2,
                Damage = 340,
                DamageScaling = 0.05,
            });

            alarakHero.AddUnitArmor(new UnitArmor()
            {
                Type = "Hero",
                AbilityArmor = 5,
                BasicArmor = 10,
                SplashArmor = 15,
            });
            alarakHero.AddUnitArmor(new UnitArmor()
            {
                Type = "Merc",
                AbilityArmor = 25,
                BasicArmor = 50,
                SplashArmor = 75,
            });
            alarakHero.AddHeroDescriptor("EnergyImportant");
            alarakHero.AddHeroDescriptor("WaveClearer");
            alarakHero.AddHeroDescriptor("Overconfident");
            alarakHero.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("AlarakDiscordStrike", "AlarakDiscordStrike"),
                Name = "Discord Strike",
                IconFileName = "storm_ui_icon_alarak_discordstrike.png",
                Tier = AbilityTier.Basic,
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
                AbilityType = AbilityType.Q,
            });
            alarakHero.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("AlarakSadismDummyUI", "AlarakSadismDummyUI"),
                Name = "Sadism",
                IconFileName = "storm_ui_icon_alarak_sadism.png",
                Tier = AbilityTier.Trait,
                Tooltip = new AbilityTalentTooltip()
                {
                    ShortTooltip = new TooltipDescription("Alarak deals increased damage and has increased self-healing against enemy Heroes"),
                    FullTooltip = new TooltipDescription("Alarak's Ability damage and self-healing are increased by <c val=\"#TooltipNumbers\">100%</c> against enemy Heroes.<n/><n/><img path=\"@UI/StormTalentInTextQuestIcon\" alignment=\"uppermiddle\" color=\"B48E4C\" width=\"20\" height=\"22\"/><c val=\"#TooltipQuest\">Repeatable Quest:</c> Takedowns increase Sadism by <c val=\"#TooltipNumbers\">3%</c>, up to <c val=\"#TooltipNumbers\">30%</c>. Sadism gained from Takedowns is lost on death."),
                },
                AbilityType = AbilityType.Trait,
            });
            alarakHero.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("HeroicAbility", "HeroicAbility"),
                Name = "Heroic",
                Tier = AbilityTier.Heroic,
                AbilityType = AbilityType.Heroic,
            });
            alarakHero.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("MountAbility", "MountAbility"),
                Name = "Mount",
                Tier = AbilityTier.Mount,
                AbilityType = AbilityType.Z,
            });
            alarakHero.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("ActivableAbility", "ActivableAbility"),
                Name = "Activable",
                Tier = AbilityTier.Activable,
                AbilityType = AbilityType.Active,
            });
            alarakHero.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("HearthAbility", "HearthAbility"),
                Name = "Hearth",
                Tier = AbilityTier.Hearth,
                AbilityType = AbilityType.B,
            });
            alarakHero.AddRole("Assassin");
            alarakHero.AddRole("Warrior");
            alarakHero.AddTalent(new Talent
            {
                AbilityTalentId = new AbilityTalentId("AlarakSustainingPower", "ButtonId"),
                Name = "Sustaining Power",
                IconFileName = "storm_ui_icon_alarak_lightningsurge_a.png",
                Tooltip = new AbilityTalentTooltip()
                {
                    ShortTooltip = new TooltipDescription("Increase Lightning Surge healing"),
                    FullTooltip = new TooltipDescription("Increase the healing received from damaging Heroes with Lightning Surge by <c val=\"#TooltipNumbers\">40%</c>."),
                },
                Column = 1,
                Tier = TalentTier.Level1,
                AbilityType = AbilityType.Q,
                IsActive = true,
                IsQuest = true,
            });
            alarakHero.AddTalent(new Talent
            {
                AbilityTalentId = new AbilityTalentId("Level4Talent", "Level4Talent"),
                Name = "Level4Talent",
                Tier = TalentTier.Level4,
                Tooltip = new AbilityTalentTooltip()
                {
                    FullTooltip = new TooltipDescription("Burrow to the target location, dealing <c val=\"#TooltipNumbers\">96~~0.04~~</c> damage and briefly stunning enemies in a small area upon surfacing, slowing them by <c val=\"#TooltipNumbers\">25%</c> for <c val=\"#TooltipNumbers\">2.5</c> seconds.<n/><n/>Burrow Charge can be reactivated to surface early."),
                },
                AbilityType = AbilityType.Q,
            });
            alarakHero.AddTalent(new Talent
            {
                AbilityTalentId = new AbilityTalentId("Level7Talent", "Level7Talent"),
                Name = "Level4Talent",
                Tier = TalentTier.Level7,
                AbilityType = AbilityType.Q,
            });
            alarakHero.AddTalent(new Talent
            {
                AbilityTalentId = new AbilityTalentId("AlarakHeroicAbilityDeadlyCharge", "AlarakHeroicAbilityDeadlyCharge"),
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
                Tier = TalentTier.Level10,
                AbilityType = AbilityType.Heroic,
            });
            alarakHero.AddTalent(new Talent
            {
                AbilityTalentId = new AbilityTalentId("AlarakHeroicAbilityCounterStrike", "AlarakCounterStrikeTargeted"),
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
                Tier = TalentTier.Level10,
                AbilityType = AbilityType.Heroic,
            });
            alarakHero.AddTalent(new Talent
            {
                AbilityTalentId = new AbilityTalentId("Leve13Talent", "Leve13Talent"),
                Name = "Leve13Talent",
                Tier = TalentTier.Level13,
                AbilityType = AbilityType.W,
            });
            alarakHero.AddTalent(new Talent
            {
                AbilityTalentId = new AbilityTalentId("Level16Talent", "Level16Talent"),
                Name = "Level16Talent",
                Tier = TalentTier.Level16,
                AbilityType = AbilityType.W,
            });
            alarakHero.AddTalent(new Talent
            {
                AbilityTalentId = new AbilityTalentId("Level20Talent", "Level20Talent"),
                Name = "Level20Talent",
                Tier = TalentTier.Level20,
                AbilityType = AbilityType.W,
            });

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
                AbilityTalentId = new AbilityTalentId("TychusOdinAnnihilate", "TychusOdinAnnihilate"),
                Name = "Annihilate",
                IconFileName = "storm_ui_icon_tychus_annihilate.png",
                Tier = AbilityTier.Basic,
                Tooltip = new AbilityTalentTooltip()
                {
                    FullTooltip = new TooltipDescription("Burrow to the target location, dealing <c val=\"#TooltipNumbers\">96~~0.04~~</c> damage and briefly stunning enemies in a small area upon surfacing, slowing them by <c val=\"#TooltipNumbers\">25%</c> for <c val=\"#TooltipNumbers\">2.5</c> seconds.<n/><n/>Burrow Charge can be reactivated to surface early."),
                },
                AbilityType = AbilityType.Q,
            });
            alexstraszaHero.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("SubAbilHeroic", "SubAbilHeroic"),
                Name = "SubAbilHeroic",
                Tier = AbilityTier.Heroic,

                AbilityType = AbilityType.Heroic,
            });
            alexstraszaHero.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("SubAbilMount", "SubAbilMount"),
                Name = "SubAbilMount",
                Tier = AbilityTier.Mount,
                AbilityType = AbilityType.Z,
            });
            alexstraszaHero.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("SubAbilTrait", "SubAbilTrait"),
                Name = "SubAbilTrait",
                Tier = AbilityTier.Trait,
                AbilityType = AbilityType.Trait,
            });
            alexstraszaHero.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("SubAbilActivable", "SubAbilActivable"),
                Name = "SubAbilActivable",
                Tier = AbilityTier.Activable,
                AbilityType = AbilityType.Active,
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
            };

            alexHeroUnit.AddUnitArmor(new UnitArmor
            {
                Type = "Minion",
                AbilityArmor = 50,
            });
            alexHeroUnit.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("DragonAbilHeroic", "DragonAbilHeroic"),
                Name = "DragonAbilHeroic",
                Tier = AbilityTier.Heroic,
                ParentLink = new AbilityTalentId("SubAbilHeroic", "SubAbilHeroic"),
                AbilityType = AbilityType.Heroic,
            });
            alexHeroUnit.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("DragonAbilMount", "DragonAbilMount"),
                Name = "DragonAbilMount",
                Tier = AbilityTier.Mount,
                ParentLink = new AbilityTalentId("SubAbilMount", "SubAbilMount"),
                AbilityType = AbilityType.Z,
            });
            alexHeroUnit.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("DragonAbilTrait", "DragonAbilTrait"),
                Name = "DragonAbilTrait",
                Tier = AbilityTier.Trait,
                Tooltip = new AbilityTalentTooltip()
                {
                    FullTooltip = new TooltipDescription("Burrow to the target location, dealing <c val=\"#TooltipNumbers\">96~~0.04~~</c> damage and briefly stunning enemies in a small area upon surfacing, slowing them by <c val=\"#TooltipNumbers\">25%</c> for <c val=\"#TooltipNumbers\">2.5</c> seconds.<n/><n/>Burrow Charge can be reactivated to surface early."),
                },
                AbilityType = AbilityType.Trait,
            });
            alexHeroUnit.AddAbility(new Ability
            {
                AbilityTalentId = new AbilityTalentId("DragonAbilActivable", "DragonAbilActivable"),
                Name = "DragonAbilActivable",
                Tier = AbilityTier.Activable,
                AbilityType = AbilityType.Trait,
            });

            alexstraszaHero.AddHeroUnit(alexHeroUnit);

            TestData.Add(alexstraszaHero);
        }
    }
}
