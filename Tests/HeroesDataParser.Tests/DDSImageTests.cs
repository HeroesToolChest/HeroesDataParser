using SixLabors.ImageSharp;

namespace HeroesDataParser.Tests;

[TestClass]
public class DDSImageTests
{
    private readonly string _testImagesDirectory = "TestImages";

    [TestMethod]
    public async Task Save_SingleImage_CreatesNewFile()
    {
        // arrange
        string file = Path.Combine(_testImagesDirectory, "storm_ui_icon_nova_orbitalstrike.dds");
        using DDSImage image = new(file);

        string outputFile = Path.ChangeExtension(file, ".png");

        // act
        await image.Save(outputFile);

        // assert
        File.Exists(outputFile).Should().BeTrue();
    }

    [TestMethod]
    public async Task Save_ImageIsThreeImages_CreateThreeNewImages()
    {
        // arrange
        string file = Path.Combine(_testImagesDirectory, "storm_ui_mvp_icons_rewards_loyaldefender.dds");

        string blueAward = Path.ChangeExtension(file.Replace("loyaldefender", "loyaldefender_blue", StringComparison.OrdinalIgnoreCase), ".png");
        string redAward = Path.ChangeExtension(file.Replace("loyaldefender", "loyaldefender_red", StringComparison.OrdinalIgnoreCase), ".png");
        string goldAward = Path.ChangeExtension(file.Replace("loyaldefender", "loyaldefender_gold", StringComparison.OrdinalIgnoreCase), ".png");

        using DDSImage image = new(file);

        // act
        int newWidth = 444 / 3;
        await image.Save(blueAward, new Point(0, 0), new Size(newWidth, image.Height));
        await image.Save(redAward, new Point(newWidth, 0), new Size(newWidth, image.Height));
        await image.Save(goldAward, new Point(newWidth * 2, 0), new Size(newWidth, image.Height));

        // assert
        // original image size
        image.Height.Should().Be(148);
        image.Width.Should().Be(444);

        // new files created
        File.Exists(blueAward).Should().BeTrue();
        File.Exists(redAward).Should().BeTrue();
        File.Exists(goldAward).Should().BeTrue();

        // verify new image sizes
        Image blueNewImage = Image.Load(blueAward);
        blueNewImage.Height.Should().Be(148);
        blueNewImage.Width.Should().Be(148);

        Image redNewImage = Image.Load(redAward);
        redNewImage.Height.Should().Be(148);
        redNewImage.Width.Should().Be(148);

        Image redGoldImage = Image.Load(goldAward);
        redGoldImage.Height.Should().Be(148);
        redGoldImage.Width.Should().Be(148);
    }

    [TestMethod]
    public async Task SaveAsGif_ImageWithMultipleImages_CreateNewFile()
    {
        // arrange
        string file = Path.Combine(_testImagesDirectory, "storm_emoji_cat_gleam_anim_sheet.dds");
        using DDSImage image = new(file);

        string outputFile = Path.ChangeExtension(file, "gif");

        // act
        await image.SaveAsGif(outputFile, new Size(34, 32), new Size(40, 32), 25, 50);

        // assert
        File.Exists(outputFile).Should().BeTrue();
    }

    [TestMethod]
    public async Task SaveAsAPNG_ImageWithMultipleImages_CreateNewFile()
    {
        // arrange
        string file = Path.Combine(_testImagesDirectory, "storm_emoji_cat_gleam_anim_sheet.dds");
        using DDSImage image = new(file);

        string outputFile = Path.ChangeExtension(file, "apng");

        // act
        await image.SaveAsAPNG(outputFile, new Size(34, 32), new Size(40, 32), 25, 50);

        // assert
        File.Exists(outputFile).Should().BeTrue();
    }

    [TestMethod]
    public async Task SaveAsAPNG_SprayImage_CreateNewFile()
    {
        // arrange
        string file = Path.Combine(_testImagesDirectory, "storm_lootspray_animated_snowglobe_cursed.dds");
        using Stream stream = File.OpenRead(file);
        using DDSImage image = new(stream);

        string outputFile = Path.ChangeExtension(file, "apng");

        // act
        await image.SaveAsAPNG(outputFile, new Size(256, 256), new Size(256, 256), 2, 2000);

        // assert
        File.Exists(outputFile).Should().BeTrue();
    }
}