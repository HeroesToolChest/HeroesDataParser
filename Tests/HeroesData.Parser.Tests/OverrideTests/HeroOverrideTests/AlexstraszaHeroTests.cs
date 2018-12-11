using HeroesData.Parser.UnitData.Overrides;
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
            Assert.IsFalse(HeroOverride.CUnitOverride.Enabled);
        }

        [TestMethod]
        public void EnergyOverrideTest()
        {
            Assert.IsTrue(HeroOverride.EnergyOverride.Enabled);
            Assert.AreEqual(0, HeroOverride.EnergyOverride.Energy);
        }

        [TestMethod]
        public void EnergyTypeOverrideTest()
        {
            Assert.IsTrue(HeroOverride.EnergyTypeOverride.Enabled);
            Assert.AreEqual("CrazyPills", HeroOverride.EnergyTypeOverride.EnergyType);
        }

        [TestMethod]
        public void NameOverrideTest()
        {
            Assert.IsFalse(HeroOverride.NameOverride.Enabled);
        }

        [TestMethod]
        public void ShortNameOverrideTest()
        {
            Assert.IsFalse(HeroOverride.ShortNameOverride.Enabled);
        }

        [TestMethod]
        public void IsNotValidAbilityTest()
        {
            Assert.IsTrue(HeroOverride.IsValidAbilityByAbilityId.ContainsKey("AVeryLargeSword"));
            Assert.IsFalse(HeroOverride.IsValidAbilityByAbilityId["AVeryLargeSword"]);
        }

        [TestMethod]
        public void IsNotAddedAbilityTest()
        {
            Assert.IsTrue(HeroOverride.AddedAbilitiesByAbilityId.ContainsKey("FireBreath"));
            Assert.IsFalse(HeroOverride.AddedAbilitiesByAbilityId["FireBreath"].Add);
        }

        [TestMethod]
        public void IsNotValidWeaponTest()
        {
            Assert.IsTrue(HeroOverride.IsValidWeaponByWeaponId.ContainsKey("Ffffwwwwaaa-2.0"));
            Assert.IsFalse(HeroOverride.IsValidWeaponByWeaponId["Ffffwwwwaaa-2.0"]);
        }

        [TestMethod]
        public void LinkedAbilitiesTest()
        {
            Assert.IsTrue(HeroOverride.LinkedElementNamesByAbilityId.Count == 0);
        }

        [TestMethod]
        public void IsValidAbilityTest()
        {
            Assert.IsTrue(HeroOverride.IsValidAbilityByAbilityId.ContainsKey("AVeryLargeSword"));
            Assert.IsFalse(HeroOverride.IsValidAbilityByAbilityId["AVeryLargeSword"]);
        }

        [TestMethod]
        public void IsAddedAbilityTest()
        {
            Assert.IsTrue(HeroOverride.AddedAbilitiesByAbilityId.ContainsKey("FireBreath"));
            Assert.IsFalse(HeroOverride.AddedAbilitiesByAbilityId["FireBreath"].Add);
        }

        [TestMethod]
        public void IsAddedButtonAbilityTest()
        {
            Assert.IsFalse(HeroOverride.AddedAbilitiesByButtonId.Contains("IceBlock"));
        }

        [TestMethod]
        public void IsValidWeaponTest()
        {
            Assert.IsFalse(HeroOverride.IsValidWeaponByWeaponId.ContainsKey("Ffffwwwwaaa"));
        }

        [TestMethod]
        public void HeroUnitTests()
        {
            Assert.IsTrue(HeroOverride.HeroUnits.Count == 0);
        }

        [TestMethod]
        public void ParentLinkedOverrideTests()
        {
            Assert.IsFalse(HeroOverride.ParentLinkOverride.Enabled);
            Assert.AreEqual(string.Empty, HeroOverride.ParentLinkOverride.ParentLink);
        }

        [TestMethod]
        public void HeroAbilSetTest()
        {
            Assert.IsTrue(HeroOverride.NewButtonValueByHeroAbilArrayButton.Count == 0);
        }
    }
}
