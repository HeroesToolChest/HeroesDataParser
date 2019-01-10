using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    [TestClass]
    public class TracerTests : HeroDataBaseTest
    {
        [TestMethod]
        public void AbilityTalentLinkIdsTests()
        {
            Talent talent = HeroTracer.Talents["TracerJumper"];
            Assert.IsTrue(talent.AbilityTalentLinkIds.Count == 1);
            Assert.IsTrue(talent.AbilityTalentLinkIds.Contains("TracerBlink"));
        }
    }
}
