using HeroesData.Parser.Overrides.DataOverrides;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.OverrideTests.HeroOverrideTest
{
    [TestClass]
    public class AbathurHeroTests : OverrideBaseTests, IHeroOverride
    {
        private readonly string Hero = "Abathur";

        public AbathurHeroTests()
            : base()
        {
        }

        protected override string CHeroId => Hero;

        [TestMethod]
        public void CUnitOverrideTest()
        {
            Assert.IsTrue(HeroDataOverride.CUnitOverride.Enabled);
            Assert.AreEqual("HeroAbathur", HeroDataOverride.CUnitOverride.CUnit);
        }

        [TestMethod]
        public void EnergyOverrideTest()
        {
            Assert.IsTrue(HeroDataOverride.EnergyOverride.Enabled);
            Assert.AreEqual(100, HeroDataOverride.EnergyOverride.Energy);
        }

        [TestMethod]
        public void EnergyTypeOverrideTest()
        {
            Assert.IsTrue(HeroDataOverride.EnergyTypeOverride.Enabled);
            Assert.AreEqual("Charge", HeroDataOverride.EnergyTypeOverride.EnergyType);
        }

        [TestMethod]
        public void NameOverrideTest()
        {
            Assert.IsTrue(HeroDataOverride.NameOverride.Enabled);
            Assert.AreEqual("Acceptable", HeroDataOverride.NameOverride.Value);
        }

        [TestMethod]
        public void HyperlinkIdOverrideTest()
        {
            Assert.IsTrue(HeroDataOverride.HyperlinkIdOverride.Enabled);
            Assert.AreEqual("Funzo", HeroDataOverride.HyperlinkIdOverride.Value);
        }

        [TestMethod]
        public void IsValidWeaponTest()
        {
            Assert.IsTrue(HeroDataOverride.ContainsAddedWeapon("SlapSlap"));
            Assert.IsTrue(HeroDataOverride.IsAddedWeapon("SlapSlap"));
        }

        [TestMethod]
        public void HeroUnitTests()
        {
            Assert.IsTrue(HeroDataOverride.ContainsHeroUnit("LittleLoco"));

            HeroDataOverride heroUnitOverride = HeroOverrideLoader.GetOverride("LittleLoco");

            Assert.IsTrue(heroUnitOverride.EnergyTypeOverride.Enabled);
            Assert.AreEqual("None", heroUnitOverride.EnergyTypeOverride.EnergyType);

            Assert.IsTrue(heroUnitOverride.EnergyOverride.Enabled);
            Assert.AreEqual(0, heroUnitOverride.EnergyOverride.Energy);
        }

        [TestMethod]
        public void ParentLinkedOverrideTests()
        {
            Assert.IsTrue(HeroDataOverride.ParentLinkOverride.Enabled);
            Assert.AreEqual("TheSwarm", HeroDataOverride.ParentLinkOverride.ParentLink);
        }
    }
}
