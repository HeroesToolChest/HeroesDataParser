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
            Assert.AreEqual(Franchise.Warcraft, HeroTestHero.Franchise);
        }

        [TestMethod]
        public void HeroEnergyTypeTest()
        {
            Assert.AreEqual("Mana", HeroTestHero.Energy.EnergyType);
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
        public void TalentCooldownTest()
        {
            Talent talent = HeroTestHero.GetTalent("TestHeroBattleRage");
            Assert.AreEqual("Charge Cooldown: 40 seconds", talent.Tooltip.Cooldown.CooldownTooltip?.RawDescription);
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
        public void TalentActiveCooldownOverrideTextTest()
        {
            Talent talent = HeroTestHero.GetTalent("TestHeroTimeOut");
            Assert.AreEqual("Cooldown: 60 seconds", talent.Tooltip.Cooldown.CooldownTooltip.RawDescription);
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
