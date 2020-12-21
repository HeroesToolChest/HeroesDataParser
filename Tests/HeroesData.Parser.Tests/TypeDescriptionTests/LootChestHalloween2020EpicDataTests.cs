using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.TypeDescriptionTests
{
    [TestClass]
    public class LootChestHalloween2020EpicDataTests : TypeDescriptionParserBaseTest
    {
        [TestMethod]
        public void PropertiesTest()
        {
            Assert.AreEqual("LootChestHalloween2020Epic", LootChestHalloween2020Epic.Id);
            Assert.AreEqual("LootChestHalloween2020Epic", LootChestHalloween2020Epic.HyperlinkId);
            Assert.AreEqual("storm_typedescription_lootchesthalloween2020epic.dds", LootChestHalloween2020Epic.ImageFileName);
            Assert.AreEqual(string.Empty, LootChestHalloween2020Epic.Name);
            Assert.AreEqual(41, LootChestHalloween2020Epic.IconSlot);
            Assert.AreEqual("storm_ui_heroes_rewardicons_sheet.dds", LootChestHalloween2020Epic.TextureSheet.Image);
            Assert.AreEqual(12, LootChestHalloween2020Epic.TextureSheet.Rows);
            Assert.AreEqual(5, LootChestHalloween2020Epic.TextureSheet.Columns);
        }
    }
}
