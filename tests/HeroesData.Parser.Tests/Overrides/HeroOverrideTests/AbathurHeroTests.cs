using Heroes.Models;
using HeroesData.Parser.UnitData.Overrides;
using Xunit;

namespace HeroesData.Parser.Tests.Overrides.HeroOverrideTest
{
    public class AbathurHeroTests : OverrideBase, IHeroOverride
    {
        private readonly string Hero = "Abathur";

        public AbathurHeroTests()
            : base()
        {
        }

        protected override string CHeroId => Hero;

        [Fact]
        public void CUnitOverrideTest()
        {
            Assert.True(HeroOverride.CUnitOverride.Enabled);
            Assert.Equal("HeroAbathur", HeroOverride.CUnitOverride.CUnit);
        }

        [Fact]
        public void EnergyOverrideTest()
        {
            Assert.True(HeroOverride.EnergyOverride.Enabled);
            Assert.Equal(100, HeroOverride.EnergyOverride.Energy);
        }

        [Fact]
        public void EnergyTypeOverrideTest()
        {
            Assert.True(HeroOverride.EnergyTypeOverride.Enabled);
            Assert.Equal(UnitEnergyType.Charge, HeroOverride.EnergyTypeOverride.EnergyType);
        }

        [Fact]
        public void NameOverrideTest()
        {
            Assert.True(HeroOverride.NameOverride.Enabled);
            Assert.Equal("Acceptable", HeroOverride.NameOverride.Name);
        }

        [Fact]
        public void ShortNameOverrideTest()
        {
            Assert.True(HeroOverride.ShortNameOverride.Enabled);
            Assert.Equal("Funzo", HeroOverride.ShortNameOverride.ShortName);
        }
    }
}
