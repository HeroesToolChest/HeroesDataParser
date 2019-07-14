using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace HeroesData.Parser.Tests.EmoticonParserTests
{
    [TestClass]
    public class LunaraAngryTests : EmoticonParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("Angry", LunaraAngry.Name);
            Assert.AreEqual("Lunara Angry :lunaangry:", LunaraAngry.Description.RawDescription);
            Assert.AreEqual("Dryad", LunaraAngry.HeroId);
            Assert.IsTrue(string.IsNullOrEmpty(LunaraAngry.HyperlinkId));
            Assert.AreEqual("storm_emoji_lunara_sheet.dds", LunaraAngry.TextureSheet.Image);
            Assert.AreEqual(3, LunaraAngry.TextureSheet.Rows);
            Assert.AreEqual(4, LunaraAngry.TextureSheet.Columns);
            Assert.AreEqual(2, LunaraAngry.UniversalAliases.Count());
            Assert.AreEqual(0, LunaraAngry.LocalizedAliases.Count());
            Assert.AreEqual(0, LunaraAngry.Image.Index);
            Assert.AreEqual(38, LunaraAngry.Image.Width);
        }
    }
}
