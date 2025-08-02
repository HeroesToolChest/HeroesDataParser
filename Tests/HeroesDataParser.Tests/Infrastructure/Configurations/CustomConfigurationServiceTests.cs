using Microsoft.Extensions.FileProviders;

namespace HeroesDataParser.Infrastructure.Configurations.Tests;

[TestClass]
public class CustomConfigurationServiceTests
{
    private readonly ILogger<CustomConfigurationService> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IFileProvider _fileProvider;

    public CustomConfigurationServiceTests()
    {
        _logger = Substitute.For<ILogger<CustomConfigurationService>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _fileProvider = Substitute.For<IFileProvider>();
    }

    [TestMethod]
    public void SelectedCustomDataFilePaths_Random_MatchesAppropriateBuilds()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            BuildNumber = 5800,
            Extractors =
            {
                { "hero", new ExtractorOptions() },
                { "announcer", new ExtractorOptions() },
            },
        });

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("hero", true),
            CreateFileInfo("someotherfile.xml", false),
            CreateFileInfo("announcer", true),
        }.GetEnumerator());

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("Abathur.xml"),
            CreateFileInfo("Abathur_1000.xml"),
            CreateFileInfo("Abathur_2000.xml"),
            CreateFileInfo("Abathur_3000.xml"),
            CreateFileInfo("Alarak.xml"),
            CreateFileInfo("Alarak_12455.xml"),
            CreateFileInfo("Alarak_13455.xml"),
            CreateFileInfo("Ana.xml"),
            CreateFileInfo("someDirectory", true),
        }.GetEnumerator());

        IDirectoryContents announcerDirectoryContents = Substitute.For<IDirectoryContents>();
        announcerDirectoryContents.Exists.Returns(true);
        announcerDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("SomeFile.xml"),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom")).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom", "hero")).Returns(heroDirectoryContents);
        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom", "announcer")).Returns(announcerDirectoryContents);

        CustomConfigurationService customConfigurationService = new(_logger, _options, _fileProvider);
        customConfigurationService.Load();

        // act
        IReadOnlyList<string> selectedPaths = customConfigurationService.SelectedCustomDataFilePaths;

        // assert
        selectedPaths.Should().HaveCount(4)
            .And.ContainInConsecutiveOrder(
                [
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "hero", "Abathur.xml"),
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "hero", "Alarak_12455.xml"),
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "hero", "Ana.xml"),
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "announcer", "SomeFile.xml"),
                ]);
    }

    [TestMethod]
    public void SelectedCustomDataFilePaths_BuildNumberLowerThanFiles_MatchesLowestBuild()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            BuildNumber = 1,
            Extractors =
            {
                { "hero", new ExtractorOptions() },
            },
        });

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("hero", true),
        }.GetEnumerator());

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("Abathur.xml"),
            CreateFileInfo("Abathur_1000.xml"),
            CreateFileInfo("Abathur_2000.xml"),
            CreateFileInfo("Abathur_3000.xml"),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom")).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom", "hero")).Returns(heroDirectoryContents);

        CustomConfigurationService customConfigurationService = new(_logger, _options, _fileProvider);
        customConfigurationService.Load();

        // act
        IReadOnlyList<string> selectedPaths = customConfigurationService.SelectedCustomDataFilePaths;

        // assert
        selectedPaths.Should().ContainSingle()
            .And.Contain(
                [
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "hero", "Abathur_1000.xml")
                ]);
    }

    [TestMethod]
    public void SelectedCustomDataFilePaths_BuildNumberHigherThanFiles_MatchesDefaultFile()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            BuildNumber = 99999,
            Extractors =
            {
                { "hero", new ExtractorOptions() },
                { "announcer", new ExtractorOptions() },
            },
        });

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("hero", true),
            CreateFileInfo("someotherfile.xml", false),
            CreateFileInfo("announcer", true),
        }.GetEnumerator());

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("Abathur.xml"),
            CreateFileInfo("Abathur_1000.xml"),
            CreateFileInfo("Abathur_2000.xml"),
            CreateFileInfo("Abathur_3000.xml"),
            CreateFileInfo("Alarak.xml"),
            CreateFileInfo("Alarak_12455.xml"),
            CreateFileInfo("Alarak_13455.xml"),
            CreateFileInfo("Ana.xml"),
            CreateFileInfo("someDirectory", true),
        }.GetEnumerator());

        IDirectoryContents announcerDirectoryContents = Substitute.For<IDirectoryContents>();
        announcerDirectoryContents.Exists.Returns(true);
        announcerDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("SomeFile.xml"),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom")).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom", "hero")).Returns(heroDirectoryContents);
        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom", "announcer")).Returns(announcerDirectoryContents);

        CustomConfigurationService customConfigurationService = new(_logger, _options, _fileProvider);
        customConfigurationService.Load();

        // act
        IReadOnlyList<string> selectedPaths = customConfigurationService.SelectedCustomDataFilePaths;

        // assert
        selectedPaths.Should().HaveCount(4)
            .And.Contain(
                [
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "hero", "Abathur.xml"),
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "hero", "Alarak.xml"),
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "hero", "Ana.xml"),
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "announcer", "SomeFile.xml"),
                ]);
    }

    [TestMethod]
    public void SelectedCustomDataFilePaths_BuildNumberBetweenFiles_MatchesNextLowestBuild()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            BuildNumber = 2999,
            Extractors =
            {
                { "hero", new ExtractorOptions() },
                { "announcer", new ExtractorOptions() },
            },
        });

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("hero", true),
        }.GetEnumerator());

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("Abathur.xml"),
            CreateFileInfo("Abathur_1000.xml"),
            CreateFileInfo("Abathur_2000.xml"),
            CreateFileInfo("Abathur_3000.xml"),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom")).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom", "hero")).Returns(heroDirectoryContents);

        CustomConfigurationService customConfigurationService = new(_logger, _options, _fileProvider);
        customConfigurationService.Load();

        // act
        IReadOnlyList<string> selectedPaths = customConfigurationService.SelectedCustomDataFilePaths;

        // assert
        selectedPaths.Should().ContainSingle()
            .And.Contain(
                [
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "hero", "Abathur_2000.xml")
                ]);
    }

    [TestMethod]
    public void SelectedCustomDataFilePaths_BuildNumberEqualsAFile_MatchesBuild()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            BuildNumber = 2000,
            Extractors =
            {
                { "hero", new ExtractorOptions() },
                { "announcer", new ExtractorOptions() },
            },
        });

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("hero", true),
        }.GetEnumerator());

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("Abathur.xml"),
            CreateFileInfo("Abathur_1000.xml"),
            CreateFileInfo("Abathur_2000.xml"),
            CreateFileInfo("Abathur_3000.xml"),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom")).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom", "hero")).Returns(heroDirectoryContents);

        CustomConfigurationService customConfigurationService = new(_logger, _options, _fileProvider);
        customConfigurationService.Load();

        // act
        IReadOnlyList<string> selectedPaths = customConfigurationService.SelectedCustomDataFilePaths;

        // assert
        selectedPaths.Should().ContainSingle()
            .And.Contain(
                [
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "hero", "Abathur_2000.xml")
                ]);
    }

    [TestMethod]
    public void SelectedCustomDataFilePaths_NoBuildSet_MatchesHighestBuild()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            BuildNumber = null,
            Extractors =
            {
                { "hero", new ExtractorOptions() },
            },
        });

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("hero", true),
        }.GetEnumerator());

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("Abathur.xml"),
            CreateFileInfo("Abathur_1000.xml"),
            CreateFileInfo("Abathur_2000.xml"),
            CreateFileInfo("Abathur_3000.xml"),
            CreateFileInfo("Alarak.xml"),
            CreateFileInfo("Alarak_12455.xml"),
            CreateFileInfo("Alarak_13455.xml"),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom")).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom", "hero")).Returns(heroDirectoryContents);

        CustomConfigurationService customConfigurationService = new(_logger, _options, _fileProvider);
        customConfigurationService.Load();

        // act
        IReadOnlyList<string> selectedPaths = customConfigurationService.SelectedCustomDataFilePaths;

        // assert
        selectedPaths.Should().HaveCount(2)
            .And.Contain(
                [
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "hero", "Abathur.xml"),
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "hero", "Alarak.xml"),
                ]);
    }

    [TestMethod]
    public void SelectedCustomDataFilePaths_NoBuildSetWithNoFiles_SelectedPathShouldBeEmpty()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            BuildNumber = null,
            Extractors =
            {
                { "hero", new ExtractorOptions() },
            },
        });

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("hero", true),
        }.GetEnumerator());

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom")).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom", "hero")).Returns(heroDirectoryContents);

        CustomConfigurationService customConfigurationService = new(_logger, _options, _fileProvider);
        customConfigurationService.Load();

        // act
        IReadOnlyList<string> selectedPaths = customConfigurationService.SelectedCustomDataFilePaths;

        // assert
        selectedPaths.Should().BeEmpty();
    }

    [TestMethod]
    public void SelectedCustomDataFilePaths_NoExtractorsSet_SelectedPathShouldBeEmpty()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            BuildNumber = null,
            Extractors = { },
        });

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("hero", true),
        }.GetEnumerator());

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("Abathur.xml"),
            CreateFileInfo("Abathur_1000.xml"),
            CreateFileInfo("Abathur_2000.xml"),
            CreateFileInfo("Abathur_3000.xml"),
            CreateFileInfo("Alarak.xml"),
            CreateFileInfo("Alarak_12455.xml"),
            CreateFileInfo("Alarak_13455.xml"),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom")).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom", "hero")).Returns(heroDirectoryContents);

        CustomConfigurationService customConfigurationService = new(_logger, _options, _fileProvider);
        customConfigurationService.Load();

        // act
        IReadOnlyList<string> selectedPaths = customConfigurationService.SelectedCustomDataFilePaths;

        // assert
        selectedPaths.Should().BeEmpty();
    }

    [TestMethod]
    public void SelectedCustomDataFilePaths_FileNameStartsWithSingleUnderscore_Matches()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            BuildNumber = 1,
            Extractors =
            {
                { "hero", new ExtractorOptions() },
            },
        });

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("hero", true),
        }.GetEnumerator());

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("_Abil.xml"),
            CreateFileInfo("_Abil_1000.xml"),
            CreateFileInfo("Abathur.xml"),
            CreateFileInfo("Abathur_1000.xml"),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom")).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom", "hero")).Returns(heroDirectoryContents);

        CustomConfigurationService customConfigurationService = new(_logger, _options, _fileProvider);
        customConfigurationService.Load();

        // act
        IReadOnlyList<string> selectedPaths = customConfigurationService.SelectedCustomDataFilePaths;

        // assert
        selectedPaths.Should().HaveCount(2)
            .And.ContainInConsecutiveOrder(
                [
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "hero", "_Abil_1000.xml"),
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "hero", "Abathur_1000.xml"),
                ]);
    }

    [TestMethod]
    public void SelectedCustomDataFilePaths_ExtractorsHaveSubdirectories_ReturnsCorrectFiles()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            BuildNumber = 5800,
            Extractors =
            {
                { "hero", new ExtractorOptions() },
                { "announcer", new ExtractorOptions() },
            },
        });

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("hero", true),
            CreateFileInfo("someotherfile.xml", false),
            CreateFileInfo("announcer", true),
        }.GetEnumerator());

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("Abathur.xml"),
            CreateFileInfo("Abathur_1000.xml"),
            CreateFileInfo("Abathur_2000.xml"),
            CreateFileInfo("Abathur_3000.xml"),
            CreateFileInfo("Amazon_23000.xml"),
            CreateFileInfo("Ana.xml"),
            CreateFileInfo("Alarak", true, Path.Join("config-files", "custom", "hero", "Alarak")),
            CreateFileInfo("Amazon", true, Path.Join("config-files", "custom", "hero", "Amazon")),
        }.GetEnumerator());

        IDirectoryContents alarakDirectoryContents = Substitute.For<IDirectoryContents>();
        alarakDirectoryContents.Exists.Returns(true);
        alarakDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("Alarak.xml"),
            CreateFileInfo("Alarak_12455.xml"),
            CreateFileInfo("Alarak_13455.xml"),
        }.GetEnumerator());

        IDirectoryContents amazonDirectoryContents = Substitute.For<IDirectoryContents>();
        amazonDirectoryContents.Exists.Returns(true);
        amazonDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("Amazon.xml"),
            CreateFileInfo("Amazon_12455.xml"),
        }.GetEnumerator());

        IDirectoryContents announcerDirectoryContents = Substitute.For<IDirectoryContents>();
        announcerDirectoryContents.Exists.Returns(true);
        announcerDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("SomeFile.xml"),
            CreateFileInfo("Folder1", true, Path.Join("config-files", "custom", "announcer", "Folder1")),
            CreateFileInfo("Folder2", true, Path.Join("config-files", "custom", "announcer", "Folder2")),
        }.GetEnumerator());

        IDirectoryContents folder1DirectoryContents = Substitute.For<IDirectoryContents>();
        folder1DirectoryContents.Exists.Returns(true);
        folder1DirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("SomeFile.xml"),
        }.GetEnumerator());

        IDirectoryContents folder2DirectoryContents = Substitute.For<IDirectoryContents>();
        folder2DirectoryContents.Exists.Returns(true);
        folder2DirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("Announcer2.xml"),
            CreateFileInfo("Announcer2_23333.xml"),
            CreateFileInfo("Announcer2_26000.xml"),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom")).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom", "hero")).Returns(heroDirectoryContents);
        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom", "hero", "Alarak")).Returns(alarakDirectoryContents);
        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom", "hero", "Amazon")).Returns(amazonDirectoryContents);
        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom", "announcer")).Returns(announcerDirectoryContents);
        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom", "announcer", "Folder1")).Returns(folder1DirectoryContents);
        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom", "announcer", "Folder2")).Returns(folder2DirectoryContents);

        CustomConfigurationService customConfigurationService = new(_logger, _options, _fileProvider);
        customConfigurationService.Load();

        // act
        IReadOnlyList<string> selectedPaths = customConfigurationService.SelectedCustomDataFilePaths;

        // assert
        selectedPaths.Should().HaveCount(6)
            .And.ContainInConsecutiveOrder(
                [
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "hero", "Abathur.xml"),
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "hero", "Alarak", "Alarak_12455.xml"),
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "hero", "Amazon", "Amazon_12455.xml"),
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "hero", "Ana.xml"),
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "announcer", "Folder2", "Announcer2_23333.xml"),
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "announcer", "Folder1", "SomeFile.xml"),
                ]);
    }

    [TestMethod]
    public void SelectedCustomDataFilePaths_HitsMaxDepth_Returns12455InsteadOf5800()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            BuildNumber = 5800,
            Extractors =
            {
                { "hero", new ExtractorOptions() },
            },
        });

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("hero", true),
        }.GetEnumerator());

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("Alarak", true, Path.Join("config-files", "custom", "hero", "Alarak")),
        }.GetEnumerator());

        IDirectoryContents alarakDirectoryContents = Substitute.For<IDirectoryContents>();
        alarakDirectoryContents.Exists.Returns(true);
        alarakDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("Alarak.xml"),
            CreateFileInfo("Alarak_12455.xml"),
            CreateFileInfo("InnerDirectory1", true, Path.Join("config-files", "custom", "hero", "Alarak", "InnerDirectory1")),
        }.GetEnumerator());

        IDirectoryContents innerDirectory1Contents = Substitute.For<IDirectoryContents>();
        innerDirectory1Contents.Exists.Returns(true);
        innerDirectory1Contents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("InnerDirectory2", true, Path.Join("config-files", "custom", "hero", "Alarak", "InnerDirectory1", "InnerDirectory2")),
        }.GetEnumerator());

        IDirectoryContents innerDirectory2Contents = Substitute.For<IDirectoryContents>();
        innerDirectory2Contents.Exists.Returns(true);
        innerDirectory2Contents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo("Alarak_5800.xml"),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom")).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom", "hero")).Returns(heroDirectoryContents);
        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom", "hero", "Alarak")).Returns(alarakDirectoryContents);
        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom", "hero", "Alarak", "InnerDirectory1")).Returns(innerDirectory1Contents);
        _fileProvider.GetDirectoryContents(Path.Join("config-files", "custom", "hero", "Alarak", "InnerDirectory1", "InnerDirectory2")).Returns(innerDirectory2Contents);

        CustomConfigurationService customConfigurationService = new(_logger, _options, _fileProvider);
        customConfigurationService.Load();

        // act
        IReadOnlyList<string> selectedPaths = customConfigurationService.SelectedCustomDataFilePaths;

        // assert
        selectedPaths.Should().HaveCount(1)
            .And.ContainInConsecutiveOrder(
                [
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "hero", "Alarak", "Alarak_12455.xml"),
                ]);
    }

    private static IFileInfo CreateFileInfo(string name, bool isDirectory = false, string physicalPath = "")
    {
        IFileInfo fileInfo = Substitute.For<IFileInfo>();
        fileInfo.Exists.Returns(true);
        fileInfo.Name.Returns(name);
        fileInfo.IsDirectory.Returns(isDirectory);
        fileInfo.PhysicalPath.Returns(physicalPath);

        return fileInfo;
    }
}