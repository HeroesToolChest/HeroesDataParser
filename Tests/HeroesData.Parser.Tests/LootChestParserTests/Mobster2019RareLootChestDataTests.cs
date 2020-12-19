using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.LootChestParserTests
{
    [TestClass]
    public class Mobster2019RareLootChestDataTests : LootChestParserBaseTest
    {
        [TestMethod]
        public void PropertiesTest()
        {
            Assert.AreEqual("Mobster2019RareLootChest", Mobster2019RareLootChest.Id);
            Assert.AreEqual("Mobster2019RareLootChest", Mobster2019RareLootChest.HyperlinkId);
            Assert.AreEqual(string.Empty, Mobster2019RareLootChest.Description.RawDescription);
            Assert.AreEqual("Mobster19", Mobster2019RareLootChest.EventName);
            Assert.AreEqual("Mobster2019RareLootChest", Mobster2019RareLootChest.TypeDescription);
            Assert.IsNull(Mobster2019RareLootChest.Name);
            Assert.AreEqual(5, Mobster2019RareLootChest.MaxRerolls);
            Assert.AreEqual(Rarity.Rare, Mobster2019RareLootChest.Rarity);
        }
    }
}
