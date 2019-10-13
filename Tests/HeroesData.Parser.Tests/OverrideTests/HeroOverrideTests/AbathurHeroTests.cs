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
            Assert.IsTrue(HeroDataOverride.CUnitOverride.enabled);
            Assert.AreEqual("HeroAbathur", HeroDataOverride.CUnitOverride.cUnit);
        }

        [TestMethod]
        public void EnergyOverrideTest()
        {
            Assert.IsTrue(HeroDataOverride.EnergyOverride.enabled);
            Assert.AreEqual(100, HeroDataOverride.EnergyOverride.energy);
        }

        [TestMethod]
        public void EnergyTypeOverrideTest()
        {
            Assert.IsTrue(HeroDataOverride.EnergyTypeOverride.enabled);
            Assert.AreEqual("Charge", HeroDataOverride.EnergyTypeOverride.energyType);
        }

        [TestMethod]
        public void NameOverrideTest()
        {
            Assert.IsTrue(HeroDataOverride.NameOverride.enabled);
            Assert.AreEqual("Acceptable", HeroDataOverride.NameOverride.value);
        }

        [TestMethod]
        public void HyperlinkIdOverrideTest()
        {
            Assert.IsTrue(HeroDataOverride.HyperlinkIdOverride.enabled);
            Assert.AreEqual("Funzo", HeroDataOverride.HyperlinkIdOverride.value);
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

            Assert.IsTrue(heroUnitOverride.EnergyTypeOverride.enabled);
            Assert.AreEqual("None", heroUnitOverride.EnergyTypeOverride.energyType);

            Assert.IsTrue(heroUnitOverride.EnergyOverride.enabled);
            Assert.AreEqual(0, heroUnitOverride.EnergyOverride.energy);
        }

        [TestMethod]
        public void ParentLinkedOverrideTests()
        {
            Assert.IsTrue(HeroDataOverride.ParentLinkOverride.enabled);
            Assert.AreEqual("TheSwarm", HeroDataOverride.ParentLinkOverride.parentLink);
        }
    }
}
