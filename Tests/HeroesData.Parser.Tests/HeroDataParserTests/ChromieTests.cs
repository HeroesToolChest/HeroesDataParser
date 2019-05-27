using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class ChromieTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("Ranged Assassin", HeroChromie.ExpandedRole);
        }

        [TestMethod]
        public void WeaponTests()
        {
            Assert.AreEqual(82, HeroChromie.Weapons.ToList()[0].Damage);
            Assert.AreEqual(7, HeroChromie.Weapons.ToList()[0].Range);
        }

        [TestMethod]
        public void UnitTests()
        {
            Assert.AreEqual(2, HeroChromie.Units.Count());
        }
    }
}
