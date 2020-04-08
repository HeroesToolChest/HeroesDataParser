using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.BannerParserTests
{
    [TestClass]
#pragma warning disable SA1649 // File name should match first type name
    public class BannerParserBaseTest : ParserBase
#pragma warning restore SA1649 // File name should match first type name
    {
        public BannerParserBaseTest()
        {
            Parse();
        }

        protected Banner AmberWizardWarbanner { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            BannerParser bannerParser = new BannerParser(XmlDataService);
            Assert.IsTrue(bannerParser.Items.Count > 0);
        }

        private void Parse()
        {
            BannerParser bannerParser = new BannerParser(XmlDataService);
            AmberWizardWarbanner = bannerParser.Parse("BannerD3WizardRareVar1");
        }
    }
}
