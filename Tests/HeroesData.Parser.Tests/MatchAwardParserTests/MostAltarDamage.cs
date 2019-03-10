using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.MatchAwardParserTests
{
    [TestClass]
    public class MostAltarDamage : MatchAwardParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("storm_ui_mvp_cannoneer_%color%.dds", MostAltarDamage.MVPScreenImageFileName);
            Assert.AreEqual("Cannoneer", MostAltarDamage.Name);
            Assert.AreEqual("storm_ui_scorescreen_mvp_cannoneer_%team%.dds", MostAltarDamage.ScoreScreenImageFileName);
            Assert.AreEqual("MostAltarDamage", MostAltarDamage.Id);
            Assert.AreEqual("EndOfMatchAwardMostAltarDamageDone", MostAltarDamage.HyperlinkId);
            Assert.AreEqual("AwAD", MostAltarDamage.Tag);
        }
    }
}
