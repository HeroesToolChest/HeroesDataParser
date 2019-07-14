using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.SprayParserTests
{
    [TestClass]
    public class SprayParserBaseTest : ParserBase
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
