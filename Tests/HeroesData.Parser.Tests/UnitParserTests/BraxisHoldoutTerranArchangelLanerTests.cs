﻿using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
            Assert.IsTrue(BraxisHoldoutTerranArchangelLaner.ContainsAbility("TerranArchangelLanerBulletstorm", StringComparison.OrdinalIgnoreCase));

            Ability terranArchangelLanerAbility = BraxisHoldoutTerranArchangelLaner.GetAbility(new AbilityTalentId("TerranArchangelLanerBulletstorm", "TerranArchangelBulletstorm")
            {
                AbilityType = AbilityTypes.Q,
            });
            Assert.AreEqual("Cooldown: 15 seconds", terranArchangelLanerAbility.Tooltip.Cooldown.CooldownTooltip.PlainText); // actually 12
            Assert.AreEqual("Bulletstorm", terranArchangelLanerAbility.Name);
            Assert.AreEqual("Unleash a hail of bullets in a line", terranArchangelLanerAbility.Tooltip.ShortTooltip.PlainText);
        }
    }
}
