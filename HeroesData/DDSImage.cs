using Pfim;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
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

        /// <summary>
        /// Writes the image to the file path.
        /// </summary>
        /// <param name="file">The file path the image will be written to.</param>
        public void Save(string file)
        {
            if (DDSImageFile.Format == ImageFormat.Rgba32)
                Save<Bgra32>(file);
            else if (DDSImageFile.Format == ImageFormat.Rgb24)
                Save<Bgr24>(file);
            else
                throw new Exception("Unsupported pixel format (" + DDSImageFile.Format + ")");
        }

        /// <summary>
        /// Writes the image to the file path.
        /// </summary>
        /// <param name="file">The file path the image will be written to.</param>
        /// <param name="point">The coordinates where the image will be cropped from.</param>
        /// <param name="size">The size of the new image.</param>
        public void Save(string file, Point point, Size size)
        {
            if (DDSImageFile.Format == ImageFormat.Rgba32)
                Save<Bgra32>(file, point, size);
            else if (DDSImageFile.Format == ImageFormat.Rgb24)
                Save<Bgr24>(file, point, size);
            else
                throw new Exception("Unsupported pixel format (" + DDSImageFile.Format + ")");
        }

        /// <summary>
        /// Writes the image to the file path as a gif.
        /// </summary>
        /// <param name="file">The file path the image will be written to.</param>
        /// <param name="size">The size of the new image.</param>
        /// <param name="maxSize"></param>
        /// <param name="frames"></param>
        /// <param name="frameDelay"></param>
        public void SaveAsGif(string file, Size size, Size maxSize, int frames, int frameDelay)
        {
            if (Path.GetExtension(file) != ".gif")
                throw new Exception("File is not a gif");

            if (DDSImageFile.Format == ImageFormat.Rgba32)
                SaveAsGif<Bgra32>(file, size, maxSize, frames, frameDelay);
            else if (DDSImageFile.Format == ImageFormat.Rgb24)
                SaveAsGif<Bgr24>(file, size, maxSize, frames, frameDelay);
            else
                throw new Exception("Unsupported pixel format (" + DDSImageFile.Format + ")");
        }

        private void Save<T>(string file)
            where T : struct, IPixel<T>
        {
            using (Image<T> image = Image.LoadPixelData<T>(DDSImageFile.Data, DDSImageFile.Width, DDSImageFile.Height))
            {
                string extension = Path.GetExtension(file);
                if (extension == ".png")
                {
                    image.Save(file, new PngEncoder()
                    {
                        CompressionLevel = 6, // default
                    });
                }
                else if (extension == ".jpg")
                {
                    image.Save(file, new JpegEncoder()
                    {
                        Quality = 85,
                    });
                }
            }
        }

        private void Save<T>(string file, Point point, Size size)
            where T : struct, IPixel<T>
        {
            using (Image<T> image = Image.LoadPixelData<T>(DDSImageFile.Data, DDSImageFile.Width, DDSImageFile.Height))
            {
                image.Mutate(x => x.Crop(new Rectangle(point, size)));
                string extension = Path.GetExtension(file);
                if (extension == ".png")
                {
                    image.Save(file, new PngEncoder()
                    {
                        CompressionLevel = 6, // default
                    });
                }
                else if (extension == ".jpg")
                {
                    image.Save(file, new JpegEncoder()
                    {
                        Quality = 85,
                    });
                }
            }
        }

        private void SaveAsGif<T>(string file, Size size, Size maxSize, int frames, int frameDelay)
            where T : struct, IPixel<T>
        {
            // Load full base image
            using (Image<T> image = Image.LoadPixelData<T>(DDSImageFile.Data, DDSImageFile.Width, DDSImageFile.Height))
            {
                using (Image<T> gif = new Image<T>(size.Width, size.Height))
                {
                    for (int i = 0; i < frames; i++)
                    {
                        int xPos = (i % (image.Bounds().Right / maxSize.Width)) * maxSize.Width;
                        int yPos = (i / (image.Bounds().Right / maxSize.Width)) * maxSize.Height;

                        using (Image<T> imagePart = image.Clone())
                        {
                            imagePart.Mutate(x => x.Crop(new Rectangle(new Point(xPos, yPos), size)));

                            GifFrameMetaData gifFrameMetaData = imagePart.Frames[0].MetaData.GetFormatMetaData(GifFormat.Instance);
                            gifFrameMetaData.FrameDelay = frameDelay / 10;
                            gifFrameMetaData.DisposalMethod = GifDisposalMethod.RestoreToBackground;

                            gif.Frames.AddFrame(imagePart.Frames[0]);
                        }
                    }

                    gif.Frames.RemoveFrame(0);
                    gif.Save(file, new GifEncoder()
                    {
                        ColorTableMode = GifColorTableMode.Local,
                    });
                }
            }
        }
    }
}
