using Xunit;

namespace HeroesData.Parser.Tests.OverrideTests.WeaponOverrideTests
{
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

        [Fact]
        public void DamageOverrideTest()
        {
            Assert.Equal(500, TestWeapon.Damage);
        }

        [Fact]
        public void RangeOverrideTest()
        {
            Assert.Equal(5, TestWeapon.Range);
        }
    }
}
