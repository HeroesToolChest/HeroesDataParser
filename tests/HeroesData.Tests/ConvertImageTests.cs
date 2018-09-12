using DDSReader;
using System.IO;
using Xunit;

namespace HeroesData.Tests
{
    public class ConvertImageTests
    {
        [Fact]
        public void DDSToPNGImageTest()
        {
            string file = "storm_ui_icon_nova_orbitalstrike.dds";

            DDSImage image = new DDSImage(file);
            image.Save(Path.ChangeExtension(file, ".png"));

            Assert.True(File.Exists(Path.ChangeExtension(file, ".png")));
        }
    }
}
