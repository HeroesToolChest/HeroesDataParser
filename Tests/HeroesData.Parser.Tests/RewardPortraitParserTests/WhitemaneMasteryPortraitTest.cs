using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.RewardPortraitParserTests
{
    [TestClass]
    public class WhitemaneMasteryPortraitTest : RewardPortraitParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("Whitemane Mastery Portrait", WhitemaneMasteryPortrait.Name);
            Assert.AreEqual("Unlocks this Hero's Master Portrait in your Collection", WhitemaneMasteryPortrait.Description.RawDescription);
            Assert.AreEqual("Whitemane", WhitemaneMasteryPortrait.HeroId);
            Assert.AreEqual("PortraitProgression1", WhitemaneMasteryPortrait.CollectionCategory);
            Assert.AreEqual("WhitemaneMasteryPortrait", WhitemaneMasteryPortrait.HyperlinkId);
            Assert.AreEqual(27, WhitemaneMasteryPortrait.IconSlot);
            Assert.AreEqual("WhitemaneMasteryPortrait", WhitemaneMasteryPortrait.Id);
            Assert.IsTrue(string.IsNullOrEmpty(WhitemaneMasteryPortrait.PortraitPackId));
            Assert.AreEqual(Rarity.Epic, WhitemaneMasteryPortrait.Rarity);
            Assert.AreEqual(6, WhitemaneMasteryPortrait.TextureSheet.Columns);
            Assert.AreEqual(6, WhitemaneMasteryPortrait.TextureSheet.Rows);
            Assert.AreEqual("ui_heroes_portraits_sheet8.dds", WhitemaneMasteryPortrait.TextureSheet.Image);
        }
    }
}
