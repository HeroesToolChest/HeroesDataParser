using System;
using System.Collections.Generic;
using System.Text;

namespace HeroesDataParser.Infrastructure.XmlDataParsers.Tests;

[TestClass]
public class MountParserTests
{
    private readonly ILogger<MountParser> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;
    private readonly IGameStringTextService _gameStringTextService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public MountParserTests()
    {
        _logger = Substitute.For<ILogger<MountParser>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();
        _gameStringTextService = Substitute.For<IGameStringTextService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void Parse_MountBearRank17Base_ReturnsMapData()
    {
        // arrange
        _gameStringTextService.GetGameStringTextFromId("Mount/Info/MountBearRank17Base").Returns(new GameStringText("info mount bear"));
        _gameStringTextService.GetGameStringTextFromId("Mount/SortName/MountBearRank17BaseCommonVar0").Returns(new GameStringText("the sort name"));
        MountParser mountParser = new(_logger, _options, _heroesXmlLoaderService, _gameStringTextService);

        // act
        Mount? mount = mountParser.Parse("MountBearRank17Base");

        // assert
        mount.Should().NotBeNull();
        mount.AttributeId.Should().Be("BEA5");
        mount.Id.Should().Be("MountBearRank17Base");
        mount.Rarity.Should().Be(Rarity.Common);
        mount.ReleaseDate.Should().Be(new DateOnly(2017, 9, 5));
        mount.Category.Should().Be("Beast");
        mount.Event.Should().BeNull();
        mount.HyperlinkId.Should().Be("EarthbreakerGrizzly");
        mount.VariationMountIds.Should().HaveCount(2)
            .And.ContainInConsecutiveOrder("MountBearRank17BaseVar1", "MountBearRank17BaseVar2");
        mount.InfoText!.RawText.Should().Be("info mount bear");
        mount.Franchise.Should().Be(Franchise.Warcraft);
        mount.MountCategory.Should().Be("Ride");
        mount.SortName!.RawText.Should().Be("the sort name");
    }
}
