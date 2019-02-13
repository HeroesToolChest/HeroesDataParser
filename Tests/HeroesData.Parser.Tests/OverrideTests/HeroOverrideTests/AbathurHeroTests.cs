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
            Assert.AreEqual("Acceptable", HeroDataOverride.NameOverride.Name);
        }

        [TestMethod]
        public void ShortNameOverrideTest()
        {
            Assert.IsTrue(HeroDataOverride.ShortNameOverride.Enabled);
            Assert.AreEqual("Funzo", HeroDataOverride.ShortNameOverride.ShortName);
        }

        [TestMethod]
        public void IsValidAbilityTest()
        {
            Assert.IsTrue(HeroDataOverride.IsValidAbilityByAbilityId.ContainsKey("SpikeAbilityThingy"));
            Assert.IsTrue(HeroDataOverride.IsValidAbilityByAbilityId["SpikeAbilityThingy"]);
        }

        [TestMethod]
        public void IsAddedAbilityTest()
        {
            Assert.IsTrue(HeroDataOverride.AddedAbilityByAbilityId.ContainsKey("MindControl"));
            Assert.IsTrue(HeroDataOverride.AddedAbilityByAbilityId["MindControl"].IsAdded);
            Assert.AreEqual("MindControlButton", HeroDataOverride.AddedAbilityByAbilityId["MindControl"].ButtonName);
        }

        [TestMethod]
        public void IsAddedButtonAbilityTest()
        {
            Assert.IsTrue(HeroDataOverride.AddedAbilityByButtonId.Contains(("IceBlock", "StormButtonParent")));
        }

        [TestMethod]
        public void IsValidWeaponTest()
        {
            Assert.IsTrue(HeroDataOverride.IsValidWeaponByWeaponId.ContainsKey("SlapSlap"));
            Assert.IsTrue(HeroDataOverride.IsValidWeaponByWeaponId["SlapSlap"]);
        }

        [TestMethod]
        public void HeroUnitTests()
        {
            Assert.IsTrue(HeroDataOverride.HeroUnits.Contains("LittleLoco"));

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

        [TestMethod]
        public void AbilityButtonNameOverrideTest()
        {
            Assert.AreEqual("CarapaceCollection", HeroDataOverride.ButtonNameOverrideByAbilityButtonId[("CarapaceCollection", "CarapaceCollectionStore")]);
        }
    }
}
