using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class BraxisHoldoutTerranArchangelLanerTests : UnitParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("braxisholdoutdata-TerranArchangelLaner", BraxisHoldoutTerranArchangelLaner.Id);
        }

        [TestMethod]
        public void WeaponsTests()
        {
            UnitWeapon weapon1 = BraxisHoldoutTerranArchangelLaner.Weapons.ToList()[0];

            Assert.AreEqual(4, weapon1.Range);
            Assert.AreEqual(0.0625, weapon1.Period);
            Assert.AreEqual(25, weapon1.Damage);
            Assert.AreEqual("Minion", weapon1.AttributeFactors.First().Type);
            Assert.AreEqual(1, weapon1.AttributeFactors.First().Value);

            UnitWeapon weapon2 = BraxisHoldoutTerranArchangelLaner.Weapons.ToList()[0];

            Assert.AreEqual(4, weapon2.Range);
            Assert.AreEqual(0.0625, weapon2.Period);
            Assert.AreEqual(25, weapon2.Damage);
            Assert.AreEqual("Minion", weapon2.AttributeFactors.First().Type);
            Assert.AreEqual(1, weapon2.AttributeFactors.First().Value);
        }

        [TestMethod]
        public void LifePropertiesTests()
        {
            Assert.AreEqual(15500, BraxisHoldoutTerranArchangelLaner.Life.LifeMax);
        }

        [TestMethod]
        public void AbilitiesTests()
        {
            Assert.IsTrue(BraxisHoldoutTerranArchangelLaner.Abilities.ContainsKey("TerranArchangelLanerBulletstorm"));

            Ability terranArchangelLanerAbility = BraxisHoldoutTerranArchangelLaner.Abilities["TerranArchangelLanerBulletstorm"];
            Assert.AreEqual("Cooldown: 12 seconds", terranArchangelLanerAbility.Tooltip.Cooldown.CooldownTooltip.PlainText);
            Assert.AreEqual("Bulletstorm", terranArchangelLanerAbility.Name);
            Assert.AreEqual("Unleash a hail of bullets in a line", terranArchangelLanerAbility.Tooltip.ShortTooltip.PlainText);
        }
    }
}
