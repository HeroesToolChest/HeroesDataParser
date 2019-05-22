using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class TracerTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void AbilityTalentLinkIdsTests()
        {
            Talent talent = HeroTracer.GetTalent("TracerJumper");
            Assert.IsTrue(talent.AbilityTalentLinkIds.Count == 1);
            Assert.IsTrue(talent.AbilityTalentLinkIds.Contains("TracerBlink"));
        }
    }
}
