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
            Assert.IsTrue(HeroDataOverride.CUnitOverride.enabled);
        }

        [TestMethod]
        public void EnergyOverrideTest()
        {
            Assert.IsTrue(HeroDataOverride.EnergyOverride.enabled);
            Assert.AreEqual(0, HeroDataOverride.EnergyOverride.energy);
        }

        [TestMethod]
        public void EnergyTypeOverrideTest()
        {
            Assert.IsTrue(HeroDataOverride.EnergyTypeOverride.enabled);
            Assert.AreEqual("Ammo", HeroDataOverride.EnergyTypeOverride.energyType);
        }

        [TestMethod]
        public void IsValidWeaponTest()
        {
            Assert.IsTrue(HeroDataOverride.AddedWeaponsCount == 0);
        }

        [TestMethod]
        public void NameOverrideTest()
        {
            Assert.IsTrue(HeroDataOverride.NameOverride.enabled);
        }

        [TestMethod]
        public void HyperlinkIdOverrideTest()
        {
            Assert.IsTrue(HeroDataOverride.HyperlinkIdOverride.enabled);
        }

        [TestMethod]
        public void HeroUnitTests()
        {
            Assert.IsTrue(HeroDataOverride.HeroUnitsCount == 0);
        }

        [TestMethod]
        public void ParentLinkedOverrideTests()
        {
            Assert.IsTrue(HeroDataOverride.ParentLinkOverride.enabled);
            Assert.AreEqual(string.Empty, HeroDataOverride.ParentLinkOverride.parentLink);
        }
    }
}
