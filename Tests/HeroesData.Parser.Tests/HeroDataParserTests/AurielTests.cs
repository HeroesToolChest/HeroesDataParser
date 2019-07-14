using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class AurielTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void BasicPropertiesTests()
        {
            Assert.AreEqual("Healer", HeroAuriel.ExpandedRole);
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
            Assert.AreEqual(1, HeroAuriel.Roles.Count());
            Assert.AreEqual("Support", HeroAuriel.Roles.ToList()[0]);
        }

        [TestMethod]
        public void AbilityEnergyTooltipTextTest()
        {
            Ability ability = HeroAuriel.GetAbility("AurielSacredSweep");
            Assert.IsTrue(string.IsNullOrEmpty(ability.Tooltip.Energy?.EnergyTooltip?.RawDescription));
        }
    }
}
