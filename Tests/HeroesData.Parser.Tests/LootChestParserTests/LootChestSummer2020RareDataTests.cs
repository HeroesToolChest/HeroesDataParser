using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.LootChestParserTests
{
    [TestClass]
    public class LootChestSummer2020RareDataTests : LootChestParserBaseTest
    {
        [TestMethod]
        public void PropertiesTest()
        {
            Assert.AreEqual("LootChestSummer2020Rare", LootChestSummer2020Rare.Id);
            Assert.AreEqual("LootChestSummer2020Rare", LootChestSummer2020Rare.HyperlinkId);
            Assert.AreEqual("description of loot chest", LootChestSummer2020Rare.Description.RawDescription);
            Assert.IsNull(LootChestSummer2020Rare.EventName);
            Assert.AreEqual("Name of loot chest", LootChestSummer2020Rare.Name);
            Assert.AreEqual("LootChestSummer2020Rare", LootChestSummer2020Rare.TypeDescription);
            Assert.AreEqual(5, LootChestSummer2020Rare.MaxRerolls);
            Assert.AreEqual(Rarity.Rare, LootChestSummer2020Rare.Rarity);
        }
    }
}
