using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.RewardPortraitParserTests
{
    [TestClass]
    public class Season4TL2018GrandMasterTest : RewardPortraitParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("2018 S4 Team League Portrait", Season4TL2018GrandMaster.Name);
            Assert.IsTrue(string.IsNullOrEmpty(Season4TL2018GrandMaster.HeroId));
            Assert.AreEqual("PortraitAchievements1", Season4TL2018GrandMaster.CollectionCategory);
            Assert.AreEqual("2018Season4TLGrandMaster", Season4TL2018GrandMaster.HyperlinkId);
            Assert.AreEqual(22, Season4TL2018GrandMaster.IconSlot);
            Assert.AreEqual("2018Season4TLGrandMaster", Season4TL2018GrandMaster.Id);
            Assert.AreEqual(Rarity.Common, Season4TL2018GrandMaster.Rarity);
            Assert.AreEqual(6, Season4TL2018GrandMaster.TextureSheet.Columns);
            Assert.AreEqual(6, Season4TL2018GrandMaster.TextureSheet.Rows);
            Assert.AreEqual("ui_heroes_portraits_sheet34.dds", Season4TL2018GrandMaster.TextureSheet.Image);
        }
    }
}
