using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class TownCannonTowerL2Tests : UnitParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual(3, TownCannonTowerL2.Attributes.Count());
            Assert.IsTrue(TownCannonTowerL2.Attributes.Contains("Tower"));
            Assert.IsTrue(TownCannonTowerL2.Attributes.Contains("Structure"));
            Assert.IsTrue(TownCannonTowerL2.Attributes.Contains("AITargetableStructure"));
            Assert.AreEqual("TownCannonTowerL2", TownCannonTowerL2.CUnitId);
            Assert.AreEqual("Structure", TownCannonTowerL2.DamageType);
            Assert.IsNotNull(TownCannonTowerL2.Description?.RawDescription);
            Assert.AreEqual(0, TownCannonTowerL2.HeroDescriptors.Count());
            Assert.AreEqual("TownCannonTowerL2", TownCannonTowerL2.HyperlinkId);
            Assert.AreEqual("TownCannonTowerL2", TownCannonTowerL2.Id);
            Assert.AreEqual(0, TownCannonTowerL2.InnerRadius);
            Assert.AreEqual("Cannon Tower", TownCannonTowerL2.Name);
            Assert.AreEqual(1, TownCannonTowerL2.Radius);
            Assert.AreEqual(10, TownCannonTowerL2.Sight);
            Assert.AreEqual(0, TownCannonTowerL2.Speed);
        }

        [TestMethod]
        public void ArmorTests()
        {
            Assert.AreEqual(null, TownCannonTowerL2.Armor.FirstOrDefault()?.BasicArmor);
            Assert.AreEqual(null, TownCannonTowerL2.Armor.FirstOrDefault()?.AbilityArmor);
        }

        [TestMethod]
        public void LifeTests()
        {
            Assert.AreEqual(4300, TownCannonTowerL2.Life.LifeMax);
            Assert.AreEqual(0, TownCannonTowerL2.Life.LifeRegenerationRate);
            Assert.AreEqual(0, TownCannonTowerL2.Life.LifeRegenerationRateScaling);
            Assert.AreEqual(0, TownCannonTowerL2.Life.LifeScaling);
        }

        [TestMethod]
        public void WeaponTests()
        {
            List<UnitWeapon> unitWeapons = TownCannonTowerL2.Weapons.ToList();
            Assert.AreEqual(1, unitWeapons.Count);
            Assert.AreEqual(250, unitWeapons[0].Damage);
            Assert.AreEqual(0, unitWeapons[0].DamageScaling);
            Assert.AreEqual(string.Empty, unitWeapons[0].Name);
            Assert.AreEqual(1, unitWeapons[0].Period);
            Assert.AreEqual(7.75, unitWeapons[0].Range);
            Assert.AreEqual("GuardTowerL2Weapon", unitWeapons[0].WeaponNameId);
        }

        [TestMethod]
        public void DetectorAbilityActionTest()
        {
            Ability ability = TownCannonTowerL2.GetFirstAbility("Detector");
            Assert.AreEqual("Detector", ability.AbilityTalentId.ButtonId);
            Assert.AreEqual(AbilityType.Stop, ability.AbilityType);
            Assert.IsTrue(ability.IsPassive);
        }
    }
}
