using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.BannerParserTests
{
    [TestClass]
    public class BannerParserBaseTest : ParserBase
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
