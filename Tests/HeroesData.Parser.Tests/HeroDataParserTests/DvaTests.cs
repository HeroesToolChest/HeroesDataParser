using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class DvaTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void BunnyHopTalentTests()
        {
            Talent talent = HeroDva.GetTalent("DVaBunnyHop");
            Assert.AreEqual("Cooldown: 100 Seconds", talent.Tooltip.Cooldown.CooldownTooltip.RawDescription);
            Assert.AreEqual(AbilityType.Heroic, talent.AbilityType);
            Assert.IsTrue(talent.IsActive);
            Assert.AreEqual("DVaMechBunnyHopHeroic", talent.AbilityTalentLinkIds.ToList()[0]);
        }
    }
}
