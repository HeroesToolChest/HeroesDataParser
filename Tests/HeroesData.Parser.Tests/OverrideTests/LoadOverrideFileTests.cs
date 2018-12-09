using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.UnitData.Overrides;
using System.IO;
using Xunit;

namespace HeroesData.Parser.Tests.OverrideTests
{
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

        [Fact]
        public void LoadOverrideFileTest()
        {
            OverrideData overrideData = OverrideData.Load(GameData, HeroOverrideTest);
            Assert.NotNull(overrideData);

            Assert.Equal(4, overrideData.Count); // LittleLoco counts as one
            Assert.Equal(HeroOverrideTest, overrideData.HeroDataOverrideXmlFile);
        }

        [Fact]
        public void LoadOverrideHasBuildInFileTest()
        {
            OverrideData overrideData = OverrideData.Load(GameData, 12345, HeroOverrideBuild12345Test);
            Assert.NotNull(overrideData);

            Assert.Equal(1, overrideData.Count);
            Assert.Equal(HeroOverrideBuild12345Test, overrideData.HeroDataOverrideXmlFile);
        }

        [Fact]
        public void LoadOverrideHasBuildOpenNextLowestBuildTest()
        {
            OverrideData overrideData = OverrideData.Load(GameData, 11500, HeroOverrideTest);
            Assert.NotNull(overrideData);

            Assert.Equal(7, overrideData.Count);
            Assert.Equal("HeroOverrideTest_11000.xml", overrideData.HeroDataOverrideXmlFile);
        }

        [Fact]
        public void LoadOverrideHasBuildOpenNextLowestBuild2Test()
        {
            OverrideData overrideData = OverrideData.Load(GameData, 12100, HeroOverrideTest);
            Assert.NotNull(overrideData);

            Assert.Equal(11, overrideData.Count);
            Assert.Equal("HeroOverrideTest_12000.xml", overrideData.HeroDataOverrideXmlFile);
        }

        [Fact]
        public void LoadOverrideHasBuildLowerThanLowTest()
        {
            OverrideData overrideData = OverrideData.Load(GameData, 1000, HeroOverrideTest);
            Assert.NotNull(overrideData);

            Assert.Equal(7, overrideData.Count);
            Assert.Equal("HeroOverrideTest_11000.xml", overrideData.HeroDataOverrideXmlFile);
        }

        [Fact]
        public void LoadOverrideHasBuildHighestDefaultBuildTest()
        {
            OverrideData overrideData = OverrideData.Load(GameData, 13000, HeroOverrideTest);
            Assert.NotNull(overrideData);

            Assert.Equal(4, overrideData.Count);
            Assert.Equal(HeroOverrideTest, overrideData.HeroDataOverrideXmlFile);
        }

        [Fact]
        public void LoadOverrideNoBuildInFileTest()
        {
            OverrideData overrideData = OverrideData.Load(GameData, 12345, HeroOverrideTest);
            Assert.NotNull(overrideData);

            Assert.Equal(1, overrideData.Count);
            Assert.Equal("HeroOverrideTest_12345.xml", overrideData.HeroDataOverrideXmlFile);
        }

        [Fact]
        public void LoadOverrideNoBuildDoesntExistTest()
        {
            Assert.Throws<FileNotFoundException>(() =>
            {
                OverrideData.Load(GameData, "FileDoesntExist.xml");
            });
        }

        [Fact]
        public void LoadOverrideBuildDoesntExistTest()
        {
            Assert.Throws<FileNotFoundException>(() =>
            {
                OverrideData.Load(GameData, 12345, "FileDoesntExist.xml");
            });
        }

        [Fact]
        public void LoadOverrideOnlyGameDataTest()
        {
            OverrideData overrideData = OverrideData.Load(GameData);
            Assert.Equal("HeroOverrides.xml", overrideData.HeroDataOverrideXmlFile);
        }
    }
}
