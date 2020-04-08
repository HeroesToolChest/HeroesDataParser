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
        private const string _testDataFolder = "TestData";

        private readonly string _modsTestFolder = Path.Combine(_testDataFolder, "mods");
        private readonly string _overrideFileNameSuffix = "overrides-test";
        private readonly GameData _gameData;

        public XmlDataOverridersTests()
        {
            _gameData = new FileGameData(_modsTestFolder);
        }

        [TestMethod]
        public void LoadedDataOverridersNoBuildTest()
        {
            XmlDataOverriders xmlDataOverriders = XmlDataOverriders.Load(App.AssemblyPath, _gameData, _overrideFileNameSuffix);

            List<string> loadedOverrideFileNames = xmlDataOverriders.LoadedFileNames.ToList();

            Assert.AreEqual(3, xmlDataOverriders.Count);
            Assert.AreEqual("hero-overrides-test.xml", Path.GetFileName(loadedOverrideFileNames[0]));
        }

        [TestMethod]
        public void LoadedDataOverridersHasBuildTest()
        {
            XmlDataOverriders xmlDataOverriders = XmlDataOverriders.Load(App.AssemblyPath, _gameData, 12000, _overrideFileNameSuffix);

            List<string> loadedOverrideFileNames = xmlDataOverriders.LoadedFileNames.ToList();

            Assert.AreEqual(3, xmlDataOverriders.Count);
            Assert.AreEqual("hero-overrides-test_12000.xml", Path.GetFileName(loadedOverrideFileNames[0]));
        }

        [TestMethod]
        [DataRow(typeof(HeroDataParser))]
        public void LoadedDataOverridersGetOverrider(Type type)
        {
            XmlDataOverriders xmlDataOverriders = XmlDataOverriders.Load(App.AssemblyPath, _gameData, _overrideFileNameSuffix);
            IOverrideLoader overrideLoader = xmlDataOverriders.GetOverrider(type);

            Assert.IsNotNull(overrideLoader);
        }

        [TestMethod]
        public void LoadedDataOverridersGetOverriderIsNull()
        {
            XmlDataOverriders xmlDataOverriders = XmlDataOverriders.Load(App.AssemblyPath, _gameData, _overrideFileNameSuffix);
            IOverrideLoader overrideLoader = xmlDataOverriders.GetOverrider(typeof(XmlDataOverridersTests));

            Assert.IsNull(overrideLoader);
        }
    }
}
