using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.LootChestParserTests
{
    [TestClass]
#pragma warning disable SA1649 // File name should match first type name
    public class LootChestParserBaseTest : ParserBase
#pragma warning restore SA1649 // File name should match first type name
    {
        public LootChestParserBaseTest()
        {
            Parse();
        }

        protected LootChest LootChestSummer2020Rare { get; set; }
        protected LootChest Mobster2019RareLootChest { get; set; }
        protected LootChest LootChestChristmas2020Epic { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            LootChestParser lootChestParser = new LootChestParser(XmlDataService);
            Assert.IsTrue(lootChestParser.Items.Count > 0);
        }

        private void Parse()
        {
            LootChestParser lootChestParser = new LootChestParser(XmlDataService);
            LootChestSummer2020Rare = lootChestParser.Parse("LootChestSummer2020Rare");
            Mobster2019RareLootChest = lootChestParser.Parse("Mobster2019RareLootChest");
            LootChestChristmas2020Epic = lootChestParser.Parse("LootChestChristmas2020Epic");
        }
    }
}
