using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.PortraitPackParserTests
{
    [TestClass]
#pragma warning disable SA1649 // File name should match first type name
    public class PortraitPackParserBaseTest : ParserBase
#pragma warning restore SA1649 // File name should match first type name
    {
        public PortraitPackParserBaseTest()
        {
            Parse();
        }

        protected PortraitPack WhitemaneSpooky18ToonPortrait { get; set; }
        protected PortraitPack StitchesPortraitSummer { get; set; }
        protected PortraitPack QhiraEmblemPortrait { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            PortraitPackParser portraitParser = new PortraitPackParser(XmlDataService);
            Assert.IsTrue(portraitParser.Items.Count > 0);
        }

        private void Parse()
        {
            PortraitPackParser portraitParser = new PortraitPackParser(XmlDataService);
            WhitemaneSpooky18ToonPortrait = portraitParser.Parse("WhitemaneSpooky18ToonPortrait");
            StitchesPortraitSummer = portraitParser.Parse("StitchesPortraitSummer");
            QhiraEmblemPortrait = portraitParser.Parse("QhiraEmblemPortrait");
        }
    }
}
