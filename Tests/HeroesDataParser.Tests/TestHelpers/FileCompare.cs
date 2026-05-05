namespace HeroesDataParser.Tests.TestHelpers;

public static class FileCompare
{
    public static void ShouldBeEqual(string createdFilePath, string testAgainstFilePath)
    {
        File.Exists(createdFilePath).Should().BeTrue();

        string newFileContent = File.ReadAllText(createdFilePath).ReplaceLineEndings("\n");
        string comparedToText = File.ReadAllText(testAgainstFilePath).ReplaceLineEndings("\n");

        newFileContent.Should().Be(comparedToText);
    }
}
