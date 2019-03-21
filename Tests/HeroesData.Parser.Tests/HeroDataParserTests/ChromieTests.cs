using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class ChromieTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void WeaponTests()
        {
            Assert.AreEqual(73, HeroChromie.Weapons[0].Damage);
            Assert.AreEqual(7, HeroChromie.Weapons[0].Range);
        }
    }
}
