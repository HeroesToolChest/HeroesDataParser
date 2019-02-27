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
            MountParser mountParser = new MountParser(GameData, DefaultData);
            Assert.IsTrue(mountParser.Items.Count > 0);
            Assert.IsTrue(mountParser.Items[0].Length == 1);
        }

        private void Parse()
        {
            MountParser mountParser = new MountParser(GameData, DefaultData);
            MountCloudVar1 = mountParser.Parse("MountCloudVar1");
        }
    }
}
