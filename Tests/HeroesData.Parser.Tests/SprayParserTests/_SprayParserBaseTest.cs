using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.SprayParserTests
{
    [TestClass]
#pragma warning disable SA1649 // File name should match first type name
    public class SprayParserBaseTest : ParserBase
#pragma warning restore SA1649 // File name should match first type name
    {
        public SprayParserBaseTest()
        {
            Parse();
        }

        protected Spray CarbotLiLi { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            SprayParser sprayParser = new SprayParser(XmlDataService);
            Assert.IsTrue(sprayParser.Items.Count > 0);
        }

        private void Parse()
        {
            SprayParser sprayParser = new SprayParser(XmlDataService);
            CarbotLiLi = sprayParser.Parse("SprayStaticCarbotsLili");
        }
    }
}
