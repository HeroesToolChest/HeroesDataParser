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
    public sealed class DDSImage : IDisposable
    {
        private readonly Pfim.IImage _ddsImageFile;

        public DDSImage(string file)
        {
            _ddsImageFile = Pfim.Pfim.FromFile(file, new PfimConfig(decompress: true));
        }

        public DDSImage(Stream stream)
        {
            if (stream == null)
                throw new Exception($"DDSImage ctor: {nameof(stream)} is null");

            _ddsImageFile = Dds.Create(stream, new PfimConfig(decompress: true));
        }

        public int Width => _ddsImageFile.Width;

        public int Height => _ddsImageFile.Height;

        public void Dispose()
        {
            _ddsImageFile?.Dispose();
        }

        /// <summary>
        /// Writes the image to the file path.
        /// </summary>
        /// <param name="file">The file path the image will be written to.</param>
        public void Save(string file)
        {
            if (_ddsImageFile.Format == ImageFormat.Rgba32)
            {
                Save<Bgra32>(file);
            }
            else if (_ddsImageFile.Format == ImageFormat.Rgb24)
            {
                Save<Bgr24>(file);
            }
            else if (_ddsImageFile.Format == ImageFormat.Rgba16)
            {
                Save<Bgra4444>(file);
            }
            else if (_ddsImageFile.Format == ImageFormat.R5g5b5)
            {
                // Turn the alpha channel on for image sharp
                for (int i = 1; i < _ddsImageFile.Data.Length; i += 2)
                {
                    _ddsImageFile.Data[i] |= 128;
                }

                Save<Bgra5551>(file);
            }
            else if (_ddsImageFile.Format == ImageFormat.R5g5b5a1)
            {
                Save<Bgra5551>(file);
            }
            else if (_ddsImageFile.Format == ImageFormat.R5g6b5)
            {
                Save<Bgr565>(file);
            }
            else if (_ddsImageFile.Format == ImageFormat.Rgb8)
            {
                Save<Gray8>(file);
            }
            else
            {
                throw new Exception($"Unsupported pixel format ({_ddsImageFile.Format})");
            }
        }

        /// <summary>
        /// Writes the image to the file path.
        /// </summary>
        /// <param name="file">The file path the image will be written to.</param>
        /// <param name="point">The coordinates where the image will be cropped from.</param>
        /// <param name="size">The size of the new image.</param>
        public void Save(string file, Point point, Size size)
        {
            if (_ddsImageFile.Format == ImageFormat.Rgba32)
            {
                Save<Bgra32>(file, point, size);
            }
            else if (_ddsImageFile.Format == ImageFormat.Rgb24)
            {
                Save<Bgr24>(file, point, size);
            }
            else if (_ddsImageFile.Format == ImageFormat.Rgba16)
            {
                Save<Bgra4444>(file, point, size);
            }
            else if (_ddsImageFile.Format == ImageFormat.R5g5b5)
            {
                // Turn the alpha channel on for image sharp
                for (int i = 1; i < _ddsImageFile.Data.Length; i += 2)
                {
                    _ddsImageFile.Data[i] |= 128;
                }

                Save<Bgra5551>(file, point, size);
            }
            else if (_ddsImageFile.Format == ImageFormat.R5g5b5a1)
            {
                Save<Bgra5551>(file, point, size);
            }
            else if (_ddsImageFile.Format == ImageFormat.R5g6b5)
            {
                Save<Bgr565>(file, point, size);
            }
            else if (_ddsImageFile.Format == ImageFormat.Rgb8)
            {
                Save<Gray8>(file, point, size);
            }
            else
            {
                throw new Exception($"Unsupported pixel format ({_ddsImageFile.Format})");
            }
        }

        /// <summary>
        /// Writes the image to the file path as a gif.
        /// </summary>
        /// <param name="file">The file path the image will be written to.</param>
        /// <param name="size">The size of the new image.</param>
        /// <param name="maxSize">The maximum size from the base image. Not the base image size.</param>
        /// <param name="frames">The amount of frames.</param>
        /// <param name="frameDelay">The delay of each frame.</param>
        public void SaveAsGif(string file, Size size, Size maxSize, int frames, int frameDelay)
        {
            if (Path.GetExtension(file) != ".gif")
                throw new Exception("File is not a gif");

            if (_ddsImageFile.Format == ImageFormat.Rgba32)
            {
                SaveAsGif<Bgra32>(file, size, maxSize, frames, frameDelay);
            }
            else if (_ddsImageFile.Format == ImageFormat.Rgb24)
            {
                SaveAsGif<Bgr24>(file, size, maxSize, frames, frameDelay);
            }
            else if (_ddsImageFile.Format == ImageFormat.Rgba16)
            {
                SaveAsGif<Bgra4444>(file, size, maxSize, frames, frameDelay);
            }
            else if (_ddsImageFile.Format == ImageFormat.R5g5b5)
            {
                // Turn the alpha channel on for image sharp
                for (int i = 1; i < _ddsImageFile.Data.Length; i += 2)
                {
                    _ddsImageFile.Data[i] |= 128;
                }

                SaveAsGif<Bgra5551>(file, size, maxSize, frames, frameDelay);
            }
            else if (_ddsImageFile.Format == ImageFormat.R5g5b5a1)
            {
                SaveAsGif<Bgra5551>(file, size, maxSize, frames, frameDelay);
            }
            else if (_ddsImageFile.Format == ImageFormat.R5g6b5)
            {
                SaveAsGif<Bgr565>(file, size, maxSize, frames, frameDelay);
            }
            else if (_ddsImageFile.Format == ImageFormat.Rgb8)
            {
                SaveAsGif<Gray8>(file, size, maxSize, frames, frameDelay);
            }
            else
            {
                throw new Exception($"Unsupported pixel format ({_ddsImageFile.Format})");
            }
        }

        private void Save<T>(string file)
            where T : struct, IPixel<T>
        {
            using Image<T> image = Image.LoadPixelData<T>(_ddsImageFile.Data, _ddsImageFile.Width, _ddsImageFile.Height);

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

        private void Save<T>(string file, Point point, Size size)
            where T : struct, IPixel<T>
        {
            using Image<T> image = Image.LoadPixelData<T>(_ddsImageFile.Data, _ddsImageFile.Width, _ddsImageFile.Height);

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

        private void SaveAsGif<T>(string file, Size size, Size maxSize, int frames, int frameDelay)
            where T : struct, IPixel<T>
        {
            // Load full base image
            using Image<T> image = Image.LoadPixelData<T>(_ddsImageFile.Data, _ddsImageFile.Width, _ddsImageFile.Height);
            using Image<T> gif = new Image<T>(size.Width, size.Height);

            for (int i = 0; i < frames; i++)
            {
#pragma warning disable SA1407 // Arithmetic expressions should declare precedence
                int xPos = i % (image.Width / maxSize.Width) * maxSize.Width;
#pragma warning restore SA1407 // Arithmetic expressions should declare precedence
                int yPos = i / (image.Width / maxSize.Width) * maxSize.Height;

                using Image<T> imagePart = image.Clone();

                imagePart.Mutate(x => x.Crop(new Rectangle(new Point(xPos, yPos), size)));

                GifFrameMetadata gifFrameMetadata = imagePart.Frames[0].Metadata.GetFormatMetadata(GifFormat.Instance);
                gifFrameMetadata.FrameDelay = frameDelay / 10;
                gifFrameMetadata.DisposalMethod = GifDisposalMethod.RestoreToBackground;

                gif.Frames.AddFrame(imagePart.Frames[0]);
            }

            gif.Frames.RemoveFrame(0);
            gif.Save(file, new GifEncoder()
            {
                ColorTableMode = GifColorTableMode.Local,
            });
        }
    }
}
