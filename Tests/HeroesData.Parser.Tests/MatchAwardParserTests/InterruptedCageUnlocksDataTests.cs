using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.MatchAwardParserTests
{
    [TestClass]
    public class AbathurBoneDataTests : MatchAwardParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("storm_ui_mvp_loyaldefender_%color%.dds", InterruptedCageUnlocks.MVPScreenImageFileName);
            Assert.AreEqual("Loyal Defender", InterruptedCageUnlocks.Name);
            Assert.AreEqual("storm_ui_scorescreen_mvp_loyaldefender_wcav_%team%.dds", InterruptedCageUnlocks.ScoreScreenImageFileName);
            Assert.AreEqual("MostInterruptedCageUnlocks", InterruptedCageUnlocks.ShortName);
            Assert.AreEqual("AwCU", InterruptedCageUnlocks.Tag);
        }
    }
}
