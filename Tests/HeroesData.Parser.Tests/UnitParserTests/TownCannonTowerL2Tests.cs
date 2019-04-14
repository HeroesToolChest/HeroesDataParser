using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class TownCannonTowerL2Tests : UnitParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual(3, TownCannonTowerL2.Attributes.Count);
            Assert.IsTrue(TownCannonTowerL2.Attributes.Contains("Tower"));
            Assert.IsTrue(TownCannonTowerL2.Attributes.Contains("Structure"));
            Assert.IsTrue(TownCannonTowerL2.Attributes.Contains("AITargetableStructure"));
            Assert.AreEqual("TownCannonTowerL2", TownCannonTowerL2.CUnitId);
            Assert.AreEqual("Structure", TownCannonTowerL2.DamageType);
            Assert.IsNull(TownCannonTowerL2.Description?.RawDescription);
            Assert.AreEqual(0, TownCannonTowerL2.HeroDescriptors.Count);
            Assert.AreEqual("TownCannonTowerL2", TownCannonTowerL2.HyperlinkId);
            Assert.AreEqual("TownCannonTowerL2", TownCannonTowerL2.Id);
            Assert.AreEqual(0, TownCannonTowerL2.InnerRadius);
            Assert.AreEqual("Cannon Tower", TownCannonTowerL2.Name);
            Assert.AreEqual(1, TownCannonTowerL2.Radius);
            Assert.AreEqual(10, TownCannonTowerL2.Sight);
            Assert.AreEqual(0, TownCannonTowerL2.Speed);
            Assert.IsTrue(TownCannonTowerL2.TargetInfoPanelImageFileNames.Count == 3);
        }

        [TestMethod]
        public void ArmorTests()
        {
            Assert.AreEqual(0, TownCannonTowerL2.Armor.PhysicalArmor);
            Assert.AreEqual(0, TownCannonTowerL2.Armor.SpellArmor);
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
            Assert.AreEqual(1, TownCannonTowerL2.Weapons.Count);
            Assert.AreEqual(250, TownCannonTowerL2.Weapons[0].Damage);
            Assert.AreEqual(0, TownCannonTowerL2.Weapons[0].DamageScaling);
            Assert.AreEqual(string.Empty, TownCannonTowerL2.Weapons[0].Name);
            Assert.AreEqual(1, TownCannonTowerL2.Weapons[0].Period);
            Assert.AreEqual(7.75, TownCannonTowerL2.Weapons[0].Range);
            Assert.AreEqual("GuardTowerL2Weapon", TownCannonTowerL2.Weapons[0].WeaponNameId);
        }
    }
}
