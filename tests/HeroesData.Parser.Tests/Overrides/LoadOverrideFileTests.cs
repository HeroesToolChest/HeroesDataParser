using HeroesData.Parser.UnitData.Overrides;
using HeroesData.Parser.XmlGameData;
using System.IO;
using Xunit;

namespace HeroesData.Parser.Tests.Overrides
{
    public class LoadOverrideFileTests
    {
        private const string TestDataFolder = "TestData";
        private readonly string ModsTestFolder = Path.Combine(TestDataFolder, "mods");
        private readonly string HeroOverrideTestFolder = "HeroOverrideTest.xml";
        private readonly string HeroOverrideBuildTestFolder = "HeroOverrideTest_12345.xml";
        private readonly GameData GameData;

        public LoadOverrideFileTests()
        {
            GameData = GameData.Load(ModsTestFolder);
        }

        [Fact]
        public void LoadOverrideFileTest()
        {
            OverrideData overrideData = OverrideData.Load(GameData, HeroOverrideTestFolder);
            Assert.NotNull(overrideData);

            Assert.Equal(4, overrideData.Count); // LittleLoco counts as one
        }

        [Fact]
        public void LoadOverrideHasBuildInFileTest()
        {
            OverrideData overrideData = OverrideData.Load(GameData, 12345, HeroOverrideBuildTestFolder);
            Assert.NotNull(overrideData);

            Assert.Equal(1, overrideData.Count);
        }

        [Fact]
        public void LoadOverrideNoBuildInFileTest()
        {
            OverrideData overrideData = OverrideData.Load(GameData, 12345, HeroOverrideTestFolder);
            Assert.NotNull(overrideData);

            Assert.Equal(1, overrideData.Count);
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

        [Fact]
        public void LoadOverrideOnlyGameDataAndHotsBuildNoExistsTest()
        {
            OverrideData overrideData = OverrideData.Load(GameData, 23433);

            // finds HeroOverrides.xml since HeroOverrides_23433.xml doesn't exist
            Assert.Equal("HeroOverrides.xml", overrideData.HeroDataOverrideXmlFile);
        }
    }
}
