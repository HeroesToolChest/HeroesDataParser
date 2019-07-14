using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.MatchAwardParserTests
{
    [TestClass]
    public class MostAltarDamage : MatchAwardParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("storm_ui_mvp_test_%color%.dds", MostAltarDamage.MVPScreenImageFileName);
            Assert.AreEqual("storm_ui_test_icon.dds", MostAltarDamage.MVPScreenImageFileNameOriginal);
            Assert.AreEqual("Cannoneer", MostAltarDamage.Name);
            Assert.AreEqual("score_screen_image.dds", MostAltarDamage.ScoreScreenImageFileName);
            Assert.AreEqual("score_screen_image_original.dds", MostAltarDamage.ScoreScreenImageFileNameOriginal);
            Assert.AreEqual("MostAltarDamage", MostAltarDamage.Id);
            Assert.AreEqual("EndOfMatchAwardMostAltarDamageDone", MostAltarDamage.HyperlinkId);
            Assert.AreEqual("AwAD", MostAltarDamage.Tag);
            Assert.AreEqual("Description of most altar damage", MostAltarDamage.Description.RawDescription);
        }
    }
}
