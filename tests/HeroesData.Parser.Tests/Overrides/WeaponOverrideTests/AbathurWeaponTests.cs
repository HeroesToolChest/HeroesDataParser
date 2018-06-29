using Xunit;

namespace HeroesData.Parser.Tests.Overrides.WeaponOverrideTests
{
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

        [Fact]
        public void DamageOverrideTest()
        {
            Assert.Equal(1000, TestWeapon.Damage);
        }

        [Fact]
        public void RangeOverrideTest()
        {
            Assert.Equal(1.5, TestWeapon.Range);
        }
    }
}
