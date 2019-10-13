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
            Assert.IsFalse(HeroDataOverride.CUnitOverride.enabled);
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
            Assert.AreEqual("CrazyPills", HeroDataOverride.EnergyTypeOverride.energyType);
        }

        [TestMethod]
        public void NameOverrideTest()
        {
            Assert.IsFalse(HeroDataOverride.NameOverride.enabled);
        }

        [TestMethod]
        public void HyperlinkIdOverrideTest()
        {
            Assert.IsFalse(HeroDataOverride.HyperlinkIdOverride.enabled);
        }

        [TestMethod]
        public void IsNotValidWeaponTest()
        {
            Assert.IsTrue(HeroDataOverride.ContainsAddedWeapon("Ffffwwwwaaa-2.0"));
            Assert.IsFalse(HeroDataOverride.IsAddedWeapon("Ffffwwwwaaa-2.0"));
        }

        [TestMethod]
        public void IsValidWeaponTest()
        {
            Assert.IsFalse(HeroDataOverride.ContainsAddedWeapon("Ffffwwwwaaa"));
        }

        [TestMethod]
        public void HeroUnitTests()
        {
            Assert.IsTrue(HeroDataOverride.HeroUnitsCount == 0);
        }

        [TestMethod]
        public void ParentLinkedOverrideTests()
        {
            Assert.IsFalse(HeroDataOverride.ParentLinkOverride.enabled);
            Assert.AreEqual(string.Empty, HeroDataOverride.ParentLinkOverride.parentLink);
        }
    }
}
