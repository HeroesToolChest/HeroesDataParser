using HeroesData.Parser.UnitData.Overrides;
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
            Assert.IsTrue(HeroOverride.CUnitOverride.Enabled);
            Assert.AreEqual("HeroAbathur", HeroOverride.CUnitOverride.CUnit);
        }

        [TestMethod]
        public void EnergyOverrideTest()
        {
            Assert.IsTrue(HeroOverride.EnergyOverride.Enabled);
            Assert.AreEqual(100, HeroOverride.EnergyOverride.Energy);
        }

        [TestMethod]
        public void EnergyTypeOverrideTest()
        {
            Assert.IsTrue(HeroOverride.EnergyTypeOverride.Enabled);
            Assert.AreEqual("Charge", HeroOverride.EnergyTypeOverride.EnergyType);
        }

        [TestMethod]
        public void NameOverrideTest()
        {
            Assert.IsTrue(HeroOverride.NameOverride.Enabled);
            Assert.AreEqual("Acceptable", HeroOverride.NameOverride.Name);
        }

        [TestMethod]
        public void ShortNameOverrideTest()
        {
            Assert.IsTrue(HeroOverride.ShortNameOverride.Enabled);
            Assert.AreEqual("Funzo", HeroOverride.ShortNameOverride.ShortName);
        }

        [TestMethod]
        public void IsValidAbilityTest()
        {
            Assert.IsTrue(HeroOverride.IsValidAbilityByAbilityId.ContainsKey("SpikeAbilityThingy"));
            Assert.IsTrue(HeroOverride.IsValidAbilityByAbilityId["SpikeAbilityThingy"]);
        }

        [TestMethod]
        public void IsAddedAbilityTest()
        {
            Assert.IsTrue(HeroOverride.AddedAbilityByAbilityId.ContainsKey("MindControl"));
            Assert.IsTrue(HeroOverride.AddedAbilityByAbilityId["MindControl"].IsAdded);
            Assert.AreEqual("MindControlButton", HeroOverride.AddedAbilityByAbilityId["MindControl"].ButtonName);
        }

        [TestMethod]
        public void IsAddedButtonAbilityTest()
        {
            Assert.IsTrue(HeroOverride.AddedAbilityByButtonId.Contains(("IceBlock", "StormButtonParent")));
        }

        [TestMethod]
        public void IsValidWeaponTest()
        {
            Assert.IsTrue(HeroOverride.IsValidWeaponByWeaponId.ContainsKey("SlapSlap"));
            Assert.IsTrue(HeroOverride.IsValidWeaponByWeaponId["SlapSlap"]);
        }

        [TestMethod]
        public void HeroUnitTests()
        {
            Assert.IsTrue(HeroOverride.HeroUnits.Contains("LittleLoco"));

            HeroOverride heroOverride = OverrideData.HeroOverride("LittleLoco");

            Assert.IsTrue(heroOverride.EnergyTypeOverride.Enabled);
            Assert.AreEqual("None", heroOverride.EnergyTypeOverride.EnergyType);

            Assert.IsTrue(heroOverride.EnergyOverride.Enabled);
            Assert.AreEqual(0, heroOverride.EnergyOverride.Energy);
        }

        [TestMethod]
        public void ParentLinkedOverrideTests()
        {
            Assert.IsTrue(HeroOverride.ParentLinkOverride.Enabled);
            Assert.AreEqual("TheSwarm", HeroOverride.ParentLinkOverride.ParentLink);
        }

        [TestMethod]
        public void AbilityButtonNameOverrideTest()
        {
            Assert.AreEqual("CarapaceCollection", HeroOverride.ButtonNameOverrideByAbilityId["CarapaceCollection"]);
        }
    }
}
