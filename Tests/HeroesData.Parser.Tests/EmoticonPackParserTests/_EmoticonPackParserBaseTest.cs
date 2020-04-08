using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.EmoticonPackParserTests
{
    [TestClass]
#pragma warning disable SA1649 // File name should match first type name
    public class EmoticonPackParserBaseTest : ParserBase
#pragma warning restore SA1649 // File name should match first type name
    {
        public EmoticonPackParserBaseTest()
        {
            Parse();
        }

        protected EmoticonPack JohannaEmoticonPack2 { get; set; }
        protected EmoticonPack DeputyVallaPack1 { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            EmoticonPackParser emoticonPackParser = new EmoticonPackParser(XmlDataService);
            Assert.IsTrue(emoticonPackParser.Items.Count > 0);
        }

        private void Parse()
        {
            EmoticonPackParser emoticonPackParser = new EmoticonPackParser(XmlDataService);
            JohannaEmoticonPack2 = emoticonPackParser.Parse("JohannaEmoticonPack2");
            DeputyVallaPack1 = emoticonPackParser.Parse("DeputyVallaPack1");
        }
    }
}
