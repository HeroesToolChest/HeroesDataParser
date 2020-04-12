using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.RewardPortraitParserTests
{
    [TestClass]
    public class WhitemaneBasePortraitTest : RewardPortraitParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("Whitemane Portrait", WhitemaneBasePortrait.Name);
            Assert.AreEqual("Unlocks this Hero's Portrait in your Collection", WhitemaneBasePortrait.Description.RawDescription);
            Assert.AreEqual("You have unlocked this Hero's Portrait in your Collection", WhitemaneBasePortrait.DescriptionUnearned.RawDescription);
            Assert.AreEqual("Whitemane", WhitemaneBasePortrait.HeroId);
            Assert.AreEqual("PortraitProgression1", WhitemaneBasePortrait.CollectionCategory);
            Assert.AreEqual("WhitemanePortrait", WhitemaneBasePortrait.HyperlinkId);
            Assert.AreEqual(27, WhitemaneBasePortrait.IconSlot);
            Assert.AreEqual("WhitemaneBasePortrait", WhitemaneBasePortrait.Id);
            Assert.AreEqual(Rarity.Common, WhitemaneBasePortrait.Rarity);
            Assert.AreEqual(6, WhitemaneBasePortrait.TextureSheet.Columns);
            Assert.AreEqual(6, WhitemaneBasePortrait.TextureSheet.Rows);
            Assert.AreEqual("ui_heroes_portraits_sheet7.dds", WhitemaneBasePortrait.TextureSheet.Image);
        }
    }
}
