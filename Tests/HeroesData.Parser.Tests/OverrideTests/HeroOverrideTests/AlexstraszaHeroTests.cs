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
            Assert.IsFalse(HeroDataOverride.ParentLinkOverride.Enabled);
            Assert.AreEqual(string.Empty, HeroDataOverride.ParentLinkOverride.ParentLink);
        }
    }
}
