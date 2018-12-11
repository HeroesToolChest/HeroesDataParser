using DDSReader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SixLabors.ImageSharp;
using SixLabors.Primitives;
using System.IO;

namespace HeroesData.Tests
{
    [TestClass]
    public class ConvertImageTests
    {
        [TestMethod]
        public void DDSToPNGImageTest()
        {
            string file = "storm_ui_icon_nova_orbitalstrike.dds";

            DDSImage image = new DDSImage(file);
            image.Save(Path.ChangeExtension(file, ".png"));

            Assert.IsTrue(File.Exists(Path.ChangeExtension(file, ".png")));
        }

        [TestMethod]
        public void MVPImageSplitIntoThreeTest()
        {
            string file = "storm_ui_mvp_icons_rewards_loyaldefender.dds";

            string blueAward = Path.ChangeExtension(file.Replace("loyaldefender", "loyaldefender_blue"), ".png");
            string redAward = Path.ChangeExtension(file.Replace("loyaldefender", "loyaldefender_red"), ".png");
            string goldAward = Path.ChangeExtension(file.Replace("loyaldefender", "loyaldefender_gold"), ".png");

            DDSImage image = new DDSImage(file);

            Assert.AreEqual(148, image.Height);
            Assert.AreEqual(444, image.Width);

            int newWidth = 444 / 3;
            image.Save(blueAward, new Point(0, 0), new Size(newWidth, image.Height));
            image.Save(redAward, new Point(newWidth, 0), new Size(newWidth, image.Height));
            image.Save(goldAward, new Point(newWidth * 2, 0), new Size(newWidth, image.Height));

            Assert.IsTrue(File.Exists(blueAward));
            Assert.IsTrue(File.Exists(redAward));
            Assert.IsTrue(File.Exists(goldAward));

            var newImage = Image.Load(blueAward);

            Assert.AreEqual(148, newImage.Height);
            Assert.AreEqual(148, newImage.Width);
        }
    }
}
