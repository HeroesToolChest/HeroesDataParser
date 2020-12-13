using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.BundleParserTests
{
    [TestClass]
#pragma warning disable SA1649 // File name should match first type name
    public class BundleParserBaseTest : ParserBase
#pragma warning restore SA1649 // File name should match first type name
    {
        public BundleParserBaseTest()
        {
            Parse();
        }

        protected Bundle BattleBundle { get; set; }
        protected Bundle CyberdemonZaryaSkinPack { get; set; }
        protected Bundle WelcomeBundle { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            BundleParser bundleParser = new BundleParser(XmlDataService);
            Assert.IsTrue(bundleParser.Items.Count > 0);
        }

        private void Parse()
        {
            BundleParser bundleParser = new BundleParser(XmlDataService);
            BattleBundle = bundleParser.Parse("BattleBundle");
            CyberdemonZaryaSkinPack = bundleParser.Parse("CyberdemonZaryaSkinPack");
            WelcomeBundle = bundleParser.Parse("WelcomeBundle");
        }
    }
}
