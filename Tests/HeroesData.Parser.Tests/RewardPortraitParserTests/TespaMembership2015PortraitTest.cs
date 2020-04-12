using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.RewardPortraitParserTests
{
    [TestClass]
    public class TespaMembership2015PortraitTest : RewardPortraitParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("2015 Tespa Membership Portrait", TespaMembership2015Portrait.Name);
            Assert.IsTrue(string.IsNullOrEmpty(TespaMembership2015Portrait.HeroId));
            Assert.AreEqual("PortraitAchievements1", TespaMembership2015Portrait.CollectionCategory);
            Assert.AreEqual("2015TespaMembershipPortrait", TespaMembership2015Portrait.HyperlinkId);
            Assert.AreEqual(11, TespaMembership2015Portrait.IconSlot);
            Assert.AreEqual("2015TespaMembershipPortrait", TespaMembership2015Portrait.Id);
            Assert.AreEqual(Rarity.Common, TespaMembership2015Portrait.Rarity);
            Assert.AreEqual(6, TespaMembership2015Portrait.TextureSheet.Columns);
            Assert.AreEqual(6, TespaMembership2015Portrait.TextureSheet.Rows);
            Assert.AreEqual("ui_heroes_portraits_sheet3.dds", TespaMembership2015Portrait.TextureSheet.Image);
        }
    }
}
