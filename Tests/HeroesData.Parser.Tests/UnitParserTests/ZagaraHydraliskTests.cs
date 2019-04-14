using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            Assert.AreEqual("storm_ui_ingame_targetinfopanel_unit_zagara_hydralisk.dds", ZagaraHydralisk.TargetInfoPanelImageFileNames.First());
        }

        [TestMethod]
        public void ArmorTests()
        {
            Assert.AreEqual(0, ZagaraHydralisk.Armor.PhysicalArmor);
            Assert.AreEqual(0, ZagaraHydralisk.Armor.SpellArmor);
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
            Assert.AreEqual(2, ZagaraHydralisk.Weapons.Count);
            Assert.AreEqual(71, ZagaraHydralisk.Weapons[0].Damage);
            Assert.AreEqual(0.05, ZagaraHydralisk.Weapons[0].DamageScaling);
            Assert.AreEqual("Hydralisk Melee", ZagaraHydralisk.Weapons[0].Name);
            Assert.AreEqual(1, ZagaraHydralisk.Weapons[0].Period);
            Assert.AreEqual(0.5, ZagaraHydralisk.Weapons[0].Range);
            Assert.AreEqual("ZagaraHydraliskMelee", ZagaraHydralisk.Weapons[0].WeaponNameId);
        }
    }
}
