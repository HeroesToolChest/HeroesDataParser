using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class MephistoTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void AbilityTalentLinkIdsTests()
        {
            Talent talent = HeroMephisto.GetTalent("MephistoShadeOfMephistoGhastlyArmor");
            Assert.IsTrue(talent.AbilityTalentLinkIdsCount == 1);
            Assert.IsTrue(talent.ContainsAbilityTalentLinkId("MephistoShadeOfMephisto"));

            talent = HeroMephisto.GetTalent("MephistoShadeOfMephistoShadeLord");
            Assert.IsTrue(talent.AbilityTalentLinkIdsCount == 1);
        }
    }
}
