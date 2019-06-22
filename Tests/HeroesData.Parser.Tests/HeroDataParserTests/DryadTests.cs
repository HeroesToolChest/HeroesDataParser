using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class DryadTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void AbilityMountNoCooldownUntilTalentUpgradeTest()
        {
            Ability ability = HeroDryad.GetAbility("DryadDryadsSwiftness");
            Assert.IsTrue(string.IsNullOrEmpty(ability.Tooltip.Cooldown?.CooldownTooltip?.RawDescription));
        }

        [TestMethod]
        public void TalentCooldownTest()
        {
            Talent talent = HeroDryad.GetTalent("DryadGallopingGait");
            Assert.AreEqual("Cooldown: 30 seconds", talent.Tooltip.Cooldown?.CooldownTooltip?.RawDescription);
        }

        [TestMethod]
        public void AbilityTalentLinkIdTest()
        {
            Talent talent = HeroDryad.GetTalent("DryadHippityHop");
            Assert.IsTrue(talent.ContainsAbilityTalentLinkId("DryadDryadsSwiftness"));
            Assert.AreEqual(AbilityType.Z, talent.AbilityType);
        }
    }
}
