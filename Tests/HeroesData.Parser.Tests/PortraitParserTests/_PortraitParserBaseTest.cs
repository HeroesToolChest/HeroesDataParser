using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.PortraitParserTests
{
    [TestClass]
#pragma warning disable SA1649 // File name should match first type name
    public class PortraitParserBaseTest : ParserBase
#pragma warning restore SA1649 // File name should match first type name
    {
        public PortraitParserBaseTest()
        {
            Parse();
        }

        protected Portrait StitchesPortraitSummer { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            PortraitParser portraitParser = new PortraitParser(XmlDataService);
            Assert.IsTrue(portraitParser.Items.Count > 0);
        }

        private void Parse()
        {
            PortraitParser portraitParser = new PortraitParser(XmlDataService);
            StitchesPortraitSummer = portraitParser.Parse("StitchesPortraitSummer");
        }
    }
}
