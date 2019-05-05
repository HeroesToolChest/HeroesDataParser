using HeroesData.Parser.Overrides.DataOverrides;
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
            Assert.AreEqual("Ammo", HeroDataOverride.EnergyTypeOverride.EnergyType);
        }

        [TestMethod]
        public void IsAddedAbilityTest()
        {
            Assert.IsTrue(HeroDataOverride.AddedAbilityByAbilityId.Count == 0);
        }

        [TestMethod]
        public void IsValidAbilityTest()
        {
            Assert.IsTrue(HeroDataOverride.IsValidAbilityByAbilityId.Count == 0);
        }

        [TestMethod]
        public void IsAddedButtonAbilityTest()
        {
            Assert.IsTrue(HeroDataOverride.AddedAbilityByButtonId.Contains(new AddedButtonAbility() { ButtonId = "IceBlock" }));
        }

        [TestMethod]
        public void IsValidWeaponTest()
        {
            Assert.IsTrue(HeroDataOverride.IsValidWeaponByWeaponId.Count == 0);
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
        public void HeroUnitTests()
        {
            Assert.IsTrue(HeroDataOverride.HeroUnits.Count == 0);
        }

        [TestMethod]
        public void ParentLinkedOverrideTests()
        {
            Assert.IsTrue(HeroDataOverride.ParentLinkOverride.Enabled);
            Assert.AreEqual(string.Empty, HeroDataOverride.ParentLinkOverride.ParentLink);
        }

        [TestMethod]
        public void AbilityButtonNameOverrideTest()
        {
            Assert.IsFalse(HeroDataOverride.ButtonNameOverrideByAbilityButtonId.ContainsKey(("SnapCollection", "SnapCollectionStore")));
        }
    }
}
