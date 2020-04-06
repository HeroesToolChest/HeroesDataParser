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
            Assert.AreEqual(AbilityTypes.Heroic, talent.AbilityTalentId.AbilityType);
            Assert.IsTrue(talent.IsActive);
            Assert.AreEqual("DVaMechBunnyHopHeroic", talent.AbilityTalentLinkIds.ToList()[0]);
        }

        [TestMethod]
        public void DVaMechProMovesTalentTests()
        {
            Talent talent = HeroDva.GetTalent("DVaMechProMoves");
            Assert.AreEqual(1, talent.AbilityTalentLinkIds.Count);
            Assert.AreEqual("DVaMechMechMode", talent.AbilityTalentLinkIds.First());
        }
    }
}
