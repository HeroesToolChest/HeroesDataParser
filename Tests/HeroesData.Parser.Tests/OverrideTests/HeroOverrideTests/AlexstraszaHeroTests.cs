using HeroesData.Parser.UnitData.Overrides;
using Xunit;

namespace HeroesData.Parser.Tests.OverrideTests.HeroOverrideTest
{
    public class AlexstraszaHeroTests : OverrideBaseTests, IHeroOverride
    {
        private readonly string Hero = "Alexstrasza";

        public AlexstraszaHeroTests()
            : base()
        {
        }

        protected override string CHeroId => Hero;

        [Fact]
        public void CUnitOverrideTest()
        {
            Assert.False(HeroOverride.CUnitOverride.Enabled);
        }

        [Fact]
        public void EnergyOverrideTest()
        {
            Assert.True(HeroOverride.EnergyOverride.Enabled);
            Assert.Equal(0, HeroOverride.EnergyOverride.Energy);
        }

        [Fact]
        public void EnergyTypeOverrideTest()
        {
            Assert.True(HeroOverride.EnergyTypeOverride.Enabled);
            Assert.Equal("CrazyPills", HeroOverride.EnergyTypeOverride.EnergyType);
        }

        [Fact]
        public void NameOverrideTest()
        {
            Assert.False(HeroOverride.NameOverride.Enabled);
        }

        [Fact]
        public void ShortNameOverrideTest()
        {
            Assert.False(HeroOverride.ShortNameOverride.Enabled);
        }

        [Fact]
        public void IsNotValidAbilityTest()
        {
            Assert.True(HeroOverride.IsValidAbilityByAbilityId.ContainsKey("AVeryLargeSword"));
            Assert.False(HeroOverride.IsValidAbilityByAbilityId["AVeryLargeSword"]);
        }

        [Fact]
        public void IsNotAddedAbilityTest()
        {
            Assert.True(HeroOverride.AddedAbilitiesByAbilityId.ContainsKey("FireBreath"));
            Assert.False(HeroOverride.AddedAbilitiesByAbilityId["FireBreath"].Add);
        }

        [Fact]
        public void IsNotValidWeaponTest()
        {
            Assert.True(HeroOverride.IsValidWeaponByWeaponId.ContainsKey("Ffffwwwwaaa-2.0"));
            Assert.False(HeroOverride.IsValidWeaponByWeaponId["Ffffwwwwaaa-2.0"]);
        }

        [Fact]
        public void LinkedAbilitiesTest()
        {
            Assert.Empty(HeroOverride.LinkedElementNamesByAbilityId);
        }

        [Fact]
        public void IsValidAbilityTest()
        {
            Assert.True(HeroOverride.IsValidAbilityByAbilityId.ContainsKey("AVeryLargeSword"));
            Assert.False(HeroOverride.IsValidAbilityByAbilityId["AVeryLargeSword"]);
        }

        [Fact]
        public void IsAddedAbilityTest()
        {
            Assert.True(HeroOverride.AddedAbilitiesByAbilityId.ContainsKey("FireBreath"));
            Assert.False(HeroOverride.AddedAbilitiesByAbilityId["FireBreath"].Add);
        }

        [Fact]
        public void IsAddedButtonAbilityTest()
        {
            Assert.False(HeroOverride.IsAddedAbilityByButtonId.ContainsKey("IceBlock"));
        }

        [Fact]
        public void IsValidWeaponTest()
        {
            Assert.False(HeroOverride.IsValidWeaponByWeaponId.ContainsKey("Ffffwwwwaaa"));
        }

        [Fact]
        public void HeroUnitTests()
        {
            Assert.Empty(HeroOverride.HeroUnits);
        }

        [Fact]
        public void ParentLinkedOverrideTests()
        {
            Assert.False(HeroOverride.ParentLinkOverride.Enabled);
            Assert.Equal(string.Empty, HeroOverride.ParentLinkOverride.ParentLink);
        }

        [Fact]
        public void HeroAbilSetTest()
        {
            Assert.Empty(HeroOverride.NewButtonValueByHeroAbilArrayButton);
        }
    }
}
