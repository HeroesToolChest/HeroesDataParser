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
        }

        [TestMethod]
        public void HeroEnergyTypeTest()
        {
            Assert.AreEqual("Mana", HeroTestHero.Energy.EnergyType);
        }

        [TestMethod]
        public void AbilityNameOverrideTest()
        {
            Ability ability = HeroTestHero.GetAbilities("TestHeroBigBoom").First();
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
            Ability ability = HeroTestHero.GetAbilities("TestHeroTheHunt").First();
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
            Ability ability = HeroTestHero.GetAbilities("TestHeroNerazimDummy").First();
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
            Ability ability = HeroTestHero.GetAbilities("TestHeroIllusionMaster").First();
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
            Ability ability = HeroTestHero.GetAbilities("TestHeroAdvancingStrikes").First();
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
            Ability ability = HeroTestHero.GetAbilities("TestHeroBigBoom").First();
            Assert.AreEqual(AbilityType.Heroic, ability.AbilityType);

            ability = HeroTestHero.GetAbilities("TestHeroNerazimDummy").First();
            Assert.AreEqual(AbilityType.W, ability.AbilityType);

            ability = HeroTestHero.GetAbilities("TestHeroIllusionMaster").First();
            Assert.AreEqual(AbilityType.Z, ability.AbilityType);

            ability = HeroTestHero.GetAbilities("TestHeroAdvancingStrikes").First();
            Assert.AreEqual(AbilityType.Trait, ability.AbilityType);

            ability = HeroTestHero.GetAbilities("TestHeroActiveAbility").First();
            Assert.AreEqual(AbilityType.Active, ability.AbilityType);

            ability = HeroTestHero.GetAbilities("TestUnitStab").First();
            Assert.AreEqual(AbilityType.E, ability.AbilityType);

            ability = HeroTestHero.GetAbilities("TestUnitCallUnit").First();
            Assert.AreEqual(AbilityType.W, ability.AbilityType);

            ability = HeroTestHero.GetAbilities("FaerieDragonPolymorph").First();
            Assert.AreEqual(AbilityType.W, ability.AbilityType);

            ability = HeroTestHero.GetAbilities("TestHeroCriticalStrikeDummy").First();
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
            Ability ability = HeroTestHero.GetAbilities("TestHeroBigBoom").First();
            Assert.AreEqual("<s val=\"StandardTooltipDetails\">Mana: 10 picks</s>", ability.Tooltip.Energy.EnergyTooltip?.RawDescription);
            Assert.AreEqual("Cooldown: 20 per second", ability.Tooltip.Cooldown.CooldownTooltip?.RawDescription);

            ability = HeroTestHero.GetAbilities("TestHeroBigBoomV2").First();
            Assert.AreEqual("<s val=\"StandardTooltipDetails\">Health: </s><s val=\"StandardTooltipDetails\">15%</s>", ability.Tooltip.Life.LifeCostTooltip.RawDescription);
            Assert.AreEqual("<s val=\"StandardTooltipDetails\">Mana: 40</s>", ability.Tooltip.Energy.EnergyTooltip.RawDescription);

            ability = HeroTestHero.GetAbilities("TestHeroBigBoomV3").First();
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
            Ability ability = HeroTestHero.GetAbilities("TestHeroEssenseCollection").First();
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
            Assert.IsTrue(talentNoLinks.AbilityTalentLinkIdsCount == 0);

            Talent talentHasLinks = HeroTestHero.GetTalent("TestHeroBodyCheckBruteForce");
            Assert.IsTrue(talentHasLinks.AbilityTalentLinkIdsCount == 1);
            Assert.AreEqual("TestHeroArmorUpBodyCheck", talentHasLinks.AbilityTalentLinkIds.ToList()[0]);
        }
    }
}
