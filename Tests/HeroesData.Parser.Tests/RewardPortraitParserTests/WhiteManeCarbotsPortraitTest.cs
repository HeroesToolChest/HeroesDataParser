using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.RewardPortraitParserTests
{
    [TestClass]
    public class WhiteManeCarbotsPortraitTest : RewardPortraitParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("Carbot Whitemane Portrait", WhitemaneCarbotsPortrait.Name);
            Assert.AreEqual("You have unlocked this portrait in your Collection", WhitemaneCarbotsPortrait.Description.RawDescription);
            Assert.AreEqual("Forge using Shards, purchase with Gems, or receive in a Loot Chest to unlock.", WhitemaneCarbotsPortrait.DescriptionUnearned.RawDescription);
            Assert.IsTrue(string.IsNullOrEmpty(WhitemaneCarbotsPortrait.HeroId));
            Assert.AreEqual("HeroStormPortrait", WhitemaneCarbotsPortrait.CollectionCategory);
            Assert.AreEqual("CarbotWhitemanePortrait", WhitemaneCarbotsPortrait.HyperlinkId);
            Assert.AreEqual(32, WhitemaneCarbotsPortrait.IconSlot);
            Assert.AreEqual("WhitemaneCarbotsPortrait", WhitemaneCarbotsPortrait.Id);
            Assert.AreEqual("WhitemaneCarbotsPortrait", WhitemaneCarbotsPortrait.PortraitPackId);
            Assert.AreEqual(Rarity.Common, WhitemaneCarbotsPortrait.Rarity);
            Assert.AreEqual(6, WhitemaneCarbotsPortrait.TextureSheet.Columns);
            Assert.AreEqual(6, WhitemaneCarbotsPortrait.TextureSheet.Rows);
            Assert.AreEqual("ui_heroes_portraits_sheet30.dds", WhitemaneCarbotsPortrait.TextureSheet.Image);
        }
    }
}
