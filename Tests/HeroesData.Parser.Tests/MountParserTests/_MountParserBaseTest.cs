using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.MountParserTests
{
    [TestClass]
    public class MountParserBaseTest : ParserBase
    {
        public MountParserBaseTest()
        {
            Parse();
        }

        protected Mount MountCloudVar1 { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            MountParser mountParser = new MountParser(XmlDataService);
            Assert.IsTrue(mountParser.Items.Count > 0);
        }

        private void Parse()
        {
            MountParser mountParser = new MountParser(XmlDataService);
            MountCloudVar1 = mountParser.Parse("MountCloudVar1");
        }
    }
}
