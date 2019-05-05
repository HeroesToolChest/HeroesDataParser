using HeroesData.Parser.Overrides.DataOverrides;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.HeroOverrideTest
{
    [TestClass]
    public class AlexstraszaHeroTests : OverrideBaseTests, IHeroOverride
    {
        private readonly string Hero = "Alexstrasza";

        public AlexstraszaHeroTests()
            : base()
        {
        }

        protected override string CHeroId => Hero;

        [TestMethod]
        public void CUnitOverrideTest()
        {
            Assert.IsFalse(HeroDataOverride.CUnitOverride.Enabled);
        }

        [TestMethod]
        public void EnergyOverrideTest()
        {
            Assert.IsTrue(HeroDataOverride.EnergyOverride.Enabled);
            Assert.AreEqual(0, HeroDataOverride.EnergyOverride.Energy);
        }

        [TestMethod]
        public void EnergyTypeOverrideTest()
        {
            Assert.IsTrue(HeroDataOverride.EnergyTypeOverride.Enabled);
            Assert.AreEqual("CrazyPills", HeroDataOverride.EnergyTypeOverride.EnergyType);
        }

        [TestMethod]
        public void NameOverrideTest()
        {
            Assert.IsFalse(HeroDataOverride.NameOverride.Enabled);
        }

        [TestMethod]
        public void HyperlinkIdOverrideTest()
        {
            Assert.IsFalse(HeroDataOverride.HyperlinkIdOverride.Enabled);
        }

        [TestMethod]
        public void IsNotValidAbilityTest()
        {
            Assert.IsTrue(HeroDataOverride.IsValidAbilityByAbilityId.ContainsKey("AVeryLargeSword"));
            Assert.IsFalse(HeroDataOverride.IsValidAbilityByAbilityId["AVeryLargeSword"]);
        }

        [TestMethod]
        public void IsNotAddedAbilityTest()
        {
            Assert.IsTrue(HeroDataOverride.AddedAbilityByAbilityId.ContainsKey("FireBreath"));
            Assert.IsFalse(HeroDataOverride.AddedAbilityByAbilityId["FireBreath"].IsAdded);
        }

        [TestMethod]
        public void IsNotValidWeaponTest()
        {
            Assert.IsTrue(HeroDataOverride.IsValidWeaponByWeaponId.ContainsKey("Ffffwwwwaaa-2.0"));
            Assert.IsFalse(HeroDataOverride.IsValidWeaponByWeaponId["Ffffwwwwaaa-2.0"]);
        }

        [TestMethod]
        public void IsValidAbilityTest()
        {
            Assert.IsTrue(HeroDataOverride.IsValidAbilityByAbilityId.ContainsKey("AVeryLargeSword"));
            Assert.IsFalse(HeroDataOverride.IsValidAbilityByAbilityId["AVeryLargeSword"]);
        }

        [TestMethod]
        public void IsAddedAbilityTest()
        {
            Assert.IsTrue(HeroDataOverride.AddedAbilityByAbilityId.ContainsKey("FireBreath"));
            Assert.IsFalse(HeroDataOverride.AddedAbilityByAbilityId["FireBreath"].IsAdded);
        }

        [TestMethod]
        public void IsAddedButtonAbilityTest()
        {
            Assert.IsFalse(HeroDataOverride.AddedAbilityByButtonId.Contains(new AddedButtonAbility() { ButtonId = "IceBlock" }));
        }

        [TestMethod]
        public void IsValidWeaponTest()
        {
            Assert.IsFalse(HeroDataOverride.IsValidWeaponByWeaponId.ContainsKey("Ffffwwwwaaa"));
        }

        [TestMethod]
        public void HeroUnitTests()
        {
            Assert.IsTrue(HeroDataOverride.HeroUnits.Count == 0);
        }

        [TestMethod]
        public void ParentLinkedOverrideTests()
        {
            Assert.IsFalse(HeroDataOverride.ParentLinkOverride.Enabled);
            Assert.AreEqual(string.Empty, HeroDataOverride.ParentLinkOverride.ParentLink);
        }

        [TestMethod]
        public void AbilityButtonNameOverrideTest()
        {
            Assert.IsTrue(HeroDataOverride.ButtonNameOverrideByAbilityButtonId.Count == 0);
        }
    }
}
