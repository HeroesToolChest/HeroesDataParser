using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace HeroesData.Parser.Tests
{
    [TestClass]
    public class ConfigurationTests
    {
        private readonly Configuration _configuration;

        public ConfigurationTests()
        {
            _configuration = new Configuration();
            _configuration.Load();
        }

        [TestMethod]
        public void ConfigFileExistsTest()
        {
            Assert.IsTrue(_configuration.ConfigFileExists());
        }

        [TestMethod]
        public void GamestringDefaultValuesTests()
        {
            List<(string Part, string Value)> values = _configuration.GamestringDefaultValues("ModifyFraction").ToList();
            Assert.AreEqual("last", values[0].Part);
            Assert.AreEqual("1", values[0].Value);
        }

        [TestMethod]
        public void XmlElementTests()
        {
            List<string> list = _configuration.GamestringXmlElements("Effect").ToList();
            Assert.IsTrue(list.Contains("CEffectModifyUnit"));
            Assert.IsTrue(list.Contains("CEffectModifyCatalogNumeric"));
            Assert.IsTrue(list.Contains("CEffectCreateUnit"));

            Assert.IsFalse(list.Contains("CValidatorUnitCompareTokenCount"));
            Assert.IsFalse(list.Contains("CAccumulatorDistance"));
            Assert.IsFalse(list.Contains("CArmor"));
        }

        [TestMethod]
        public void XmlElementIdsTests()
        {
            List<string> list = _configuration.AddDataXmlElementIds("CUnit").ToList();
            Assert.IsTrue(list.Contains("VolskayaVehicleGunner"));
            Assert.IsTrue(list.Contains("Chromie"));
            Assert.IsTrue(list.Contains("Tower"));

            Assert.IsFalse(list.Contains("PlasmaRifle"));
            Assert.IsFalse(list.Contains("Infernos"));

            list = _configuration.AddDataXmlElementIds("CHero").ToList();
            Assert.IsTrue(list.Contains("PlasmaRifle"));
            Assert.IsTrue(list.Contains("Infernos"));
            Assert.IsFalse(list.Contains("Infernos2"));
            Assert.IsFalse(list.Contains("Infernos3"));

            list = _configuration.RemoveDataXmlElementIds("CHero").ToList();
            Assert.IsFalse(list.Contains("PlasmaRifle"));
            Assert.IsFalse(list.Contains("Infernos"));
            Assert.IsTrue(list.Contains("Infernos2"));
            Assert.IsTrue(list.Contains("Infernos3"));
        }

        [TestMethod]
        public void ContainsDeadImageFileNameTests()
        {
            Assert.IsTrue(_configuration.ContainsDeadImageFileName("hud_icon_teammapmechanic_tribute.dds"));
            Assert.IsFalse(_configuration.ContainsDeadImageFileName("asdf.dds"));
        }

        [TestMethod]
        public void ContainsIgnorableExtraAbilityTests()
        {
            Assert.IsTrue(_configuration.ContainsIgnorableExtraAbility("Attack"));
            Assert.IsFalse(_configuration.ContainsIgnorableExtraAbility("asdf"));
        }
    }
}
