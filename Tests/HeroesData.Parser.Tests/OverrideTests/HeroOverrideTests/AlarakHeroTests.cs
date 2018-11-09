using HeroesData.Parser.UnitData.Overrides;
using Xunit;

namespace HeroesData.Parser.Tests.OverrideTests.HeroOverrideTest
{
    public class AlarakHeroTests : OverrideBaseTests, IHeroOverride
    {
        private readonly string Hero = "Alarak";

        public AlarakHeroTests()
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
            Assert.Equal("Ammo", HeroOverride.EnergyTypeOverride.EnergyType);
        }

        [Fact]
        public void IsAddedAbilityTest()
        {
            Assert.Empty(HeroOverride.AddedAbilitiesByAbilityId);
        }

        [Fact]
        public void IsValidAbilityTest()
        {
            Assert.Empty(HeroOverride.IsValidAbilityByAbilityId);
        }

        [Fact]
        public void IsValidWeaponTest()
        {
            Assert.Empty(HeroOverride.IsValidWeaponByWeaponId);
        }

        [Fact]
        public void NameOverrideTest()
        {
            Assert.False(HeroOverride.NameOverride.Enabled);
        }

        [Fact]
        public void LinkedAbilitiesTest()
        {
            Assert.False(HeroOverride.LinkedElementNamesByAbilityId.ContainsKey("AbathurBigAbaSlapSwing"));
            Assert.False(HeroOverride.LinkedElementNamesByAbilityId.ContainsKey("AbathurBigAbaMeteorLocust"));

            Assert.Empty(HeroOverride.LinkedElementNamesByAbilityId);
        }

        [Fact]
        public void ShortNameOverrideTest()
        {
            Assert.False(HeroOverride.ShortNameOverride.Enabled);
        }

        [Fact]
        public void HeroUnitTests()
        {
            Assert.Empty(HeroOverride.HeroUnits);
        }

        [Fact]
        public void ParentLinkedOverrideTests()
        {
            Assert.True(HeroOverride.ParentLinkOverride.Enabled);
            Assert.Equal(string.Empty, HeroOverride.ParentLinkOverride.ParentLink);
        }

        [Fact]
        public void HeroAbilSetTest()
        {
            Assert.False(HeroOverride.NewButtonValueByHeroAbilArrayButton.ContainsKey("SnapCollectionStore"));
        }
    }
}
