using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.TypeDescriptionTests
{
    [TestClass]
#pragma warning disable SA1649 // File name should match first type name
    public class TypeDescriptionParserBaseTest : ParserBase
#pragma warning restore SA1649 // File name should match first type name
    {
        public TypeDescriptionParserBaseTest()
        {
            Parse();
        }

        protected TypeDescription Gold { get; set; }
        protected TypeDescription LootChestHalloween2020Epic { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            TypeDescriptionParser typeDescriptionParser = new TypeDescriptionParser(XmlDataService);
            Assert.IsTrue(typeDescriptionParser.Items.Count > 0);
        }

        private void Parse()
        {
            TypeDescriptionParser typeDescriptionParser = new TypeDescriptionParser(XmlDataService);
            Gold = typeDescriptionParser.Parse("Gold");
            LootChestHalloween2020Epic = typeDescriptionParser.Parse("LootChestHalloween2020Epic");
        }
    }
}
