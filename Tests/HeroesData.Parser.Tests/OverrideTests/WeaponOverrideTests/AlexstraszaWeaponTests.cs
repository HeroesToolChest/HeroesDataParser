using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.WeaponOverrideTests
{
    [TestClass]
    public class AlexstraszaWeaponTests : OverrideBaseTests, IWeaponOverride
    {
        private readonly string Hero = "Alexstrasza";

        public AlexstraszaWeaponTests()
            : base()
        {
            LoadOverrideIntoTestWeapon(WeaponName);
        }

        public string WeaponName => "Ffffwwwwaaa";

        protected override string CHeroId => Hero;

        [TestMethod]
        public void DamageOverrideTest()
        {
            Assert.AreEqual(500, TestWeapon.Damage);
        }

        [TestMethod]
        public void RangeOverrideTest()
        {
            Assert.AreEqual(5, TestWeapon.Range);
        }
    }
}
