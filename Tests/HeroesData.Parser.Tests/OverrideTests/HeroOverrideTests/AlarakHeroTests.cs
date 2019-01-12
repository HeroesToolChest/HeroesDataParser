using HeroesData.Parser.HeroData.Overrides;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.HeroOverrideTest
{
    [TestClass]
    public class AlarakHeroTests : OverrideBaseTests, IHeroOverride
    {
        private readonly string Hero = "Alarak";

        public AlarakHeroTests()
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
            Assert.AreEqual("Ammo", HeroOverride.EnergyTypeOverride.EnergyType);
        }

        [TestMethod]
        public void IsAddedAbilityTest()
        {
            Assert.IsTrue(HeroOverride.AddedAbilityByAbilityId.Count == 0);
        }

        [TestMethod]
        public void IsValidAbilityTest()
        {
            Assert.IsTrue(HeroOverride.IsValidAbilityByAbilityId.Count == 0);
        }

        [TestMethod]
        public void IsAddedButtonAbilityTest()
        {
            Assert.IsTrue(HeroOverride.AddedAbilityByButtonId.Contains(("IceBlock", string.Empty)));
        }

        [TestMethod]
        public void IsValidWeaponTest()
        {
            Assert.IsTrue(HeroOverride.IsValidWeaponByWeaponId.Count == 0);
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
        public void HeroUnitTests()
        {
            Assert.IsTrue(HeroOverride.HeroUnits.Count == 0);
        }

        [TestMethod]
        public void ParentLinkedOverrideTests()
        {
            Assert.IsTrue(HeroOverride.ParentLinkOverride.Enabled);
            Assert.AreEqual(string.Empty, HeroOverride.ParentLinkOverride.ParentLink);
        }

        [TestMethod]
        public void AbilityButtonNameOverrideTest()
        {
            Assert.IsFalse(HeroOverride.ButtonNameOverrideByAbilityButtonId.ContainsKey(("SnapCollection", "SnapCollectionStore")));
        }
    }
}
