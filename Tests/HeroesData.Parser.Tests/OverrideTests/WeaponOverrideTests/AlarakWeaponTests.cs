using Xunit;

namespace HeroesData.Parser.Tests.OverrideTests.WeaponOverrideTests
{
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

        [Fact]
        public void DamageOverrideTest()
        {
            Assert.Equal(500, TestWeapon.Damage);
        }

        [Fact]
        public void RangeOverrideTest()
        {
            Assert.Equal(2, TestWeapon.Range);
        }
    }
}
