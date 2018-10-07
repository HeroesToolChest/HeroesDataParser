using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Xunit;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    public class FalstadDataTests : HeroDataBaseTest
    {
        [Fact]
        public void BasicAbilitiesTests()
        {
            Ability ability = HeroFalstad.Abilities["FalstadHammerang"];
            Assert.Equal(AbilityTier.Basic, ability.Tier);
            Assert.Equal("FalstadHammerang", ability.ReferenceNameId);
            Assert.Equal("Hammerang", ability.Name);
            Assert.Equal("FalstadHammerang", ability.ShortTooltipNameId);
            Assert.Equal("FalstadHammerang", ability.FullTooltipNameId);
            Assert.Equal("storm_ui_icon_falstad_hammerang.dds", ability.IconFileName);

            Assert.Equal("<s val=\"StandardTooltipDetails\">Mana: 70</s>", ability.Tooltip.Energy.EnergyTooltip?.RawDescription);
            Assert.Equal("Cooldown: 10 seconds", ability.Tooltip.Cooldown.CooldownTooltip?.RawDescription);
            Assert.True(string.IsNullOrEmpty(ability.Tooltip.Life.LifeCostTooltip?.RawDescription));
            Assert.Null(ability.Tooltip.Charges.CountMax);

            Assert.Equal("Throw out a Hammer that returns. Hammer slows and damages enemies", ability.Tooltip.ShortTooltip.RawDescription);
            Assert.Equal("Throw out a Hammer that returns to Falstad, dealing <c val=\"#TooltipNumbers\">121~~0.04~~</c> damage and slowing enemies by <c val=\"#TooltipNumbers\">25%</c> for <c val=\"#TooltipNumbers\">2</c> seconds.", ability.Tooltip.FullTooltip.RawDescription);
        }

        [Fact]
        public void BasicDataTests()
        {
            Assert.Equal("Falstad", HeroFalstad.Name);
            Assert.Equal("Falstad", HeroFalstad.CHeroId);
            Assert.Equal("HeroFalstad", HeroFalstad.CUnitId);
            Assert.Equal("Fals", HeroFalstad.AttributeId);
            Assert.Equal("Medium", HeroFalstad.Difficulty);
            Assert.Equal(HeroFranchise.Warcraft, HeroFalstad.Franchise);
            Assert.Equal(HeroGender.Male, HeroFalstad.Gender);
            Assert.Equal(0.9375, HeroFalstad.InnerRadius);
            Assert.Equal(0.9375, HeroFalstad.Radius);
            Assert.Equal("2014-03-13", HeroFalstad.ReleaseDate.Value.ToString("yyyy-MM-dd"));
            Assert.Equal(12.0, HeroFalstad.Sight);
            Assert.Equal(4.3984, HeroFalstad.Speed);
            Assert.Equal("Ranged", HeroFalstad.Type);
            Assert.Equal(HeroRarity.Epic, HeroFalstad.Rarity);
            Assert.Equal("An Assassin that can fly large distances, excelling on large battlegrounds.", HeroFalstad.Description.RawDescription);
        }

        [Fact]
        public void EnergyTests()
        {
            Assert.Equal(500, HeroFalstad.Energy.EnergyMax);
            Assert.Equal("Mana", HeroFalstad.Energy.EnergyType);
            Assert.Equal(3.0, HeroFalstad.Energy.EnergyRegenerationRate);
        }

        [Fact]
        public void HeroicAbilitiesTests()
        {
            Ability ability = HeroFalstad.Abilities["FalstadHinterlandBlast"];
            Assert.Equal(AbilityTier.Heroic, ability.Tier);
            Assert.Equal("FalstadHinterlandBlast", ability.ReferenceNameId);
            Assert.Equal("Hinterland Blast", ability.Name);
            Assert.Equal("FalstadHinterlandBlast", ability.ShortTooltipNameId);
            Assert.Equal("FalstadHinterlandBlast", ability.FullTooltipNameId);
            Assert.Equal("storm_ui_icon_falstad_hinterlandblast.dds", ability.IconFileName);

            Assert.Equal("<s val=\"StandardTooltipDetails\">Mana: 100</s>", ability.Tooltip.Energy.EnergyTooltip?.RawDescription);
            Assert.Equal("Mana: 100", ability.Tooltip.Energy.EnergyTooltip?.PlainText);
            Assert.Equal("Cooldown: 120 seconds", ability.Tooltip.Cooldown.CooldownTooltip?.RawDescription);
            Assert.True(string.IsNullOrEmpty(ability.Tooltip.Life.LifeCostTooltip?.RawDescription));
            Assert.Null(ability.Tooltip.Charges.CountMax);

            Assert.Equal("Long range damage beam", ability.Tooltip.ShortTooltip.RawDescription);
            Assert.Equal("After <c val=\"#TooltipNumbers\">1</c> second, deal <c val=\"#TooltipNumbers\">475~~0.0475~~</c> damage to enemies within a long line. The cooldown is reduced by <c val=\"#TooltipNumbers\">25</c> seconds for every enemy Hero hit.", ability.Tooltip.FullTooltip.RawDescription);
        }

        [Fact]
        public void LifeTests()
        {
            Assert.Equal(1365.0, HeroFalstad.Life.LifeMax);
            Assert.Equal(0.04, HeroFalstad.Life.LifeScaling);
            Assert.Equal(2.8437, HeroFalstad.Life.LifeRegenerationRate);
            Assert.Equal(0.04, HeroFalstad.Life.LifeRegenerationRateScaling);
        }

        [Fact]
        public void MountAbilitiesTests()
        {
            Ability ability = HeroFalstad.Abilities["FalstadFlight"];
            Assert.Equal(AbilityTier.Mount, ability.Tier);
            Assert.Equal("FalstadFlight", ability.ReferenceNameId);
            Assert.Equal("Flight", ability.Name);
            Assert.Equal("FalstadFlight", ability.ShortTooltipNameId);
            Assert.Equal("FalstadFlight", ability.FullTooltipNameId);
            Assert.Equal("storm_ui_icon_falstad_mount.dds", ability.IconFileName);

            Assert.True(string.IsNullOrEmpty(ability.Tooltip.Energy.EnergyTooltip?.RawDescription));
            Assert.Equal("Cooldown: 55 seconds", ability.Tooltip.Cooldown.CooldownTooltip?.RawDescription);
            Assert.Null(ability.Tooltip.Charges.CountMax);

            Assert.Equal("Instead of mounting, Falstad can fly a great distance over terrain", ability.Tooltip.ShortTooltip.RawDescription);
            Assert.Equal("Instead of mounting, Falstad can fly a great distance over terrain.", ability.Tooltip.FullTooltip.RawDescription);
        }

        [Fact]
        public void PortraitsTests()
        {
            Assert.Equal("storm_ui_ingame_heroselect_btn_gryphon_rider.dds", HeroFalstad.HeroPortrait.HeroSelectPortraitFileName);
            Assert.Equal("storm_ui_ingame_hero_leaderboard_falstad.dds", HeroFalstad.HeroPortrait.LeaderboardPortraitFileName);
            Assert.Equal("storm_ui_ingame_hero_loadingscreen_falstad.dds", HeroFalstad.HeroPortrait.LoadingScreenPortraitFileName);
            Assert.Equal("storm_ui_ingame_partypanel_btn_falstad.dds", HeroFalstad.HeroPortrait.PartyPanelPortraitFileName);
            Assert.Equal("ui_targetportrait_hero_falstad.dds", HeroFalstad.HeroPortrait.TargetPortraitFileName);
        }

        [Fact]
        public void RatingsTests()
        {
            Assert.Equal(5.0, HeroFalstad.Ratings.Complexity);
            Assert.Equal(8.0, HeroFalstad.Ratings.Damage);
            Assert.Equal(4.0, HeroFalstad.Ratings.Survivability);
            Assert.Equal(4.0, HeroFalstad.Ratings.Utility);
        }

        [Fact]
        public void RolesTests()
        {
            Assert.Equal(1, HeroFalstad.Roles.Count);
            Assert.Equal("Assassin", HeroFalstad.Roles[0]);
        }

        [Fact]
        public void TalentTests()
        {
            // Secret Weapon
            Talent talent = HeroFalstad.Talents["FalstadMasteryHammerangSecretWeapon"];
            Assert.Equal(TalentTier.Level7, talent.Tier);
            Assert.Equal("FalstadMasteryHammerangSecretWeapon", talent.ReferenceNameId);
            Assert.Equal("Secret Weapon", talent.Name);
            Assert.Equal("FalstadSecretWeaponTalent", talent.ShortTooltipNameId);
            Assert.Equal("FalstadSecretWeaponTalent", talent.FullTooltipNameId);
            Assert.Equal("storm_ui_icon_falstad_hammerang.dds", talent.IconFileName);

            Assert.True(string.IsNullOrEmpty(talent.Tooltip.Energy.EnergyTooltip?.RawDescription));
            Assert.True(string.IsNullOrEmpty(talent.Tooltip.Cooldown.CooldownTooltip?.RawDescription));
            Assert.Null(talent.Tooltip.Charges.CountMax);

            Assert.Equal("Increases Hammerang range, Basic Attack damage", talent.Tooltip.ShortTooltip.RawDescription);
            Assert.Equal("Increases Hammerang's range by <c val=\"#TooltipNumbers\">30%</c> and Basic Attacks deal <c val=\"#TooltipNumbers\">60%</c> bonus damage while Hammerang is in flight.", talent.Tooltip.FullTooltip.RawDescription);

            Assert.Equal(1, talent.Column);

            // Wingman
            talent = HeroFalstad.Talents["FalstadWingman"];
            Assert.Equal(TalentTier.Level1, talent.Tier);
            Assert.Equal("FalstadWingman", talent.ReferenceNameId);
            Assert.Equal("Wingman", talent.Name);
            Assert.Equal("FalstadWingmanHotbarTalent", talent.ShortTooltipNameId);
            Assert.Equal("FalstadWingmanHotbarTalent", talent.FullTooltipNameId);
            Assert.Equal("storm_ui_icon_talent_bribe.dds", talent.IconFileName);

            Assert.True(string.IsNullOrEmpty(talent.Tooltip.Energy.EnergyTooltip?.RawDescription));
            Assert.Null(talent.Tooltip.Cooldown.CooldownTooltip?.RawDescription);
            Assert.Equal(4, talent.Tooltip.Charges.CountMax);
            Assert.Null(talent.Tooltip.Charges.CountStart);
            Assert.Equal(1, talent.Tooltip.Charges.CountUse);
            Assert.True(talent.Tooltip.Charges.HasCharges);
            Assert.Null(talent.Tooltip.Charges.IsHideCount);
        }

        [Fact]
        public void TraitAbilitiesTests()
        {
            Ability ability = HeroFalstad.Abilities["FalstadTailwind"];
            Assert.Equal(AbilityTier.Trait, ability.Tier);
            Assert.Equal("FalstadTailwind", ability.ReferenceNameId);
            Assert.Equal("Tailwind", ability.Name);
            Assert.Equal("FalstadTailwind", ability.ShortTooltipNameId);
            Assert.Equal("FalstadTailwind", ability.FullTooltipNameId);
            Assert.Equal("storm_ui_icon_falstad_tailwind.dds", ability.IconFileName);

            Assert.True(string.IsNullOrEmpty(ability.Tooltip.Energy.EnergyTooltip?.RawDescription));
            Assert.True(string.IsNullOrEmpty(ability.Tooltip.Cooldown.CooldownTooltip?.RawDescription));
            Assert.Null(ability.Tooltip.Charges.CountMax);

            Assert.Equal("After not taking damage for a brief period, gain increased Movement Speed", ability.Tooltip.ShortTooltip.RawDescription);
            Assert.Equal("Gain <c val=\"#TooltipNumbers\">15%</c> increased Movement Speed after not taking damage for <c val=\"#TooltipNumbers\">6</c> seconds.", ability.Tooltip.FullTooltip.RawDescription);
        }

        [Fact]
        public void WeaponTests()
        {
            Assert.Equal(1, HeroFalstad.Weapons.Count);
            Assert.Equal("HeroFalstad", HeroFalstad.Weapons[0].WeaponNameId);
            Assert.Equal(5.5, HeroFalstad.Weapons[0].Range);
            Assert.Equal(0.7, HeroFalstad.Weapons[0].Period);
            Assert.Equal(104.0, HeroFalstad.Weapons[0].Damage);
            Assert.Equal(0.04, HeroFalstad.Weapons[0].DamageScaling);
        }
    }
}
