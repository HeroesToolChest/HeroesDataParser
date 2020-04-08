using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroSkinParserTests
{
    [TestClass]
#pragma warning disable SA1649 // File name should match first type name
    public class HeroSkinParserBaseTest : ParserBase
#pragma warning restore SA1649 // File name should match first type name
    {
        public HeroSkinParserBaseTest()
        {
            Parse();
        }

        protected HeroSkin AbathurCommonSkin { get; set; }
        protected HeroSkin AbathurMechaVar1Skin { get; set; }

        [TestMethod]
        public void GetItemsTest()
        {
            HeroSkinParser heroSkinParser = new HeroSkinParser(XmlDataService);
            Assert.IsTrue(heroSkinParser.Items.Count > 0);
        }

        private void Parse()
        {
            HeroSkinParser heroSkinParser = new HeroSkinParser(XmlDataService);
            AbathurCommonSkin = heroSkinParser.Parse("AbathurBone");
            AbathurMechaVar1Skin = heroSkinParser.Parse("AbathurMechaVar1");
        }
    }
}
