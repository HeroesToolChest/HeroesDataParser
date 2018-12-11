using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.WeaponOverrideTests
{
    [TestClass]
    public class AbathurWeaponTests : OverrideBaseTests, IWeaponOverride
    {
        private readonly string Hero = "Abathur";

        public AbathurWeaponTests()
            : base()
        {
            LoadOverrideIntoTestWeapon(WeaponName);
        }

        public string WeaponName => "SlapSlap";

        protected override string CHeroId => Hero;

        [TestMethod]
        public void DamageOverrideTest()
        {
            Assert.AreEqual(1000, TestWeapon.Damage);
        }

        [TestMethod]
        public void RangeOverrideTest()
        {
            Assert.AreEqual(1.5, TestWeapon.Range);
        }

        [TestMethod]
        public void WeaponParentLinkOverrideTest()
        {
            Assert.AreEqual("LittleLoco", TestWeapon.ParentLink);
        }
    }
}
