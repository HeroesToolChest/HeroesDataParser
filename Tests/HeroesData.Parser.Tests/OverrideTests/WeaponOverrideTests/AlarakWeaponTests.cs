using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.WeaponOverrideTests
{
    [TestClass]
    public class AlarakWeaponTests : OverrideBaseTests, IWeaponOverride
    {
        private readonly string Hero = "Alarak";

        public AlarakWeaponTests()
            : base()
        {
            LoadOverrideIntoTestWeapon(WeaponName);
        }

        public string WeaponName => "Slashy";

        protected override string CHeroId => Hero;

        [TestMethod]
        public void DamageOverrideTest()
        {
            Assert.AreEqual(500, TestWeapon.Damage);
        }

        [TestMethod]
        public void RangeOverrideTest()
        {
            Assert.AreEqual(2, TestWeapon.Range);
        }
    }
}
