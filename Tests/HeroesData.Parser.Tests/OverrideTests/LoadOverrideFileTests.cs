using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.UnitData.Overrides;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace HeroesData.Parser.Tests.OverrideTests
{
    [TestClass]
    public class LoadOverrideFileTests
    {
        private const string TestDataFolder = "TestData";
        private readonly string ModsTestFolder = Path.Combine(TestDataFolder, "mods");
        private readonly string HeroOverrideTest = "HeroOverrideTest.xml";
        private readonly string HeroOverrideBuild12345Test = "HeroOverrideTest_12345.xml";
        private readonly GameData GameData;

        public LoadOverrideFileTests()
        {
            GameData = new FileGameData(ModsTestFolder);
        }

        [TestMethod]
        public void LoadOverrideFileTest()
        {
            OverrideData overrideData = OverrideData.Load(GameData, HeroOverrideTest);
            Assert.IsNotNull(overrideData);

            Assert.AreEqual(4, overrideData.Count); // LittleLoco counts as one
            Assert.AreEqual(HeroOverrideTest, overrideData.HeroDataOverrideXmlFile);
        }

        [TestMethod]
        public void LoadOverrideHasBuildInFileTest()
        {
            OverrideData overrideData = OverrideData.Load(GameData, 12345, HeroOverrideBuild12345Test);
            Assert.IsNotNull(overrideData);

            Assert.AreEqual(1, overrideData.Count);
            Assert.AreEqual(HeroOverrideBuild12345Test, overrideData.HeroDataOverrideXmlFile);
        }

        [TestMethod]
        public void LoadOverrideHasBuildOpenNextLowestBuildTest()
        {
            OverrideData overrideData = OverrideData.Load(GameData, 11500, HeroOverrideTest);
            Assert.IsNotNull(overrideData);

            Assert.AreEqual(7, overrideData.Count);
            Assert.AreEqual("HeroOverrideTest_11000.xml", overrideData.HeroDataOverrideXmlFile);
        }

        [TestMethod]
        public void LoadOverrideHasBuildOpenNextLowestBuild2Test()
        {
            OverrideData overrideData = OverrideData.Load(GameData, 12100, HeroOverrideTest);
            Assert.IsNotNull(overrideData);

            Assert.AreEqual(11, overrideData.Count);
            Assert.AreEqual("HeroOverrideTest_12000.xml", overrideData.HeroDataOverrideXmlFile);
        }

        [TestMethod]
        public void LoadOverrideHasBuildLowerThanLowTest()
        {
            OverrideData overrideData = OverrideData.Load(GameData, 1000, HeroOverrideTest);
            Assert.IsNotNull(overrideData);

            Assert.AreEqual(7, overrideData.Count);
            Assert.AreEqual("HeroOverrideTest_11000.xml", overrideData.HeroDataOverrideXmlFile);
        }

        [TestMethod]
        public void LoadOverrideHasBuildHighestDefaultBuildTest()
        {
            OverrideData overrideData = OverrideData.Load(GameData, 13000, HeroOverrideTest);
            Assert.IsNotNull(overrideData);

            Assert.AreEqual(4, overrideData.Count);
            Assert.AreEqual(HeroOverrideTest, overrideData.HeroDataOverrideXmlFile);
        }

        [TestMethod]
        public void LoadOverrideNoBuildInFileTest()
        {
            OverrideData overrideData = OverrideData.Load(GameData, 12345, HeroOverrideTest);
            Assert.IsNotNull(overrideData);

            Assert.AreEqual(1, overrideData.Count);
            Assert.AreEqual("HeroOverrideTest_12345.xml", overrideData.HeroDataOverrideXmlFile);
        }

        [TestMethod]
        public void LoadOverrideNoBuildDoesntExistTest()
        {
            Assert.ThrowsException<FileNotFoundException>(() =>
            {
                OverrideData.Load(GameData, "FileDoesntExist.xml");
            });
        }

        [TestMethod]
        public void LoadOverrideBuildDoesntExistTest()
        {
            Assert.ThrowsException<FileNotFoundException>(() =>
            {
                OverrideData.Load(GameData, 12345, "FileDoesntExist.xml");
            });
        }

        [TestMethod]
        public void LoadOverrideOnlyGameDataTest()
        {
            OverrideData overrideData = OverrideData.Load(GameData);
            Assert.AreEqual("HeroOverrides.xml", overrideData.HeroDataOverrideXmlFile);
        }
    }
}
