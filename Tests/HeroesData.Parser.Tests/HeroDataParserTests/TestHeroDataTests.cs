using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class TestHeroDataTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void HeroBasicPropertiesTests()
        {
            Assert.AreEqual(2.124, HeroTestHero.Speed);
            Assert.AreEqual(6.5, HeroTestHero.Sight);
            Assert.AreEqual("Support", HeroTestHero.Roles.ToList()[0]);
            Assert.AreEqual(HeroFranchise.Warcraft, HeroTestHero.Franchise);
            Assert.AreEqual("TestHeroIllusionMaster", HeroTestHero.MountLinkId);
        }

        [TestMethod]
        public void HeroEnergyTypeTest()
        {
            Assert.AreEqual("Mana", HeroTestHero.Energy.EnergyType);
        }

        [TestMethod]
        public void AbilityNameOverrideTest()
        {
            Ability ability = HeroTestHero.GetAbility("TestHeroBigBoom");
            Assert.AreEqual("Big Boomy Boom", ability.Name);
        }

        [TestMethod]
        public void TalentChargesTest()
        {
            Talent talent = HeroTestHero.GetTalent("TestHeroBattleRage");
            Assert.AreEqual(3, talent.Tooltip.Charges.CountMax);
            Assert.AreEqual(1, talent.Tooltip.Charges.CountUse);
            Assert.IsNull(talent.Tooltip.Charges.CountStart);
            Assert.AreEqual("Charge Cooldown: 40 seconds", talent.Tooltip.Cooldown.CooldownTooltip?.RawDescription);
        }

        [TestMethod]
        public void AbilityCooldownTest()
        {
            Ability ability = HeroTestHero.GetAbility("TestHeroTheHunt");
            Assert.AreEqual("Cooldown: 100 seconds", ability.Tooltip.Cooldown.CooldownTooltip.RawDescription);
        }

        [TestMethod]
        public void TalentCooldownTest()
        {
            Talent talent = HeroTestHero.GetTalent("TestHeroBattleRage");
            Assert.AreEqual("Charge Cooldown: 40 seconds", talent.Tooltip.Cooldown.CooldownTooltip?.RawDescription);
        }

        [TestMethod]
        public void AbilityTooltipOverrideTest()
        {
            Ability ability = HeroTestHero.GetAbility("TestHeroNerazimDummy");
            Assert.AreEqual("Nerazim v2", ability.Name);
            Assert.AreEqual("storm_ui_icon_testhero_nerazim.dds", ability.IconFileName);
            Assert.AreEqual("TestHeroNerazimTalent", ability.ShortTooltipNameId);
            Assert.AreEqual("TestHeroNerazimTalent", ability.FullTooltipNameId);
            Assert.AreEqual("Gain an extra ability", ability.Tooltip.ShortTooltip.RawDescription);
            Assert.AreEqual("Gain an extra ability three times", ability.Tooltip.FullTooltip.RawDescription);
        }

        [TestMethod]
        public void AbilityIllusionMasterTest()
        {
            Ability ability = HeroTestHero.GetAbility("TestHeroIllusionMaster");
            Assert.AreEqual("Illusion Master", ability.Name);
            Assert.AreEqual("storm_ui_icon_testhero_illusiondancer.dds", ability.IconFileName);
            Assert.AreEqual("TestHeroIllusionMasterTalent", ability.ShortTooltipNameId);
            Assert.AreEqual("TestHeroIllusionMasterTalent", ability.FullTooltipNameId);
            Assert.AreEqual("Disappear", ability.Tooltip.ShortTooltip.RawDescription);
            Assert.AreEqual("Disappear and out of sight", ability.Tooltip.FullTooltip.RawDescription);
        }

        [TestMethod]
        public void AbilityTraitAdvancingStrikesTests()
        {
            Ability ability = HeroTestHero.GetAbility("TestHeroAdvancingStrikes");
            Assert.AreEqual("Advancing Strikes", ability.Name);
            Assert.AreEqual("storm_ui_icon_testhero_flowingstrikes.dds", ability.IconFileName);
            Assert.AreEqual("TestHeroAdvancingStrikes", ability.ShortTooltipNameId);
            Assert.AreEqual("TestHeroAdvancingStrikes", ability.FullTooltipNameId);
            Assert.AreEqual("Slash Slash", ability.Tooltip.ShortTooltip.RawDescription);
            Assert.AreEqual("More slashes, more damage", ability.Tooltip.FullTooltip.RawDescription);
        }

        [TestMethod]
        public void AbilityTypesForAbilitiesTests()
        {
            Ability ability = HeroTestHero.GetAbility("TestHeroBigBoom");
            Assert.AreEqual(AbilityType.Heroic, ability.AbilityType);

            ability = HeroTestHero.GetAbility("TestHeroNerazimDummy");
            Assert.AreEqual(AbilityType.W, ability.AbilityType);

            ability = HeroTestHero.GetAbility("TestHeroIllusionMaster");
            Assert.AreEqual(AbilityType.Z, ability.AbilityType);

            ability = HeroTestHero.GetAbility("TestHeroAdvancingStrikes");
            Assert.AreEqual(AbilityType.Trait, ability.AbilityType);

            ability = HeroTestHero.GetAbility("TestHeroActiveAbility");
            Assert.AreEqual(AbilityType.Active, ability.AbilityType);

            ability = HeroTestHero.GetAbility("TestUnitStab");
            Assert.AreEqual(AbilityType.E, ability.AbilityType);

            ability = HeroTestHero.GetAbility("TestUnitCallUnit");
            Assert.AreEqual(AbilityType.W, ability.AbilityType);

            ability = HeroTestHero.GetAbility("FaerieDragonPolymorph");
            Assert.AreEqual(AbilityType.W, ability.AbilityType);

            ability = HeroTestHero.GetAbility("TestHeroCriticalStrikeDummy");
            Assert.AreEqual(AbilityType.W, ability.AbilityType);
        }

        [TestMethod]
        public void AbilityTypesForTalentsTests()
        {
            Talent talent = HeroTestHero.GetTalent("TestHeroDismantle");
            Assert.AreEqual(AbilityType.W, talent.AbilityType);

            talent = HeroTestHero.GetTalent("TestHeroFastAttack");
            Assert.AreEqual(AbilityType.Passive, talent.AbilityType);

            talent = HeroTestHero.GetTalent("TestHeroSpawnLocusts");
            Assert.AreEqual(AbilityType.Active, talent.AbilityType);

            talent = HeroTestHero.GetTalent("TestHeroHighlord");
            Assert.AreEqual(AbilityType.Trait, talent.AbilityType);

            talent = HeroTestHero.GetTalent("TestHeroMasteredStab");
            Assert.AreEqual(AbilityType.E, talent.AbilityType);

            talent = HeroTestHero.GetTalent("TestHeroMekaFall");
            Assert.AreEqual(AbilityType.W, talent.AbilityType);
        }

        [TestMethod]
        public void IsActiveIsQuestForTalentsTests()
        {
            Talent talent = HeroTestHero.GetTalent("TestHeroBattleRage");
            Assert.IsTrue(talent.IsActive);

            talent = HeroTestHero.GetTalent("TestHeroHighlord");
            Assert.IsFalse(talent.IsQuest);

            talent = HeroTestHero.GetTalent("TestHeroMasteredStab");
            Assert.IsTrue(talent.IsQuest);

            talent = HeroTestHero.GetTalent("TestHeroMekaFall");
            Assert.IsFalse(talent.IsQuest);
        }

        [TestMethod]
        public void OverrideTextTests()
        {
            Ability ability = HeroTestHero.GetAbility("TestHeroBigBoom");
            Assert.AreEqual("<s val=\"StandardTooltipDetails\">Mana: 10 picks</s>", ability.Tooltip.Energy.EnergyTooltip?.RawDescription);
            Assert.AreEqual("Cooldown: 20 per second", ability.Tooltip.Cooldown.CooldownTooltip?.RawDescription);

            ability = HeroTestHero.GetAbility("TestHeroBigBoomV2");
            Assert.AreEqual("<s val=\"StandardTooltipDetails\">Health: </s><s val=\"StandardTooltipDetails\">15%</s>", ability.Tooltip.Life.LifeCostTooltip.RawDescription);
            Assert.AreEqual("<s val=\"StandardTooltipDetails\">Mana: 40</s>", ability.Tooltip.Energy.EnergyTooltip.RawDescription);

            ability = HeroTestHero.GetAbility("TestHeroBigBoomV3");
            Assert.AreEqual("<s val=\"StandardTooltipDetails\">Mana: 3</s>", ability.Tooltip.Energy.EnergyTooltip.RawDescription);
            Assert.AreEqual("Cooldown: 12 Seconds", ability.Tooltip.Cooldown.CooldownTooltip.RawDescription);
        }

        [TestMethod]
        public void TalentActiveCooldownOverrideTextTest()
        {
            Talent talent = HeroTestHero.GetTalent("TestHeroTimeOut");
            Assert.AreEqual("Cooldown: 60 seconds", talent.Tooltip.Cooldown.CooldownTooltip.RawDescription);
        }

        [TestMethod]
        public void AbilityButtonNameOverrideTest()
        {
            Ability ability = HeroTestHero.GetAbility("TestHeroEssenseCollection");
            Assert.AreEqual("Cooldown: 5 seconds", ability.Tooltip.Cooldown.CooldownTooltip.RawDescription);
        }

        [TestMethod]
        public void AbilityTalentTooltipShowUsageOffTest()
        {
            Talent talent = HeroTestHero.GetTalent("TestHeroTheWill");
            Assert.IsNull(talent.Tooltip.Cooldown?.CooldownTooltip?.RawDescription);
        }

        [TestMethod]
        public void AbilityTalentLinkIdsForTalentActivableAbilitiesTest()
        {
            Talent talentNoLinks = HeroTestHero.GetTalent("TestHeroArmorUpBodyCheck");
            Assert.IsTrue(talentNoLinks.AbilityTalentLinkIds.Count == 0);

            Talent talentHasLinks = HeroTestHero.GetTalent("TestHeroBodyCheckBruteForce");
            Assert.IsTrue(talentHasLinks.AbilityTalentLinkIds.Count == 1);
            Assert.AreEqual("TestHeroArmorUpBodyCheck", talentHasLinks.AbilityTalentLinkIds.ToList()[0]);
        }
    }
}
