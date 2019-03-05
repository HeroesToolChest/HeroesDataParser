using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroSkinParserTests
{
    [TestClass]
    public class HeroSkinParserBaseTest : ParserBase
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
            HeroSkinParser heroSkinParser = new HeroSkinParser(GameData, DefaultData);
            Assert.IsTrue(heroSkinParser.Items.Count > 0);
        }

        private void Parse()
        {
            HeroSkinParser heroSkinParser = new HeroSkinParser(GameData, DefaultData);
            AbathurCommonSkin = heroSkinParser.Parse("AbathurBone");
            AbathurMechaVar1Skin = heroSkinParser.Parse("AbathurMechaVar1");
        }
    }
}
