namespace HeroesDataParser.Tests;

[TestClass]
public class AssemblyHooks
{
    [AssemblyInitialize]
    public static void Initialize(TestContext context)
    {
        if (Directory.Exists(TestConstants.TestDirectory))
            Directory.Delete(TestConstants.TestDirectory, true);
    }
}
