using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeroesDataParser.Infrastructure;
using Heroes.XmlData;
using HeroesDataParser.Core;
using HeroesDataParser.Options;
using Microsoft.Extensions.Options;

namespace HeroesDataParser.Infrastructure.Tests;

[TestClass]
public class TooltipDescriptionServiceTests
{
    private readonly ILogger<TooltipDescriptionService> _logger;
    private readonly IOptions<RootOptions> _options;
    private readonly IHeroesXmlLoaderService _heroesXmlLoaderService;

    private readonly HeroesXmlLoader _heroesXmlLoader;

    public TooltipDescriptionServiceTests()
    {
        _logger = Substitute.For<ILogger<TooltipDescriptionService>>();
        _options = Substitute.For<IOptions<RootOptions>>();
        _heroesXmlLoaderService = Substitute.For<IHeroesXmlLoaderService>();

        _heroesXmlLoader = TestHeroesXmlLoader.GetArrangedHeroesXmlLoader();
        _heroesXmlLoaderService.HeroesXmlLoader.Returns(_heroesXmlLoader);
    }

    [TestMethod]
    public void GetTooltipDescription_GetFromTextAndNoFontStyleExtraction_GetsTooltipDescription()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            DescriptionText = new DescriptionTextOptions()
            {
                Type = DescriptionType.RawDescription,
                ReplaceFontStyles = false,
            },
        });

        TooltipDescriptionService tooltipDescriptionService = new(_logger, _options, _heroesXmlLoaderService);

        // act
        TooltipDescription result = tooltipDescriptionService.GetTooltipDescription("Cooldown: <d ref=\"Abil,AnaHealingDart,Cost.Cooldown.TimeUse\"/> seconds");

        // assert
        result.RawDescription.Should().Be("Cooldown: <d ref=\"Abil,AnaHealingDart,Cost.Cooldown.TimeUse\"/> seconds");
    }

    [TestMethod]
    public void GetTooltipDescription_WithFontStyleExtractionNoStormStylesFound_GetsTooltipDescriptionWithNoReplacements()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            DescriptionText = new DescriptionTextOptions()
            {
                Type = DescriptionType.RawDescription,
                ReplaceFontStyles = true,
            },
        });

        TooltipDescriptionService tooltipDescriptionService = new(_logger, _options, _heroesXmlLoaderService);

        // act
        TooltipDescription result = tooltipDescriptionService.GetTooltipDescription("Instantly boost an allied Hero, restoring <c val=\"#TooltipNumbers2\">200</c> Mana <c val=\"#TooltipNumbers2\">250</c>;<s val=\"StandardTooltipDetails2\">Mana: 50</s>;<s val=\"StandardTooltipDetails2\">Mana: 100</s>");

        // assert
        result.RawDescription.Should().Be("Instantly boost an allied Hero, restoring <c val=\"#TooltipNumbers2\">200</c> Mana <c val=\"#TooltipNumbers2\">250</c>;<s val=\"StandardTooltipDetails2\">Mana: 50</s>;<s val=\"StandardTooltipDetails2\">Mana: 100</s>");
        tooltipDescriptionService.ShouldExtractFontValues.Should().BeTrue();
        tooltipDescriptionService.ValByStyleConstantName.Should().BeEmpty();
        tooltipDescriptionService.ValByStyleName.Should().BeEmpty();
    }

    [TestMethod]
    public void GetTooltipDescription_WithFontStyleExtraction_GetsTooltipDescriptionWithReplacements()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            DescriptionText = new DescriptionTextOptions()
            {
                Type = DescriptionType.RawDescription,
                ReplaceFontStyles = true,
            },
        });

        TooltipDescriptionService tooltipDescriptionService = new(_logger, _options, _heroesXmlLoaderService);

        // act
        TooltipDescription result = tooltipDescriptionService.GetTooltipDescription("Instantly boost an allied Hero, restoring <c val=\"#TooltipNumbers\">200</c> Mana <c val=\"#TooltipAttackDamageNumbers\">250</c>;<s val=\"StandardTooltipDetails\">Mana: 50</s>;<s val=\"StandardTooltipProgressComplete\">Mana: 100</s>");

        // assert
        result.RawDescription.Should().Be("Instantly boost an allied Hero, restoring <c val=\"bfd4fd\">200</c> Mana <c val=\"c27f02\">250</c>;<s val=\"bfd4fd\">Mana: 50</s>;<s val=\"FFFFFF\">Mana: 100</s>");
        tooltipDescriptionService.ShouldExtractFontValues.Should().BeTrue();
        tooltipDescriptionService.ValByStyleConstantName.Should().HaveCount(2);
        tooltipDescriptionService.ValByStyleName.Should().HaveCount(2);
    }

    [TestMethod]
    public void GetTooltipDescription_WithFontStyleExtractionWithPreserve_GetsTooltipDescriptionWithReplacements()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            DescriptionText = new DescriptionTextOptions()
            {
                Type = DescriptionType.RawDescription,
                ReplaceFontStyles = true,
                PreserveFontStyleConstantVars = true,
                PreserveFontStyleVars = true,
            },
        });

        TooltipDescriptionService tooltipDescriptionService = new(_logger, _options, _heroesXmlLoaderService);

        // act
        TooltipDescription result = tooltipDescriptionService.GetTooltipDescription("Instantly boost an allied Hero, restoring <c val=\"#TooltipNumbers\">200</c> Mana <c val=\"#TooltipNumbers\">250</c>;<s val=\"StandardTooltipDetails\">Mana: 50</s>;<s val=\"StandardTooltipDetails\">Mana: 100</s>");

        // assert
        result.RawDescription.Should().Be("Instantly boost an allied Hero, restoring <c val=\"bfd4fd\" hlt-name=\"#TooltipNumbers\">200</c> Mana <c val=\"bfd4fd\" hlt-name=\"#TooltipNumbers\">250</c>;<s val=\"bfd4fd\" hlt-name=\"StandardTooltipDetails\">Mana: 50</s>;<s val=\"bfd4fd\" hlt-name=\"StandardTooltipDetails\">Mana: 100</s>");
        tooltipDescriptionService.ShouldExtractFontValues.Should().BeTrue();
        tooltipDescriptionService.ValByStyleConstantName.Should().ContainSingle();
        tooltipDescriptionService.ValByStyleName.Should().ContainSingle();
    }

    [TestMethod]
    public void GetTooltipDescription_WithFontStyleExtractionMissingAttributes_GetsTooltipDescriptionWithReplacements()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            DescriptionText = new DescriptionTextOptions()
            {
                Type = DescriptionType.RawDescription,
                ReplaceFontStyles = true,
            },
        });

        TooltipDescriptionService tooltipDescriptionService = new(_logger, _options, _heroesXmlLoaderService);

        // act
        TooltipDescription result = tooltipDescriptionService.GetTooltipDescription("Instantly boost an allied Hero, restoring <c val=\"#TooltipNumbers\">200</c> Mana <c val=\"#TooltipNumbersNoVal\">250</c>;<s val=\"StandardTooltipDetails\">Mana: 50</s>;<s val=\"StandardTooltipDetailsNoTextColor\">Mana: 100</s>");

        // assert
        result.RawDescription.Should().Be("Instantly boost an allied Hero, restoring <c val=\"bfd4fd\">200</c> Mana <c val=\"#TooltipNumbersNoVal\">250</c>;<s val=\"bfd4fd\">Mana: 50</s>;<s val=\"StandardTooltipDetailsNoTextColor\">Mana: 100</s>");
        tooltipDescriptionService.ShouldExtractFontValues.Should().BeTrue();
        tooltipDescriptionService.ValByStyleConstantName.Should().ContainSingle();
        tooltipDescriptionService.ValByStyleName.Should().ContainSingle();
    }

    [TestMethod]
    public void GetTooltipDescriptionFromId_NoIdFound_ReturnsNull()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            DescriptionText = new DescriptionTextOptions()
            {
                Type = DescriptionType.RawDescription,
                ReplaceFontStyles = true,
            },
        });

        TooltipDescriptionService tooltipDescriptionService = new(_logger, _options, _heroesXmlLoaderService);

        // act
        TooltipDescription? result = tooltipDescriptionService.GetTooltipDescriptionFromId("No--Id");

        // assert
        result.Should().BeNull();
    }

    [TestMethod]
    public void GetTooltipDescriptionFromId_IdFound_ReturnsTooltipDescription()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            DescriptionText = new DescriptionTextOptions()
            {
                Type = DescriptionType.RawDescription,
                ReplaceFontStyles = true,
            },
        });

        TooltipDescriptionService tooltipDescriptionService = new(_logger, _options, _heroesXmlLoaderService);

        // act
        TooltipDescription? result = tooltipDescriptionService.GetTooltipDescriptionFromId("test_for_tooltip_decription_service");

        // assert
        result.Should().NotBeNull();
        result.RawDescription.Should().Be("Instantly boost an allied Hero, restoring <c val=\"bfd4fd\">200</c> Mana <c val=\"#TooltipNumbersNoVal\">250</c>;<s val=\"bfd4fd\">Mana: 50</s>;<s val=\"StandardTooltipDetailsNoTextColor\">Mana: 100</s>");
        tooltipDescriptionService.ShouldExtractFontValues.Should().BeTrue();
        tooltipDescriptionService.ValByStyleConstantName.Should().ContainSingle();
        tooltipDescriptionService.ValByStyleName.Should().ContainSingle();
    }

    [TestMethod]
    public void GetTooltipDescriptionFromGameString_ParsesStormGameString_ReturnsTooltipDescription()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            DescriptionText = new DescriptionTextOptions()
            {
                Type = DescriptionType.RawDescription,
                ReplaceFontStyles = true,
            },
        });

        StormGameString stormGameString = _heroesXmlLoader.HeroesData.GetStormGameString("test_for_tooltip_decription_service")!;

        TooltipDescriptionService tooltipDescriptionService = new(_logger, _options, _heroesXmlLoaderService);

        // act
        TooltipDescription? result = tooltipDescriptionService.GetTooltipDescriptionFromGameString(stormGameString);

        // assert
        result.Should().NotBeNull();
        result.RawDescription.Should().Be("Instantly boost an allied Hero, restoring <c val=\"bfd4fd\">200</c> Mana <c val=\"#TooltipNumbersNoVal\">250</c>;<s val=\"bfd4fd\">Mana: 50</s>;<s val=\"StandardTooltipDetailsNoTextColor\">Mana: 100</s>");
        tooltipDescriptionService.ShouldExtractFontValues.Should().BeTrue();
        tooltipDescriptionService.ValByStyleConstantName.Should().ContainSingle();
        tooltipDescriptionService.ValByStyleName.Should().ContainSingle();
    }

    [TestMethod]
    public void GetTooltipDescriptionFromGameString_ParsesTextGameString_ReturnsTooltipDescription()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            DescriptionText = new DescriptionTextOptions()
            {
                Type = DescriptionType.RawDescription,
                ReplaceFontStyles = true,
            },
        });

        TooltipDescriptionService tooltipDescriptionService = new(_logger, _options, _heroesXmlLoaderService);

        // act
        TooltipDescription? result = tooltipDescriptionService.GetTooltipDescriptionFromGameString("Instantly boost an allied Hero, restoring <c val=\"#TooltipNumbers\">200</c> Mana <c val=\"#TooltipNumbersNoVal\">250</c>;<s val=\"StandardTooltipDetails\">Mana: 50</s>;<s val=\"StandardTooltipDetailsNoTextColor\">Mana: 100</s>");

        // assert
        result.Should().NotBeNull();
        result.RawDescription.Should().Be("Instantly boost an allied Hero, restoring <c val=\"bfd4fd\">200</c> Mana <c val=\"#TooltipNumbersNoVal\">250</c>;<s val=\"bfd4fd\">Mana: 50</s>;<s val=\"StandardTooltipDetailsNoTextColor\">Mana: 100</s>");
        tooltipDescriptionService.ShouldExtractFontValues.Should().BeTrue();
        tooltipDescriptionService.ValByStyleConstantName.Should().ContainSingle();
        tooltipDescriptionService.ValByStyleName.Should().ContainSingle();
    }

    [TestMethod]
    public void GetStormGameString_NoIdFound_ReturnsNull()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            DescriptionText = new DescriptionTextOptions()
            {
                Type = DescriptionType.RawDescription,
                ReplaceFontStyles = true,
            },
        });

        TooltipDescriptionService tooltipDescriptionService = new(_logger, _options, _heroesXmlLoaderService);

        // act
        string? result = tooltipDescriptionService.GetStormGameString("No--Id");

        // assert
        result.Should().BeNull();
    }

    [TestMethod]
    public void GetStormGameString_IdFound_ReturnsUnParsedGameString()
    {
        // arrange
        _options.Value.Returns(new RootOptions()
        {
            DescriptionText = new DescriptionTextOptions()
            {
                Type = DescriptionType.RawDescription,
                ReplaceFontStyles = true,
            },
        });

        TooltipDescriptionService tooltipDescriptionService = new(_logger, _options, _heroesXmlLoaderService);

        // act
        string? result = tooltipDescriptionService.GetStormGameString("test_for_tooltip_decription_service");

        // assert
        result.Should().NotBeNull();
        result.Should().Be("Instantly boost an allied Hero, restoring <c val=\"#TooltipNumbers\">200</c> Mana <c val=\"#TooltipNumbersNoVal\">250</c>;<s val=\"StandardTooltipDetails\">Mana: 50</s>;<s val=\"StandardTooltipDetailsNoTextColor\">Mana: 100</s>");
    }
}