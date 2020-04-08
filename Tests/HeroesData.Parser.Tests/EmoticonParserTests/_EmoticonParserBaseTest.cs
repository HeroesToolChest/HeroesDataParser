using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.EmoticonParserTests
{
    [TestClass]
#pragma warning disable SA1649 // File name should match first type name
    public class EmoticonParserBaseTest : ParserBase
#pragma warning restore SA1649 // File name should match first type name
    {
        public EmoticonParserBaseTest()
        {
            Parse();
        }

        protected Emoticon LunaraAngry { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            EmoticonParser emoticonParser = new EmoticonParser(XmlDataService);
            Assert.IsTrue(emoticonParser.Items.Count > 0);
        }

        private void Parse()
        {
            EmoticonParser emoticonParser = new EmoticonParser(XmlDataService);
            LunaraAngry = emoticonParser.Parse("lunara_angry");
        }
    }
}
