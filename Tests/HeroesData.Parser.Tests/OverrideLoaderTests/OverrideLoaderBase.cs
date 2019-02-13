using HeroesData.Loader.XmlGameData;
using System.IO;

namespace HeroesData.Parser.Tests.OverrideLoaderTests
{
    public class OverrideLoaderBase
    {
        private const string TestDataFolder = "TestData";
        private readonly string ModsTestFolder = Path.Combine(TestDataFolder, "mods");

        public OverrideLoaderBase()
        {
            GameData = new FileGameData(ModsTestFolder);
        }

        protected GameData GameData { get; }
        protected string OverrideFileNameSuffix { get; } = "overrides-test";
    }
}
