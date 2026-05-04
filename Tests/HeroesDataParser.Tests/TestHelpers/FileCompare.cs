namespace HeroesDataParser.Tests.TestHelpers;

public static class FileCompare
{
    public static void ShouldBeEqual(string createdFilePath, string testAgainstFilePath)
    {
        File.Exists(createdFilePath).Should().BeTrue();

        string newFileContent = File.ReadAllText(createdFilePath);
        string comparedToText = File.ReadAllText(testAgainstFilePath);

        newFileContent.Should().BeEquivalentTo(comparedToText);

        string expected = File.ReadAllText(createdFilePath);
        string actual = File.ReadAllText(testAgainstFilePath);
        expected.Should().Be(actual);
    }
}
