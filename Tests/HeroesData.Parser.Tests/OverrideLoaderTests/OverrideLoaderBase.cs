using System.IO;

namespace HeroesData.Parser.Tests.OverrideLoaderTests
{
    public class OverrideLoaderBase
    {
        private const string TestDataFolder = "TestData";
        private readonly string ModsTestFolder = Path.Combine(TestDataFolder, "mods");

        protected string OverrideFileNameSuffix { get; } = "overrides-test";
    }
}
