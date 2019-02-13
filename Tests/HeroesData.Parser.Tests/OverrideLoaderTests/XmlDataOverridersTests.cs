using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.Parser.Tests.OverrideLoaderTests
{
    [TestClass]
    public class XmlDataOverridersTests
    {
        private const string TestDataFolder = "TestData";

        private readonly string ModsTestFolder = Path.Combine(TestDataFolder, "mods");
        private readonly string OverrideFileNameSuffix = "overrides-test";
        private readonly GameData GameData;

        public XmlDataOverridersTests()
        {
            GameData = new FileGameData(ModsTestFolder);
        }

        [TestMethod]
        public void LoadedDataOverridersNoBuildTest()
        {
            XmlDataOverriders xmlDataOverriders = XmlDataOverriders.Load(GameData, OverrideFileNameSuffix);

            List<string> loadedOverrideFileNames = xmlDataOverriders.LoadedFileNames.ToList();

            Assert.AreEqual(1, xmlDataOverriders.Count);
            Assert.AreEqual("hero-overrides-test.xml", Path.GetFileName(loadedOverrideFileNames[0]));
        }

        [TestMethod]
        public void LoadedDataOverridersHasBuildTest()
        {
            XmlDataOverriders xmlDataOverriders = XmlDataOverriders.Load(GameData, 12000, OverrideFileNameSuffix);

            List<string> loadedOverrideFileNames = xmlDataOverriders.LoadedFileNames.ToList();

            Assert.AreEqual(1, xmlDataOverriders.Count);
            Assert.AreEqual("hero-overrides-test_12000.xml", Path.GetFileName(loadedOverrideFileNames[0]));
        }

        [TestMethod]
        [DataRow(typeof(HeroDataParser))]
        public void LoadedDataOverridersGetOverrider(Type type)
        {
            XmlDataOverriders xmlDataOverriders = XmlDataOverriders.Load(GameData, OverrideFileNameSuffix);
            IOverrideLoader overrideLoader = xmlDataOverriders.GetOverrider(type);

            Assert.IsNotNull(overrideLoader);
        }

        [TestMethod]
        public void LoadedDataOverridersGetOverriderIsNull()
        {
            XmlDataOverriders xmlDataOverriders = XmlDataOverriders.Load(GameData, OverrideFileNameSuffix);
            IOverrideLoader overrideLoader = xmlDataOverriders.GetOverrider(typeof(XmlDataOverridersTests));

            Assert.IsNull(overrideLoader);
        }
    }
}
