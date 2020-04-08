using HeroesData.Parser.Overrides.DataOverrides;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.HeroOverrideTest
{
    [TestClass]
    public class AlarakHeroTests : OverrideBaseTests, IHeroOverride
    {
        private readonly string _hero = "Alarak";

        public AlarakHeroTests()
            : base()
        {
        }

        protected override string CHeroId => _hero;

        [TestMethod]
        public void CUnitOverrideTest()
        {
            Assert.IsTrue(HeroDataOverride.CUnitOverride.Enabled);
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
        public void IsValidWeaponTest()
        {
            Assert.IsTrue(HeroDataOverride.AddedWeaponsCount == 0);
        }

        [TestMethod]
        public void NameOverrideTest()
        {
            Assert.IsTrue(HeroDataOverride.NameOverride.Enabled);
        }

        [TestMethod]
        public void HyperlinkIdOverrideTest()
        {
            Assert.IsTrue(HeroDataOverride.HyperlinkIdOverride.Enabled);
        }

        [TestMethod]
        public void HeroUnitTests()
        {
            Assert.IsTrue(HeroDataOverride.HeroUnitsCount == 0);
        }

        [TestMethod]
        public void ParentLinkedOverrideTests()
        {
            Assert.IsTrue(HeroDataOverride.ParentLinkOverride.Enabled);
            Assert.AreEqual(string.Empty, HeroDataOverride.ParentLinkOverride.ParentLink);
        }
    }
}
