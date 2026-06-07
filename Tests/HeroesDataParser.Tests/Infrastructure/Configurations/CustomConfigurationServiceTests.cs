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
            HeroesVersion =
            {
                Build = 5800,
            },
            Extractors =
            {
                { ExtractDataOptions.Hero, new ExtractorOptions() },
                { ExtractDataOptions.AnnouncerPack, new ExtractorOptions() },
            },
        });

        string customDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom");

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(customDirectory, true),
            CreateFileInfo(Path.Combine(customDirectory, "someotherfile.xml")),
            CreateFileInfo(Path.Combine(customDirectory, "announcerpack"), true),
            CreateFileInfo(Path.Combine(customDirectory, "hero"), true),
        }.GetEnumerator());

        string heroDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom", "hero");

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_1000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_2000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_3000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Alarak.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Alarak_12455.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Alarak_13455.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Ana.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "someDirectory"), true),
        }.GetEnumerator());

        string announcerDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom", "announcerpack");

        IDirectoryContents announcerDirectoryContents = Substitute.For<IDirectoryContents>();
        announcerDirectoryContents.Exists.Returns(true);
        announcerDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(announcerDirectory, "SomeFile.xml")),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(customDirectory).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(heroDirectory).Returns(heroDirectoryContents);
        _fileProvider.GetDirectoryContents(announcerDirectory).Returns(announcerDirectoryContents);

        CustomConfigurationService customConfigurationService = new(_logger, _options, _fileProvider);
        customConfigurationService.Load();

        // act
        IReadOnlyList<string> selectedPaths = customConfigurationService.SelectedCustomDataFilePaths;

        // assert
        selectedPaths.Should().HaveCount(4)
            .And.ContainInConsecutiveOrder(
                [
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "announcerpack", "SomeFile.xml"),
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "hero", "Abathur.xml"),
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "hero", "Alarak_12455.xml"),
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "hero", "Ana.xml"),
                ]);
    }

    [TestMethod]
    public void SelectedCustomDataFilePaths_BuildNumberLowerThanFiles_MatchesLowestBuild()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            HeroesVersion =
            {
                Build = 1,
            },
            Extractors =
            {
                { ExtractDataOptions.Hero, new ExtractorOptions() },
            },
        });

        string customDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom");

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(customDirectory, "hero"), true),
        }.GetEnumerator());

        string heroDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom", "hero");

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_1000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_2000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_3000.xml")),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(customDirectory).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(heroDirectory).Returns(heroDirectoryContents);

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
            HeroesVersion =
            {
                Build = 99999,
            },
            Extractors =
            {
                { ExtractDataOptions.Hero, new ExtractorOptions() },
                { ExtractDataOptions.AnnouncerPack, new ExtractorOptions() },
            },
        });

        string customDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom");

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(customDirectory, "hero"), true),
            CreateFileInfo(Path.Combine(customDirectory, "someotherfile.xml"), false),
            CreateFileInfo(Path.Combine(customDirectory, "announcerpack"), true),
        }.GetEnumerator());

        string heroDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom", "hero");

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_1000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_2000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_3000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Alarak.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Alarak_12455.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Alarak_13455.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Ana.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "someDirectory"), true),
        }.GetEnumerator());

        string announcerDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom", "announcerpack");

        IDirectoryContents announcerDirectoryContents = Substitute.For<IDirectoryContents>();
        announcerDirectoryContents.Exists.Returns(true);
        announcerDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(announcerDirectory, "SomeFile.xml")),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(customDirectory).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(heroDirectory).Returns(heroDirectoryContents);
        _fileProvider.GetDirectoryContents(announcerDirectory).Returns(announcerDirectoryContents);

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
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "announcerpack", "SomeFile.xml"),
                ]);
    }

    [TestMethod]
    public void SelectedCustomDataFilePaths_BuildNumberBetweenFiles_MatchesNextLowestBuild()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            HeroesVersion =
            {
                Build = 2999,
            },
            Extractors =
            {
                { ExtractDataOptions.Hero, new ExtractorOptions() },
                { ExtractDataOptions.AnnouncerPack, new ExtractorOptions() },
            },
        });

        string customDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom");

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(customDirectory, "hero"), true),
        }.GetEnumerator());

        string directory = Path.Combine(Constants.ConfigFilesDirectory, "custom");

        string heroDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom", "hero");

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_1000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_2000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_3000.xml")),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(customDirectory).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(heroDirectory).Returns(heroDirectoryContents);

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
            HeroesVersion =
            {
                Build = 2000,
            },
            Extractors =
            {
                { ExtractDataOptions.Hero, new ExtractorOptions() },
                { ExtractDataOptions.AnnouncerPack, new ExtractorOptions() },
            },
        });

        string customDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom");

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(customDirectory, "hero"), true),
        }.GetEnumerator());

        string heroDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom", "hero");

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_1000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_2000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_3000.xml")),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(customDirectory).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(heroDirectory).Returns(heroDirectoryContents);

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
            HeroesVersion =
            {
                Build = -1,
            },
            Extractors =
            {
                { ExtractDataOptions.Hero, new ExtractorOptions() },
            },
        });

        string customDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom");

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(customDirectory, "hero"), true),
        }.GetEnumerator());

        string heroDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom", "hero");

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_1000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_2000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_3000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Alarak.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Alarak_12455.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Alarak_13455.xml")),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(customDirectory).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(heroDirectory).Returns(heroDirectoryContents);

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
            HeroesVersion =
            {
                Build = -1,
            },
            Extractors =
            {
                { ExtractDataOptions.Hero, new ExtractorOptions() },
            },
        });

        string customDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom");

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(customDirectory, "hero"), true),
        }.GetEnumerator());

        string heroDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom", "hero");

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(customDirectory).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(heroDirectory).Returns(heroDirectoryContents);

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
            HeroesVersion =
            {
                Build = -1,
            },
            Extractors = { },
        });

        string customDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom");

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(customDirectory, "hero"), true),
        }.GetEnumerator());

        string heroDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom", "hero");

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_1000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_2000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_3000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Alarak.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Alarak_12455.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Alarak_13455.xml")),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(customDirectory).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(heroDirectory).Returns(heroDirectoryContents);

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
            HeroesVersion =
            {
                Build = 1,
            },
            Extractors =
            {
                { ExtractDataOptions.Hero, new ExtractorOptions() },
            },
        });

        string customDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom");

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(customDirectory, "hero"), true),
        }.GetEnumerator());

        string heroDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom", "hero");

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(heroDirectory, "_Abil.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "_Abil_1000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_1000.xml")),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(customDirectory).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(heroDirectory).Returns(heroDirectoryContents);

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
            HeroesVersion =
            {
                Build = 5800,
            },
            Extractors =
            {
                { ExtractDataOptions.Hero, new ExtractorOptions() },
                { ExtractDataOptions.AnnouncerPack, new ExtractorOptions() },
            },
        });

        string customDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom");

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(customDirectory, "hero"), true),
            CreateFileInfo(Path.Combine(customDirectory, "someotherfile.xml"), false),
            CreateFileInfo(Path.Combine(customDirectory, "announcerpack"), true),
        }.GetEnumerator());

        string heroDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom", "hero");

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_1000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_2000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Abathur_3000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Amazon_23000.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Ana.xml")),
            CreateFileInfo(Path.Combine(heroDirectory, "Alarak"), true),
            CreateFileInfo(Path.Combine(heroDirectory, "Amazon"), true),
        }.GetEnumerator());

        string alarakDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom", "hero", "Alarak");

        IDirectoryContents alarakDirectoryContents = Substitute.For<IDirectoryContents>();
        alarakDirectoryContents.Exists.Returns(true);
        alarakDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(alarakDirectory, "Alarak.xml")),
            CreateFileInfo(Path.Combine(alarakDirectory, "Alarak_12455.xml")),
            CreateFileInfo(Path.Combine(alarakDirectory, "Alarak_13455.xml")),
        }.GetEnumerator());

        string amazonDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom", "hero", "Amazon");

        IDirectoryContents amazonDirectoryContents = Substitute.For<IDirectoryContents>();
        amazonDirectoryContents.Exists.Returns(true);
        amazonDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(amazonDirectory, "Amazon.xml")),
            CreateFileInfo(Path.Combine(amazonDirectory, "Amazon_12455.xml")),
        }.GetEnumerator());

        string announcerDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom", "announcerpack");

        IDirectoryContents announcerDirectoryContents = Substitute.For<IDirectoryContents>();
        announcerDirectoryContents.Exists.Returns(true);
        announcerDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(announcerDirectory, "SomeFile.xml")),
            CreateFileInfo(Path.Combine(announcerDirectory, "Folder1"), true),
            CreateFileInfo(Path.Combine(announcerDirectory, "Folder2"), true),
        }.GetEnumerator());

        string folder1Directory = Path.Combine(Constants.ConfigFilesDirectory, "custom", "announcerpack", "Folder1");

        IDirectoryContents folder1DirectoryContents = Substitute.For<IDirectoryContents>();
        folder1DirectoryContents.Exists.Returns(true);
        folder1DirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(folder1Directory, "SomeFile.xml")),
        }.GetEnumerator());

        string folder2Directory = Path.Combine(Constants.ConfigFilesDirectory, "custom", "announcerpack", "Folder2");

        IDirectoryContents folder2DirectoryContents = Substitute.For<IDirectoryContents>();
        folder2DirectoryContents.Exists.Returns(true);
        folder2DirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(folder2Directory, "Announcer2.xml")),
            CreateFileInfo(Path.Combine(folder2Directory, "Announcer2_23333.xml")),
            CreateFileInfo(Path.Combine(folder2Directory, "Announcer2_26000.xml")),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(customDirectory).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(heroDirectory).Returns(heroDirectoryContents);
        _fileProvider.GetDirectoryContents(alarakDirectory).Returns(alarakDirectoryContents);
        _fileProvider.GetDirectoryContents(amazonDirectory).Returns(amazonDirectoryContents);
        _fileProvider.GetDirectoryContents(announcerDirectory).Returns(announcerDirectoryContents);
        _fileProvider.GetDirectoryContents(folder1Directory).Returns(folder1DirectoryContents);
        _fileProvider.GetDirectoryContents(folder2Directory).Returns(folder2DirectoryContents);

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
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "announcerpack", "Folder2", "Announcer2_23333.xml"),
                    Path.Join(customConfigurationService.CustomConfigurationDirectory, "announcerpack", "Folder1", "SomeFile.xml"),
                ]);
    }

    [TestMethod]
    public void SelectedCustomDataFilePaths_HitsMaxDepth_Returns12455InsteadOf5800()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            HeroesVersion =
            {
                Build = 5800,
            },
            Extractors =
            {
                { ExtractDataOptions.Hero, new ExtractorOptions() },
            },
        });

        string customDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom");

        IDirectoryContents customDirectoryContents = Substitute.For<IDirectoryContents>();
        customDirectoryContents.Exists.Returns(true);
        customDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(customDirectory, "hero"), true),
        }.GetEnumerator());

        string heroDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom", "hero");

        IDirectoryContents heroDirectoryContents = Substitute.For<IDirectoryContents>();
        heroDirectoryContents.Exists.Returns(true);
        heroDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(heroDirectory, "Alarak"), true),
        }.GetEnumerator());

        string alarakDirectory = Path.Combine(Constants.ConfigFilesDirectory, "custom", "hero", "Alarak");

        IDirectoryContents alarakDirectoryContents = Substitute.For<IDirectoryContents>();
        alarakDirectoryContents.Exists.Returns(true);
        alarakDirectoryContents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(alarakDirectory, "Alarak.xml")),
            CreateFileInfo(Path.Combine(alarakDirectory, "Alarak_12455.xml")),
            CreateFileInfo(Path.Combine(alarakDirectory, "InnerDirectory1"), true),
        }.GetEnumerator());

        string innerDirectory1 = Path.Combine(Constants.ConfigFilesDirectory, "custom", "hero", "Alarak", "InnerDirectory1");

        IDirectoryContents innerDirectory1Contents = Substitute.For<IDirectoryContents>();
        innerDirectory1Contents.Exists.Returns(true);
        innerDirectory1Contents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
           CreateFileInfo(Path.Combine(innerDirectory1, "InnerDirectory2"), true),
        }.GetEnumerator());

        string innerDirectory2 = Path.Combine(Constants.ConfigFilesDirectory, "custom", "hero", "Alarak", "InnerDirectory1", "InnerDirectory2");

        IDirectoryContents innerDirectory2Contents = Substitute.For<IDirectoryContents>();
        innerDirectory2Contents.Exists.Returns(true);
        innerDirectory2Contents.GetEnumerator().Returns(x => new List<IFileInfo>
        {
            CreateFileInfo(Path.Combine(innerDirectory2, "Alarak_5800.xml")),
        }.GetEnumerator());

        _fileProvider.GetDirectoryContents(customDirectory).Returns(customDirectoryContents);
        _fileProvider.GetDirectoryContents(heroDirectory).Returns(heroDirectoryContents);
        _fileProvider.GetDirectoryContents(alarakDirectory).Returns(alarakDirectoryContents);
        _fileProvider.GetDirectoryContents(innerDirectory1).Returns(innerDirectory1Contents);
        _fileProvider.GetDirectoryContents(innerDirectory2).Returns(innerDirectory2Contents);

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

    private static IFileInfo CreateFileInfo(string filePath, bool isDirectory = false)
    {
        IFileInfo fileInfo = Substitute.For<IFileInfo>();
        fileInfo.Exists.Returns(true);
        fileInfo.Name.Returns(Path.GetFileName(filePath));
        fileInfo.IsDirectory.Returns(isDirectory);
        fileInfo.PhysicalPath.Returns(filePath);
        return fileInfo;
    }
}