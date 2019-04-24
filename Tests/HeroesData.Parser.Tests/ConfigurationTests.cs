using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace HeroesData.Parser.Tests
{
    [TestClass]
    public class ConfigurationTests
    {
        private readonly Configuration Configuration;

        public ConfigurationTests()
        {
            Configuration = new Configuration();
            Configuration.Load();
        }

        [TestMethod]
        public void ConfigFileExistsTest()
        {
            Assert.IsTrue(Configuration.ConfigFileExists());
        }

        [TestMethod]
        public void GamestringDefaultValuesTests()
        {
            List<(string Part, string Value)> values = Configuration.GamestringDefaultValues("ModifyFraction").ToList();
            Assert.AreEqual("last", values[0].Part);
            Assert.AreEqual("1", values[0].Value);
        }

        [TestMethod]
        public void XmlElementTests()
        {
            List<string> list = Configuration.GamestringXmlElements("Effect").ToList();
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
            List<string> list = Configuration.AddDataXmlElementIds("CUnit").ToList();
            Assert.IsTrue(list.Contains("VolskayaVehicleGunner"));
            Assert.IsTrue(list.Contains("Chromie"));
            Assert.IsTrue(list.Contains("Tower"));

            Assert.IsFalse(list.Contains("PlasmaRifle"));
            Assert.IsFalse(list.Contains("Infernos"));

            list = Configuration.AddDataXmlElementIds("CHero").ToList();
            Assert.IsTrue(list.Contains("PlasmaRifle"));
            Assert.IsTrue(list.Contains("Infernos"));
            Assert.IsFalse(list.Contains("Infernos2"));
            Assert.IsFalse(list.Contains("Infernos3"));

            list = Configuration.RemoveDataXmlElementIds("CHero").ToList();
            Assert.IsFalse(list.Contains("PlasmaRifle"));
            Assert.IsFalse(list.Contains("Infernos"));
            Assert.IsTrue(list.Contains("Infernos2"));
            Assert.IsTrue(list.Contains("Infernos3"));
        }
    }
}
