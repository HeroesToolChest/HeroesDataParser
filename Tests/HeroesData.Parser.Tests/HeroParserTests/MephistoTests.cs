using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    [TestClass]
    public class MephistoTests : HeroDataBaseTest
    {
        [TestMethod]
        public void AbilityTalentLinkIdsTests()
        {
            Talent talent = HeroMephisto.Talents["MephistoShadeOfMephistoGhastlyArmor"];
            Assert.IsTrue(talent.AbilityTalentLinkIds.Count == 1);
            Assert.IsTrue(talent.AbilityTalentLinkIds.Contains("MephistoShadeOfMephisto"));

            talent = HeroMephisto.Talents["MephistoShadeOfMephistoShadeLord"];
            Assert.IsTrue(talent.AbilityTalentLinkIds.Count == 0);
        }
    }
}
