using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.RewardPortraitParserTests
{
    [TestClass]
    public class WhitemaneSpooky18ToonPortraitTest : RewardPortraitParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("Toon Cursed Witch Whitemane Portrait", WhitemaneSpooky18ToonPortrait.Name);
            Assert.AreEqual("ToonCursedWitchWhitemanePortrait", WhitemaneSpooky18ToonPortrait.HyperlinkId);
            Assert.AreEqual("WhitemaneSpooky18ToonPortrait", WhitemaneSpooky18ToonPortrait.Id);
            Assert.AreEqual(Rarity.Common, WhitemaneSpooky18ToonPortrait.Rarity);
        }
    }
}
