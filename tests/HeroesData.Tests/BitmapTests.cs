using Imaging.DDSReader;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Xunit;

namespace HeroesData.Tests
{
    public class BitmapTests
    {
        [Fact]
        public void DDSToPNGImageTest()
        {
            string file = "storm_ui_icon_nova_orbitalstrike.dds";

            using (Bitmap image = DDS.LoadImage(file))
            {
                image.Save(Path.ChangeExtension(file, ".png"), ImageFormat.Png);
            }

            Assert.True(File.Exists(Path.ChangeExtension(file, ".png")));
        }
    }
}
