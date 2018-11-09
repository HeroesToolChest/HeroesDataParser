using HeroesData.Parser.UnitData.Overrides;
using Xunit;

namespace HeroesData.Parser.Tests.OverrideTests.HeroOverrideTest
{
    public class AbathurHeroTests : OverrideBaseTests, IHeroOverride
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
            Assert.Equal("Charge", HeroOverride.EnergyTypeOverride.EnergyType);
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

        [Fact]
        public void IsValidAbilityTest()
        {
            Assert.True(HeroOverride.IsValidAbilityByAbilityId.ContainsKey("SpikeAbilityThingy"));
            Assert.True(HeroOverride.IsValidAbilityByAbilityId["SpikeAbilityThingy"]);
        }

        [Fact]
        public void IsAddedAbilityTest()
        {
            Assert.True(HeroOverride.AddedAbilitiesByAbilityId.ContainsKey("MindControl"));
            Assert.True(HeroOverride.AddedAbilitiesByAbilityId["MindControl"].Add);
            Assert.Equal("MindControlButton", HeroOverride.AddedAbilitiesByAbilityId["MindControl"].Button);
        }

        [Fact]
        public void IsValidWeaponTest()
        {
            Assert.True(HeroOverride.IsValidWeaponByWeaponId.ContainsKey("SlapSlap"));
            Assert.True(HeroOverride.IsValidWeaponByWeaponId["SlapSlap"]);
        }

        [Fact]
        public void LinkedAbilitiesTest()
        {
            Assert.True(HeroOverride.LinkedElementNamesByAbilityId.ContainsKey("AbathurBigAbaSlapSwing"));
            Assert.Equal("CAbilEffectTarget", HeroOverride.LinkedElementNamesByAbilityId["AbathurBigAbaSlapSwing"]);

            Assert.True(HeroOverride.LinkedElementNamesByAbilityId.ContainsKey("AbathurBigAbaMeteorLocust"));
            Assert.Equal("CAbilEffectTarget", HeroOverride.LinkedElementNamesByAbilityId["AbathurBigAbaMeteorLocust"]);
        }

        [Fact]
        public void HeroUnitTests()
        {
            Assert.Contains("LittleLoco", HeroOverride.HeroUnits);

            HeroOverride heroOverride = OverrideData.HeroOverride("LittleLoco");

            Assert.True(heroOverride.EnergyTypeOverride.Enabled);
            Assert.Equal("None", heroOverride.EnergyTypeOverride.EnergyType);

            Assert.True(heroOverride.EnergyOverride.Enabled);
            Assert.Equal(0, heroOverride.EnergyOverride.Energy);
        }

        [Fact]
        public void ParentLinkedOverrideTests()
        {
            Assert.True(HeroOverride.ParentLinkOverride.Enabled);
            Assert.Equal("TheSwarm", HeroOverride.ParentLinkOverride.ParentLink);
        }

        [Fact]
        public void HeroAbilSetTest()
        {
            Assert.Equal("CarapaceCollection", HeroOverride.NewButtonValueByHeroAbilArrayButton["CarapaceCollectionStore"]);
        }
    }
}
