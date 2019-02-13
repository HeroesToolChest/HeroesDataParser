using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class DryadDataTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void AbilityMountNoCooldownUntilTalentUpgradeTest()
        {
            Ability ability = HeroDryad.Abilities["DryadGallopingGait"];
            Assert.IsTrue(string.IsNullOrEmpty(ability.Tooltip.Cooldown?.CooldownTooltip?.RawDescription));
        }

        [TestMethod]
        public void TalentCooldownTest()
        {
            Talent talent = HeroDryad.Talents["DryadGallopingGait"];
            Assert.AreEqual("Cooldown: 30 seconds", talent.Tooltip.Cooldown?.CooldownTooltip?.RawDescription);
        }

        [TestMethod]
        public void AbilityTalentLinkIdTest()
        {
            Talent talent = HeroDryad.Talents["DryadHippityHop"];
            Assert.IsTrue(talent.AbilityTalentLinkIds.Contains("DryadGallopingGait"));
        }
    }
}
