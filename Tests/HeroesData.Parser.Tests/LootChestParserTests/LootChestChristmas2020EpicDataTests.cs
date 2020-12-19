using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.LootChestParserTests
{
    [TestClass]
    public class LootChestChristmas2020EpicDataTests : LootChestParserBaseTest
    {
        [TestMethod]
        public void PropertiesTest()
        {
            Assert.AreEqual("LootChestChristmas2020Epic", LootChestChristmas2020Epic.Id);
            Assert.AreEqual("LootChestChristmas2020Epic", LootChestChristmas2020Epic.HyperlinkId);
            Assert.AreEqual(string.Empty, LootChestChristmas2020Epic.Description.RawDescription);
            Assert.IsNull(LootChestChristmas2020Epic.EventName);
            Assert.IsNull(LootChestChristmas2020Epic.Name);
            Assert.AreEqual("LootChestChristmas2020Epic", LootChestChristmas2020Epic.TypeDescription);
            Assert.AreEqual(3, LootChestChristmas2020Epic.MaxRerolls);
            Assert.AreEqual(Rarity.Epic, LootChestChristmas2020Epic.Rarity);
        }
    }
}
