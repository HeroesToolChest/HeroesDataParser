using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class FalstadDataTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void BasicAbilitiesTests()
        {
            Ability ability = HeroFalstad.GetAbility("FalstadHammerang");
            Assert.AreEqual(AbilityTier.Basic, ability.Tier);
            Assert.AreEqual("FalstadHammerang", ability.ReferenceNameId);
            Assert.AreEqual("Hammerang", ability.Name);
            Assert.AreEqual("FalstadHammerang", ability.ShortTooltipNameId);
            Assert.AreEqual("FalstadHammerang", ability.FullTooltipNameId);
            Assert.AreEqual("storm_ui_icon_falstad_hammerang.dds", ability.IconFileName);
            Assert.AreEqual(AbilityType.Q, ability.AbilityType);

            Assert.AreEqual("<s val=\"StandardTooltipDetails\">Mana: 70</s>", ability.Tooltip.Energy.EnergyTooltip?.RawDescription);
            Assert.AreEqual("Cooldown: 10 seconds", ability.Tooltip.Cooldown.CooldownTooltip?.RawDescription);
            Assert.IsTrue(string.IsNullOrEmpty(ability.Tooltip.Life.LifeCostTooltip?.RawDescription));
            Assert.IsNull(ability.Tooltip.Charges.CountMax);

            Assert.AreEqual("Throw out a Hammer that returns. Hammer slows and damages enemies", ability.Tooltip.ShortTooltip.RawDescription);
            Assert.AreEqual("Throw out a Hammer that returns to Falstad, dealing <c val=\"#TooltipNumbers\">121~~0.04~~</c> damage and slowing enemies by <c val=\"#TooltipNumbers\">25%</c> for <c val=\"#TooltipNumbers\">2</c> seconds.", ability.Tooltip.FullTooltip.RawDescription);
        }

        [TestMethod]
        public void BasicDataTests()
        {
            Assert.AreEqual("Falstad", HeroFalstad.Name);
            Assert.AreEqual("Falstad", HeroFalstad.CHeroId);
            Assert.AreEqual("HeroFalstad", HeroFalstad.CUnitId);
            Assert.AreEqual("Fals", HeroFalstad.AttributeId);
            Assert.AreEqual("Medium", HeroFalstad.Difficulty);
            Assert.AreEqual(HeroFranchise.Warcraft, HeroFalstad.Franchise);
            Assert.AreEqual(HeroGender.Male, HeroFalstad.Gender);
            Assert.AreEqual(0.9375, HeroFalstad.InnerRadius);
            Assert.AreEqual(0.9375, HeroFalstad.Radius);
            Assert.AreEqual("2014-03-13", HeroFalstad.ReleaseDate.Value.ToString("yyyy-MM-dd"));
            Assert.AreEqual(12.0, HeroFalstad.Sight);
            Assert.AreEqual(4.3984, HeroFalstad.Speed);
            Assert.AreEqual("Ranged", HeroFalstad.Type);
            Assert.AreEqual(Rarity.Epic, HeroFalstad.Rarity);
            Assert.AreEqual("An Assassin that can fly large distances, excelling on large battlegrounds.", HeroFalstad.Description.RawDescription);
        }

        [TestMethod]
        public void EnergyTests()
        {
            Assert.AreEqual(500, HeroFalstad.Energy.EnergyMax);
            Assert.AreEqual("Mana", HeroFalstad.Energy.EnergyType);
            Assert.AreEqual(3.0, HeroFalstad.Energy.EnergyRegenerationRate);
        }

        [TestMethod]
        public void HeroicAbilitiesTests()
        {
            Ability ability = HeroFalstad.GetAbility("FalstadHinterlandBlast");
            Assert.AreEqual(AbilityTier.Heroic, ability.Tier);
            Assert.AreEqual("FalstadHinterlandBlast", ability.ReferenceNameId);
            Assert.AreEqual("Hinterland Blast", ability.Name);
            Assert.AreEqual("FalstadHinterlandBlast", ability.ShortTooltipNameId);
            Assert.AreEqual("FalstadHinterlandBlast", ability.FullTooltipNameId);
            Assert.AreEqual("storm_ui_icon_falstad_hinterlandblast.dds", ability.IconFileName);

            Assert.AreEqual("<s val=\"StandardTooltipDetails\">Mana: 100</s>", ability.Tooltip.Energy.EnergyTooltip?.RawDescription);
            Assert.AreEqual("Mana: 100", ability.Tooltip.Energy.EnergyTooltip?.PlainText);
            Assert.AreEqual("Cooldown: 120 seconds", ability.Tooltip.Cooldown.CooldownTooltip?.RawDescription);
            Assert.IsTrue(string.IsNullOrEmpty(ability.Tooltip.Life.LifeCostTooltip?.RawDescription));
            Assert.IsNull(ability.Tooltip.Charges.CountMax);

            Assert.AreEqual("Long range damage beam", ability.Tooltip.ShortTooltip.RawDescription);
            Assert.AreEqual("After <c val=\"#TooltipNumbers\">1</c> second, deal <c val=\"#TooltipNumbers\">475~~0.0475~~</c> damage to enemies within a long line. The cooldown is reduced by <c val=\"#TooltipNumbers\">25</c> seconds for every enemy Hero hit.", ability.Tooltip.FullTooltip.RawDescription);
        }

        [TestMethod]
        public void LifeTests()
        {
            Assert.AreEqual(1365.0, HeroFalstad.Life.LifeMax);
            Assert.AreEqual(0.04, HeroFalstad.Life.LifeScaling);
            Assert.AreEqual(2.8437, HeroFalstad.Life.LifeRegenerationRate);
            Assert.AreEqual(0.04, HeroFalstad.Life.LifeRegenerationRateScaling);
        }

        [TestMethod]
        public void MountAbilitiesTests()
        {
            Ability ability = HeroFalstad.GetAbility("FalstadFlight");
            Assert.AreEqual(AbilityTier.Mount, ability.Tier);
            Assert.AreEqual("FalstadFlight", ability.ReferenceNameId);
            Assert.AreEqual("Flight", ability.Name);
            Assert.AreEqual("FalstadFlight", ability.ShortTooltipNameId);
            Assert.AreEqual("FalstadFlight", ability.FullTooltipNameId);
            Assert.AreEqual("storm_ui_icon_falstad_mount.dds", ability.IconFileName);

            Assert.IsTrue(string.IsNullOrEmpty(ability.Tooltip.Energy.EnergyTooltip?.RawDescription));
            Assert.AreEqual("Cooldown: 55 seconds", ability.Tooltip.Cooldown.CooldownTooltip?.RawDescription);
            Assert.IsNull(ability.Tooltip.Charges.CountMax);

            Assert.AreEqual("Instead of mounting, Falstad can fly a great distance over terrain", ability.Tooltip.ShortTooltip.RawDescription);
            Assert.AreEqual("Instead of mounting, Falstad can fly a great distance over terrain.", ability.Tooltip.FullTooltip.RawDescription);
        }

        [TestMethod]
        public void PortraitsTests()
        {
            Assert.AreEqual("storm_ui_ingame_heroselect_btn_gryphon_rider.dds", HeroFalstad.HeroPortrait.HeroSelectPortraitFileName);
            Assert.AreEqual("storm_ui_ingame_hero_leaderboard_falstad.dds", HeroFalstad.HeroPortrait.LeaderboardPortraitFileName);
            Assert.AreEqual("storm_ui_ingame_hero_loadingscreen_falstad.dds", HeroFalstad.HeroPortrait.LoadingScreenPortraitFileName);
            Assert.AreEqual("storm_ui_ingame_partypanel_btn_falstad.dds", HeroFalstad.HeroPortrait.PartyPanelPortraitFileName);
            Assert.AreEqual("ui_targetportrait_hero_falstad.dds", HeroFalstad.HeroPortrait.TargetPortraitFileName);
        }

        [TestMethod]
        public void RatingsTests()
        {
            Assert.AreEqual(5.0, HeroFalstad.Ratings.Complexity);
            Assert.AreEqual(8.0, HeroFalstad.Ratings.Damage);
            Assert.AreEqual(4.0, HeroFalstad.Ratings.Survivability);
            Assert.AreEqual(4.0, HeroFalstad.Ratings.Utility);
        }

        [TestMethod]
        public void RolesTests()
        {
            Assert.AreEqual(1, HeroFalstad.Roles.Count);
            Assert.AreEqual("Assassin", HeroFalstad.Roles[0]);
        }

        [TestMethod]
        public void TalentTests()
        {
            // Secret Weapon
            Talent talent = HeroFalstad.Talents["FalstadMasteryHammerangSecretWeapon"];
            Assert.AreEqual(TalentTier.Level7, talent.Tier);
            Assert.AreEqual("FalstadMasteryHammerangSecretWeapon", talent.ReferenceNameId);
            Assert.AreEqual("Secret Weapon", talent.Name);
            Assert.AreEqual("FalstadSecretWeaponTalent", talent.ShortTooltipNameId);
            Assert.AreEqual("FalstadSecretWeaponTalent", talent.FullTooltipNameId);
            Assert.AreEqual("storm_ui_icon_falstad_hammerang.dds", talent.IconFileName);
            Assert.AreEqual(AbilityType.Q, talent.AbilityType);

            Assert.IsTrue(string.IsNullOrEmpty(talent.Tooltip.Energy.EnergyTooltip?.RawDescription));
            Assert.IsTrue(string.IsNullOrEmpty(talent.Tooltip.Cooldown.CooldownTooltip?.RawDescription));
            Assert.IsNull(talent.Tooltip.Charges.CountMax);

            Assert.AreEqual("Increases Hammerang range, Basic Attack damage", talent.Tooltip.ShortTooltip.RawDescription);
            Assert.AreEqual("Increases Hammerang's range by <c val=\"#TooltipNumbers\">30%</c> and Basic Attacks deal <c val=\"#TooltipNumbers\">60%</c> bonus damage while Hammerang is in flight.", talent.Tooltip.FullTooltip.RawDescription);

            Assert.AreEqual(1, talent.Column);
            Assert.IsTrue(talent.AbilityTalentLinkIds.Contains("FalstadHammerang"));

            // Wingman
            talent = HeroFalstad.Talents["FalstadWingman"];
            Assert.AreEqual(TalentTier.Level1, talent.Tier);
            Assert.AreEqual("FalstadWingman", talent.ReferenceNameId);
            Assert.AreEqual("Wingman", talent.Name);
            Assert.AreEqual("FalstadWingmanHotbarTalent", talent.ShortTooltipNameId);
            Assert.AreEqual("FalstadWingmanHotbarTalent", talent.FullTooltipNameId);
            Assert.AreEqual("storm_ui_icon_talent_bribe.dds", talent.IconFileName);
            Assert.AreEqual(AbilityType.Active, talent.AbilityType);
            Assert.IsTrue(talent.IsActive);

            Assert.IsTrue(string.IsNullOrEmpty(talent.Tooltip.Energy.EnergyTooltip?.RawDescription));
            Assert.IsNull(talent.Tooltip.Cooldown.CooldownTooltip?.RawDescription);
            Assert.AreEqual(4, talent.Tooltip.Charges.CountMax);
            Assert.IsNull(talent.Tooltip.Charges.CountStart);
            Assert.AreEqual(1, talent.Tooltip.Charges.CountUse);
            Assert.IsTrue(talent.Tooltip.Charges.HasCharges);
            Assert.IsNull(talent.Tooltip.Charges.IsHideCount);

            // Hinterland Blast
            talent = HeroFalstad.Talents["FalstadHeroicAbilityHinterlandBlast"];
            Assert.IsTrue(talent.AbilityTalentLinkIds.Contains("FalstadHinterlandBlast"));
            Assert.IsTrue(talent.IsActive);
            Assert.AreEqual(AbilityType.Heroic, talent.AbilityType);
        }

        [TestMethod]
        public void TraitAbilitiesTests()
        {
            Ability ability = HeroFalstad.GetAbility("FalstadTailwind");
            Assert.AreEqual(AbilityTier.Trait, ability.Tier);
            Assert.AreEqual("FalstadTailwind", ability.ReferenceNameId);
            Assert.AreEqual("Tailwind", ability.Name);
            Assert.AreEqual("FalstadTailwind", ability.ShortTooltipNameId);
            Assert.AreEqual("FalstadTailwind", ability.FullTooltipNameId);
            Assert.AreEqual("storm_ui_icon_falstad_tailwind.dds", ability.IconFileName);

            Assert.IsTrue(string.IsNullOrEmpty(ability.Tooltip.Energy.EnergyTooltip?.RawDescription));
            Assert.IsTrue(string.IsNullOrEmpty(ability.Tooltip.Cooldown.CooldownTooltip?.RawDescription));
            Assert.IsNull(ability.Tooltip.Charges.CountMax);

            Assert.AreEqual("After not taking damage for a brief period, gain increased Movement Speed", ability.Tooltip.ShortTooltip.RawDescription);
            Assert.AreEqual("Gain <c val=\"#TooltipNumbers\">15%</c> increased Movement Speed after not taking damage for <c val=\"#TooltipNumbers\">6</c> seconds.", ability.Tooltip.FullTooltip.RawDescription);
        }

        [TestMethod]
        public void WeaponTests()
        {
            List<UnitWeapon> unitWeapons = HeroFalstad.Weapons.ToList();
            Assert.AreEqual(1, unitWeapons.Count);
            Assert.AreEqual("HeroFalstad", unitWeapons[0].WeaponNameId);
            Assert.AreEqual(5.5, unitWeapons[0].Range);
            Assert.AreEqual(0.7, unitWeapons[0].Period);
            Assert.AreEqual(104.0, unitWeapons[0].Damage);
            Assert.AreEqual(0.04, unitWeapons[0].DamageScaling);
        }
    }
}
