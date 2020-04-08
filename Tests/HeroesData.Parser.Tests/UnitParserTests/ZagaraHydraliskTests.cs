using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class ZagaraHydraliskTests : UnitParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual(1, ZagaraHydralisk.Attributes.Count);
            Assert.IsTrue(ZagaraHydralisk.Attributes.Contains("Summoned"));
            Assert.AreEqual("ZagaraHydralisk", ZagaraHydralisk.CUnitId);
            Assert.AreEqual("Summon", ZagaraHydralisk.DamageType);
            Assert.IsNull(ZagaraHydralisk.Description?.RawDescription);
            Assert.AreEqual(0, ZagaraHydralisk.HeroDescriptors.Count);
            Assert.AreEqual("ZagaraHydralisk", ZagaraHydralisk.HyperlinkId);
            Assert.AreEqual("ZagaraHydralisk", ZagaraHydralisk.Id);
            Assert.AreEqual(0.375, ZagaraHydralisk.InnerRadius);
            Assert.AreEqual("Hydralisk", ZagaraHydralisk.Name);
            Assert.AreEqual(0.625, ZagaraHydralisk.Radius);
            Assert.AreEqual(9, ZagaraHydralisk.Sight);
            Assert.AreEqual(4, ZagaraHydralisk.Speed);
            Assert.AreEqual("storm_ui_ingame_targetinfopanel_unit_zagara_hydralisk.dds", ZagaraHydralisk.UnitPortrait.TargetInfoPanelFileName);
        }

        [TestMethod]
        public void ArmorTests()
        {
            Assert.AreEqual(null, ZagaraHydralisk.Armor.FirstOrDefault()?.BasicArmor);
            Assert.AreEqual(null, ZagaraHydralisk.Armor.FirstOrDefault()?.AbilityArmor);
        }

        [TestMethod]
        public void LifeTests()
        {
            Assert.AreEqual(450, ZagaraHydralisk.Life.LifeMax);
            Assert.AreEqual(0, ZagaraHydralisk.Life.LifeRegenerationRate);
            Assert.AreEqual(0, ZagaraHydralisk.Life.LifeRegenerationRateScaling);
            Assert.AreEqual(0.045, ZagaraHydralisk.Life.LifeScaling);
        }

        [TestMethod]
        public void WeaponTests()
        {
            List<UnitWeapon> unitWeapons = ZagaraHydralisk.Weapons.ToList();
            Assert.AreEqual(2, unitWeapons.Count);
            Assert.AreEqual(71, unitWeapons[0].Damage);
            Assert.AreEqual(0.05, unitWeapons[0].DamageScaling);
            Assert.AreEqual("Hydralisk Melee", unitWeapons[0].Name);
            Assert.AreEqual(1, unitWeapons[0].Period);
            Assert.AreEqual(0.5, unitWeapons[0].Range);
            Assert.AreEqual("ZagaraHydraliskMelee", unitWeapons[0].WeaponNameId);
        }
    }
}
