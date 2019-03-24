using Pfim;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System;
using System.IO;

namespace HeroesData
{
    public class DDSImage
    {
        private readonly Pfim.IImage DDSImageFile;

        public DDSImage(string file)
        {
            DDSImageFile = Pfim.Pfim.FromFile(file, new PfimConfig(decompress: true));
        }

        public DDSImage(Stream stream)
        {
            if (stream == null)
                throw new Exception($"DDSImage ctor: {nameof(stream)} is null");

            DDSImageFile = Dds.Create(stream, new PfimConfig(decompress: true));
        }

        public int Width => DDSImageFile.Width;

        public int Height => DDSImageFile.Height;

        public void Save(string file)
        {
            if (DDSImageFile.Format == ImageFormat.Rgba32)
                Save<Bgra32>(file);
            else if (DDSImageFile.Format == ImageFormat.Rgb24)
                Save<Bgr24>(file);
            else
                throw new Exception("Unsupported pixel format (" + DDSImageFile.Format + ")");
        }

        public void Save(string file, Point point, Size size)
        {
            if (DDSImageFile.Format == ImageFormat.Rgba32)
                Save<Bgra32>(file, point, size);
            else if (DDSImageFile.Format == ImageFormat.Rgb24)
                Save<Bgr24>(file, point, size);
            else
                throw new Exception("Unsupported pixel format (" + DDSImageFile.Format + ")");
        }

        private void Save<T>(string file)
            where T : struct, IPixel<T>
        {
            Image<T> image = Image.LoadPixelData<T>(
                DDSImageFile.Data, DDSImageFile.Width, DDSImageFile.Height);

            image.Save(file);
        }

        private void Save<T>(string file, Point point, Size size)
            where T : struct, IPixel<T>
        {
            Image<T> image = Image.LoadPixelData<T>(
                DDSImageFile.Data, DDSImageFile.Width, DDSImageFile.Height);

            image.Mutate(x => x.Crop(new Rectangle(point, size)));
            image.Save(file);
        }
    }
}
