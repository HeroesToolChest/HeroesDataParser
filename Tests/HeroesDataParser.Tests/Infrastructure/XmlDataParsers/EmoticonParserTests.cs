namespace HeroesDataParser.Infrastructure.XmlDataParsers.Tests;

[TestClass]
public class EmoticonParserTests
{
    private readonly ILogger<EmoticonParser> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IGameStringTextService _gameStringTextService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public EmoticonParserTests()
    {
        _logger = Substitute.For<ILogger<EmoticonParser>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _gameStringTextService = Substitute.For<IGameStringTextService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void Parse_greymane_rofl_ReturnsEmoticonData()
    {
        // arrange
        _gameStringTextService.GetGameStringTextFromId("Emoticon/Description/greymane_rofl").Returns(new GameStringText("greymane rofl"));

        EmoticonParser emoticonParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Emoticon? emoticon = emoticonParser.Parse("greymane_rofl");

        // assert
        emoticon.Should().NotBeNull();
        emoticon.Id.Should().Be("greymane_rofl");
        emoticon.Expression.Should().Be("Rofl");
        emoticon.HeroId.Should().Be("Greymane");
        emoticon.Description!.RawText.Should().Be("greymane rofl");
        emoticon.Animation.Should().BeNull();
        emoticon.TextureSheet.Image.Should().Be("Assets\\Textures\\storm_emoji_greymane_sheet.dds");
        emoticon.TextureSheet.Columns.Should().Be(4);
        emoticon.TextureSheet.Rows.Should().Be(3);
        emoticon.Image.Should().Be("storm_emoji_greymane_sheet_5.png");
        emoticon.UniversalAliases.Should().HaveCount(4)
            .And.ContainInOrder(":greymanelol:", ":greymanerofl:", ":greylol:", ":greyrofl:");
        emoticon.LocalizedAliases.Should().BeEmpty();
        emoticon.Index.Should().Be(5);
        emoticon.Width.Should().Be(29);
        emoticon.IsCaseSensitive.Should().BeFalse();
        emoticon.IsHidden.Should().BeFalse();
    }

    [TestMethod]
    public void Parse_cat_gleam_anim_ReturnsEmoticonData()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            Hidden =
            {
                 AnimatedImageType = AnimatedImageType.APNG,
            },
        });

        _gameStringTextService.GetGameStringTextFromId("Emoticon/Description/cat_gleam_anim").Returns(new GameStringText("cat gleam anim"));
        _gameStringTextService.GetGameStringTextFromId("Emoticon/Name/cat_gleam_anim").Returns(new GameStringText(":)"));

        EmoticonParser emoticonParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Emoticon? emoticon = emoticonParser.Parse("cat_gleam_anim");

        // assert
        emoticon.Should().NotBeNull();
        emoticon.Id.Should().Be("cat_gleam_anim");
        emoticon.Expression.Should().Be("Unknown");
        emoticon.HeroId.Should().BeNull();
        emoticon.SkinId.Should().BeNull();
        emoticon.Description!.RawText.Should().Be("cat gleam anim");
        emoticon.Animation.Should().NotBeNull();
        emoticon.Animation.Columns.Should().Be(4);
        emoticon.Animation.Duration.Should().Be(50);
        emoticon.Animation.Frames.Should().Be(25);
        emoticon.Animation.Rows.Should().Be(7);
        emoticon.Animation.Width.Should().Be(34);
        emoticon.Animation.Texture.Should().Be("storm_emoji_cat_gleam_anim_sheet.png");
        emoticon.TextureSheet.Should().NotBeNull();
        emoticon.TextureSheet.Columns.Should().Be(4);
        emoticon.TextureSheet.Rows.Should().Be(7);
        emoticon.TextureSheet.Image.Should().Be("Assets\\Textures\\storm_emoji_cat_gleam_anim_sheet.dds");
        emoticon.Image.Should().Be("storm_emoji_cat_gleam_anim_sheet.apng");
        emoticon.Index.Should().Be(0);
        emoticon.Width.Should().BeNull();
        emoticon.UniversalAliases.Should().ContainSingle()
            .And.ContainInOrder(":catgleam:");
        emoticon.LocalizedAliases.Should().ContainSingle();
        emoticon.LocalizedAliases.ToList()[0].RawText.Should().Be(":)");
        emoticon.IsCaseSensitive.Should().BeFalse();
        emoticon.IsHidden.Should().BeFalse();
    }

    [TestMethod]
    public void Parse_cat_gleam_anim_ReturnsEmoticonDataAsGIF()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            Hidden =
            {
                 AnimatedImageType = AnimatedImageType.GIF,
            },
        });

        EmoticonParser emoticonParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Emoticon? emoticon = emoticonParser.Parse("cat_gleam_anim");

        // assert
        emoticon.Should().NotBeNull();
        emoticon.Id.Should().Be("cat_gleam_anim");
        emoticon.Image.Should().Be("storm_emoji_cat_gleam_anim_sheet.gif");
    }

    [TestMethod]
    public void Parse_abathur_mecha_angry_ReturnsEmoticonData()
    {
        // arrange
        _gameStringTextService.GetGameStringTextFromId("Emoticon/Description/abathur_mecha_angry").Returns(new GameStringText("abathur mecha angry"));

        EmoticonParser emoticonParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Emoticon? emoticon = emoticonParser.Parse("abathur_mecha_angry");

        // assert
        emoticon.Should().NotBeNull();
        emoticon.Id.Should().Be("abathur_mecha_angry");
        emoticon.Expression.Should().Be("Angry");
        emoticon.HeroId.Should().Be("Abathur");
        emoticon.SkinId.Should().Be("AbathurMecha");
        emoticon.Index.Should().Be(0);
        emoticon.Width.Should().Be(37);
        emoticon.Image.Should().Be("storm_emoji_abathur_mecha_sheet_0.png");
        emoticon.IsCaseSensitive.Should().BeFalse();
        emoticon.IsHidden.Should().BeFalse();
    }

    [TestMethod]
    public void Parse_abstract_cool_casesensitive_ReturnsEmoticonData()
    {
        // arrange
        EmoticonParser emoticonParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Emoticon? emoticon = emoticonParser.Parse("abstract_cool_casesensitive");

        // assert
        emoticon.Should().NotBeNull();
        emoticon.Id.Should().Be("abstract_cool_casesensitive");
        emoticon.Expression.Should().Be("Cool");
        emoticon.HeroId.Should().BeNull();
        emoticon.SkinId.Should().BeNull();
        emoticon.Index.Should().Be(0);
        emoticon.Width.Should().Be(0);
        emoticon.Image.Should().BeNull();
        emoticon.IsCaseSensitive.Should().BeTrue();
        emoticon.IsHidden.Should().BeTrue();
    }
}
