using HeroesData.Parser.Models;
using HeroesData.Parser.Models.AbilityTalents;
using HeroesData.Parser.Models.AbilityTalents.Tooltip;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace HeroesData.FileWriter.Tests
{
    public class FileOutputTests
    {
        private readonly FileOutput FileOutput;

        private List<Hero> Heroes = new List<Hero>();

        public FileOutputTests()
        {
            SetHeroData();
            FileOutput = FileOutput.SetHeroData(Heroes);
        }

        [Fact]
        public void JsonWriterTest()
        {
            FileOutput.CreateJson();

            List<string> outputJson = new List<string>();
            List<string> outputJsonTest = new List<string>();

            using (StreamReader reader = new StreamReader(Path.Combine("output", "json", "heroesdata.json")))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    outputJson.Add(line);
                }
            }

            using (StreamReader reader = new StreamReader("JsonOutputTest.json"))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    outputJsonTest.Add(line);
                }
            }

            Assert.Equal(outputJsonTest.Count, outputJson.Count);

            if (outputJsonTest.Count == outputJson.Count)
            {
                for (int i = 0; i < outputJsonTest.Count; i++)
                {
                    Assert.Equal(outputJsonTest[i], outputJson[i]);
                }
            }
        }

        private void SetHeroData()
        {
            Hero hero = new Hero
            {
                ShortName = "Alarak",
                Name = "Alarak",
                CHeroId = "HeroAlarak",
                CUnitId = "Alar",
                Difficulty = HeroDifficulty.Hard,
                Franchise = HeroFranchise.Starcraft,
                Gender = HeroGender.Male,
                Radius = 0.875,
                ReleaseDate = new DateTime(2016, 9, 13),
                Sight = 12,
                Speed = 4.3984,
                Type = UnitType.Melee,
                Rarity = HeroRarity.Legendary,
                Description = new TooltipDescription("A combo Assassin that can move enemies around and punish mistakes."),
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
                    EnergyType = UnitEnergyType.Mana,
                    EnergyRegenerationRate = 3,
                },
                Roles = new List<HeroRole> { HeroRole.Assassin },
                Ratings = new HeroRatings()
                {
                    Complexity = 8,
                    Damage = 7,
                    Survivability = 6,
                    Utility = 7,
                },
                Weapons = new List<UnitWeapon>
                {
                    new UnitWeapon
                    {
                        WeaponNameId = "HeroWeaponAlarak",
                        Range = 1.5,
                        Period = 1.2,
                        Damage = 140,
                        DamageScaling = 0.04,
                    },
                },
                Abilities = new Dictionary<string, Ability>
                {
                    {
                        "AlarakDiscordStrike",
                        new Ability
                        {
                            ReferenceNameId = "AlarakDiscordStrike",
                            Name = "Discord Strike",
                            ShortTooltipNameId = "AlarakDiscordStrike",
                            FullTooltipNameId = "AlarakDiscordStrike",
                            IconFileName = "storm_ui_icon_alarak_discordstrike.png",
                            Tier = AbilityTier.Basic,
                            Tooltip = new AbilityTalentTooltip()
                            {
                                Energy = new TooltipEnergy
                                {
                                    EnergyCost = 45,
                                },
                                Cooldown = new TooltipCooldown()
                                {
                                    CooldownValue = 8,
                                },
                                ShortTooltip = new TooltipDescription("Damage and silence enemies in an area"),
                                FullTooltip = new TooltipDescription("After a <c val=\"#TooltipNumbers\">0.5</c> second delay, enemies in front of Alarak take <c val=\"#TooltipNumbers\">175</c> damage and are silenced for <c val=\"#TooltipNumbers\">1.5</c> seconds."),
                            },
                        }
                    },
                    {
                        "AlarakSadismDummyUI",
                        new Ability
                        {
                            ReferenceNameId = "AlarakSadismDummyUI",
                            Name = "Sadism",
                            ShortTooltipNameId = "AlarakSadismDummyUI",
                            FullTooltipNameId = "AlarakSadismDummyUI",
                            IconFileName = "storm_ui_icon_alarak_sadism.png",
                            Tier = AbilityTier.Trait,
                            Tooltip = new AbilityTalentTooltip()
                            {
                                ShortTooltip = new TooltipDescription("Alarak deals increased damage and has increased self-healing against enemy Heroes"),
                                FullTooltip = new TooltipDescription("Alarak's Ability damage and self-healing are increased by <c val=\"#TooltipNumbers\">100%</c> against enemy Heroes.<n/><n/><img path=\"@UI/StormTalentInTextQuestIcon\" alignment=\"uppermiddle\" color=\"B48E4C\" width=\"20\" height=\"22\"/><c val=\"#TooltipQuest\">Repeatable Quest:</c> Takedowns increase Sadism by <c val=\"#TooltipNumbers\">3%</c>, up to <c val=\"#TooltipNumbers\">30%</c>. Sadism gained from Takedowns is lost on death."),
                            },
                        }
                    },
                },
                Talents = new Dictionary<string, Talent>
                {
                    {
                        "AlarakSustainingPower",
                        new Talent
                        {
                            ReferenceNameId = "AlarakSustainingPower",
                            Name = "Sustaining Power",
                            ShortTooltipNameId = "AlarakSustainingPower",
                            FullTooltipNameId = "AlarakSustainingPower",
                            IconFileName = "storm_ui_icon_alarak_lightningsurge_a.png",
                            Tooltip = new AbilityTalentTooltip()
                            {
                                ShortTooltip = new TooltipDescription("Increase Lightning Surge healing"),
                                FullTooltip = new TooltipDescription("Increase the healing received from damaging Heroes with Lightning Surge by <c val=\"#TooltipNumbers\">40%</c>."),
                            },
                            Column = 1,
                            Tier = TalentTier.Level1,
                        }
                    },
                    {
                        "AlarakExtendedLightning",
                        new Talent
                        {
                            ReferenceNameId = "AlarakExtendedLightning",
                            Name = "Extended Lightning",
                            ShortTooltipNameId = "AlarakExtendedLightning",
                            FullTooltipNameId = "AlarakExtendedLightning",
                            IconFileName = "storm_ui_icon_alarak_lightningsurge.png",
                            Tooltip = new AbilityTalentTooltip()
                            {
                                ShortTooltip = new TooltipDescription("<c val=\"#TooltipQuest\">Quest:</c> Reduce Sadism, empower Lightning Surge"),
                                FullTooltip = new TooltipDescription("Reduce Sadism by <c val=\"#TooltipNumbers\">10%</c>.<n/><n/><img path=\"@UI/StormTalentInTextQuestIcon\" alignment=\"uppermiddle\" color=\"B48E4C\" width=\"20\" height=\"22\"/><c val=\"#TooltipQuest\">Quest:</c> Hit Heroes with the center of Lightning Surge.<n/><n/><img path=\"@UI/StormTalentInTextQuestIcon\" alignment=\"uppermiddle\" color=\"B48E4C\" width=\"20\" height=\"22\"/><c val=\"#TooltipQuest\">Reward:</c> After hitting <c val=\"#TooltipNumbers\">5</c> Heroes, increase Lightning Surge's range by <c val=\"#TooltipNumbers\">20%</c>.<n/><n/><img path=\"@UI/StormTalentInTextQuestIcon\" alignment=\"uppermiddle\" color=\"B48E4C\" width=\"20\" height=\"22\"/><c val=\"#TooltipQuest\">Reward:</c> After hitting <c val=\"#TooltipNumbers\">15</c> Heroes, Lightning Surge's center also Slows enemies by <c val=\"#TooltipNumbers\">40%</c> for <c val=\"#TooltipNumbers\">2</c> seconds.<n/><n/><img path=\"@UI/StormTalentInTextQuestIcon\" alignment=\"uppermiddle\" color=\"B48E4C\" width=\"20\" height=\"22\"/><c val=\"#TooltipQuest\">Reward:</c> After hitting <c val=\"#TooltipNumbers\">3</c> Heroes with the center of a single cast, increase Sadism by <c val=\"#TooltipNumbers\">10%</c> and instantly gain all other Rewards."),
                            },
                            Column = 2,
                            Tier = TalentTier.Level1,
                        }
                    },
                    {
                        "AlarakHeroicAbilityDeadlyCharge",
                        new Talent
                        {
                            ReferenceNameId = "AlarakHeroicAbilityDeadlyCharge",
                            Name = "Deadly Charge",
                            ShortTooltipNameId = "AlarakDeadlyCharge",
                            FullTooltipNameId = "AlarakDeadlyCharge",
                            IconFileName = "storm_ui_icon_alarak_recklesscharge.png",
                            Tooltip = new AbilityTalentTooltip()
                            {
                                Cooldown = new TooltipCooldown()
                                {
                                    CooldownValue = 45,
                                },
                                Energy = new TooltipEnergy()
                                {
                                    EnergyCost = 60,
                                },
                                ShortTooltip = new TooltipDescription("Channel to charge a long distance"),
                                FullTooltip = new TooltipDescription("After channeling, Alarak charges forward dealing <c val=\"#TooltipNumbers\">200</c> damage to all enemies in his path. Distance is increased based on the amount of time channeled, up to <c val=\"#TooltipNumbers\">1.6</c> seconds.<n/><n/>Issuing a Move order while this is channeling will cancel it at no cost. Taking damage will interrupt the channeling."),
                            },
                            Column = 1,
                            Tier = TalentTier.Level10,
                        }
                    },
                    {
                        "AlarakHeroicAbilityCounterStrike",
                        new Talent
                        {
                            ReferenceNameId = "AlarakHeroicAbilityCounterStrike",
                            Name = "Counter-Strike",
                            ShortTooltipNameId = "AlarakCounterStrikeTargeted",
                            FullTooltipNameId = "AlarakCounterStrikeTargeted",
                            IconFileName = "storm_ui_icon_alarak_counterstrike.png",
                            Tooltip = new AbilityTalentTooltip()
                            {
                                Energy = new TooltipEnergy()
                                {
                                    EnergyCost = 50,
                                },
                                Cooldown = new TooltipCooldown()
                                {
                                    CooldownValue = 30,
                                },
                                ShortTooltip = new TooltipDescription("Prevents damage to deal damage in a large area"),
                                FullTooltip = new TooltipDescription("Alarak targets an area and channels for <c val=\"#TooltipNumbers\">1</c> second, becoming Protected and Unstoppable. After, if he took damage from an enemy Hero, he sends a shockwave that deals <c val=\"#TooltipNumbers\">275</c> damage."),
                            },
                            Column = 2,
                            Tier = TalentTier.Level10,
                        }
                    },
                },
            };

            Heroes.Add(hero);
        }
    }
}
