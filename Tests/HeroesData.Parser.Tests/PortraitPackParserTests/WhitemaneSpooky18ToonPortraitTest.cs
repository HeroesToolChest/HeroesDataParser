using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.PortraitPackParserTests
{
    [TestClass]
    public class WhitemaneSpooky18ToonPortraitTest : PortraitPackParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("Toon Cursed Witch Whitemane Portrait", WhitemaneSpooky18ToonPortrait.Name);
            Assert.AreEqual("HallowsEnd", WhitemaneSpooky18ToonPortrait.EventName);
            Assert.AreEqual("WhitemaneSpooky18ToonPortrait", WhitemaneSpooky18ToonPortrait.HyperlinkId);
            Assert.AreEqual("WhitemaneSpooky18ToonPortrait", WhitemaneSpooky18ToonPortrait.Id);
            Assert.AreEqual(Rarity.Common, WhitemaneSpooky18ToonPortrait.Rarity);
            Assert.IsTrue(string.IsNullOrEmpty(WhitemaneSpooky18ToonPortrait.SortName));
            Assert.AreEqual(0, WhitemaneSpooky18ToonPortrait.RewardPortraitIds.Count);
        }
    }
}
