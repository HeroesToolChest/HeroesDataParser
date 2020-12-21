using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.TypeDescriptionTests
{
    [TestClass]
    public class GoldDataTests : TypeDescriptionParserBaseTest
    {
        [TestMethod]
        public void PropertiesTest()
        {
            Assert.AreEqual("Gold", Gold.Id);
            Assert.AreEqual("Gold", Gold.HyperlinkId);
            Assert.AreEqual("name of the type description", Gold.Name);
            Assert.AreEqual("storm_typedescription_gold.dds", Gold.ImageFileName);
            Assert.AreEqual(1, Gold.IconSlot);
            Assert.AreEqual("storm_ui_heroes_rewardicons_sheet.dds", Gold.TextureSheet.Image);
            Assert.AreEqual(12, Gold.TextureSheet.Rows);
            Assert.AreEqual(5, Gold.TextureSheet.Columns);
        }
    }
}
