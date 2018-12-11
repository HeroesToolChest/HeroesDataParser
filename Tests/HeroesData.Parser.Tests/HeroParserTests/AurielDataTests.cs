using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    [TestClass]
    public class AurielDataTests : HeroDataBaseTest
    {
        [TestMethod]
        public void BasicPropertiesTests()
        {
            Assert.AreEqual("SummonMount", HeroAuriel.MountLinkId);
        }

        [TestMethod]
        public void EnergyTests()
        {
            Assert.AreEqual(475, HeroAuriel.Energy.EnergyMax);
            Assert.AreEqual("Stored Energy", HeroAuriel.Energy.EnergyType);
        }

        [TestMethod]
        public void LifeTests()
        {
            Assert.AreEqual(1700, HeroAuriel.Life.LifeMax);
            Assert.AreEqual(3.539, HeroAuriel.Life.LifeRegenerationRate);
        }

        [TestMethod]
        public void RolesTests()
        {
            Assert.AreEqual(1, HeroAuriel.Roles.Count);
            Assert.AreEqual("Support", HeroAuriel.Roles[0]);
        }

        [TestMethod]
        public void AbilityEnergyTooltipTextTest()
        {
            Ability ability = HeroAuriel.Abilities["AurielSacredSweep"];
            Assert.IsTrue(string.IsNullOrEmpty(ability.Tooltip.Energy?.EnergyTooltip?.RawDescription));
        }
    }
}
