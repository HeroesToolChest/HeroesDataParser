using Pfim;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace HeroesDataParser;

public sealed class DDSImage : IDisposable
{
    private readonly IImage _ddsImageFile;

    public DDSImage(string file)
    {
        _ddsImageFile = Pfimage.FromFile(file, new PfimConfig(decompress: true));
    }

    public DDSImage(Stream stream)
    {
        _ddsImageFile = Dds.Create(stream, new PfimConfig(decompress: true));
    }

    public int Width => _ddsImageFile.Width;

    public int Height => _ddsImageFile.Height;

    public void Dispose()
    {
        _ddsImageFile.Dispose();
    }

    /// <summary>
    /// Writes the image to the file path.
    /// </summary>
    /// <param name="file">The file path the image will be written to. The file extension determine to conversion to perform.</param>
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
            throw new Exception($"Unsupported pixel format ({_ddsImageFile.Format})");
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
            throw new Exception($"Unsupported pixel format ({_ddsImageFile.Format})");
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
    /// <param name="innerSize">The size of the inner images on the base image.</param>
    /// <param name="frames">The amount of frames.</param>
    /// <param name="frameDelay">The delay of each frame.</param>
    public void SaveAsGif(string file, Size size, Size innerSize, int frames, int frameDelay)
    {
        if (Path.GetExtension(file) != ".gif")
            throw new Exception("File is not a gif");

        if (_ddsImageFile.Format == ImageFormat.Rgba32)
        {
            SaveAsGif<Bgra32>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.Rgb24)
        {
            SaveAsGif<Bgr24>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.Rgba16)
        {
            SaveAsGif<Bgra4444>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.R5g5b5)
        {
            // Turn the alpha channel on for image sharp
            for (int i = 1; i < _ddsImageFile.Data.Length; i += 2)
            {
                _ddsImageFile.Data[i] |= 128;
            }

            SaveAsGif<Bgra5551>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.R5g5b5a1)
        {
            SaveAsGif<Bgra5551>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.R5g6b5)
        {
            SaveAsGif<Bgr565>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.Rgb8)
        {
            throw new Exception($"Unsupported pixel format ({_ddsImageFile.Format})");
        }
        else
        {
            throw new Exception($"Unsupported pixel format ({_ddsImageFile.Format})");
        }
    }

    /// <summary>
    /// Writes the image to the file path as an apng.
    /// </summary>
    /// <param name="file">The file path the image will be written to.</param>
    /// <param name="size">The size of the new image.</param>
    /// <param name="innerSize">The size of the inner images on the base image.</param>
    /// <param name="frames">The amount of frames.</param>
    /// <param name="frameDelay">The delay of each frame.</param>
    public void SaveAsAPNG(string file, Size size, Size innerSize, int frames, int frameDelay)
    {
        if (_ddsImageFile.Format == ImageFormat.Rgba32)
        {
            SaveAsAPNG<Bgra32>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.Rgb24)
        {
            SaveAsAPNG<Bgr24>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.Rgba16)
        {
            SaveAsAPNG<Bgra4444>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.R5g5b5)
        {
            // Turn the alpha channel on for image sharp
            for (int i = 1; i < _ddsImageFile.Data.Length; i += 2)
            {
                _ddsImageFile.Data[i] |= 128;
            }

            SaveAsAPNG<Bgra5551>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.R5g5b5a1)
        {
            SaveAsAPNG<Bgra5551>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.R5g6b5)
        {
            SaveAsAPNG<Bgr565>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.Rgb8)
        {
            throw new Exception($"Unsupported pixel format ({_ddsImageFile.Format})");
        }
        else
        {
            throw new Exception($"Unsupported pixel format ({_ddsImageFile.Format})");
        }
    }

    private static void SaveNewFile<T>(string file, Image<T> image)
        where T : unmanaged, IPixel<T>
    {
        string extension = Path.GetExtension(file);
        if (extension == ".png")
        {
            image.Save(file, new PngEncoder()
            {
                CompressionLevel = PngCompressionLevel.DefaultCompression, // default
            });
        }
        else if (extension == ".jpg")
        {
            image.Save(file, new JpegEncoder()
            {
                Quality = 85,
            });
        }
        else
        {
            throw new Exception($"Unsupported file extension ({extension})");
        }
    }

    private void Save<T>(string file)
        where T : unmanaged, IPixel<T>
    {
        using Image<T> image = Image.LoadPixelData<T>(_ddsImageFile.Data, _ddsImageFile.Width, _ddsImageFile.Height);

        SaveNewFile(file, image);
    }

    private void Save<T>(string file, Point point, Size size)
        where T : unmanaged, IPixel<T>
    {
        using Image<T> image = Image.LoadPixelData<T>(_ddsImageFile.Data, _ddsImageFile.Width, _ddsImageFile.Height);

        image.Mutate(x => x.Crop(new Rectangle(point, size)));

        SaveNewFile(file, image);
    }

    private void SaveAsGif<T>(string file, Size size, Size innerSize, int frames, int frameDelay)
        where T : unmanaged, IPixel<T>
    {
        // Load full base image
        using Image<T> image = Image.LoadPixelData<T>(_ddsImageFile.Data, _ddsImageFile.Width, _ddsImageFile.Height);
        using Image<T> gif = new(size.Width, size.Height);

        GifMetadata gifMetadata = gif.Metadata.GetGifMetadata();
        gifMetadata.RepeatCount = 0;

        for (int i = 0; i < frames; i++)
        {
#pragma warning disable SA1407 // Arithmetic expressions should declare precedence
            int xPos = i % (image.Width / innerSize.Width) * innerSize.Width;
#pragma warning restore SA1407 // Arithmetic expressions should declare precedence
            int yPos = i / (image.Width / innerSize.Width) * innerSize.Height;

            using Image<T> imagePart = image.Clone();

            imagePart.Mutate(x => x.Crop(new Rectangle(new Point(xPos, yPos), size)));

            GifFrameMetadata gifFrameMetadata = imagePart.Frames.RootFrame.Metadata.GetGifMetadata();
            gifFrameMetadata.FrameDelay = frameDelay / 10;

            gif.Frames.AddFrame(imagePart.Frames.RootFrame);
        }

        gif.Frames.RemoveFrame(0);
        gif.Save(file, new GifEncoder()
        {
            ColorTableMode = GifColorTableMode.Local,
        });
    }

    private void SaveAsAPNG<T>(string file, Size size, Size innerSize, int frames, int frameDelay)
        where T : unmanaged, IPixel<T>
    {
        if (Path.GetExtension(file) != ".apng")
            throw new Exception("File is not an apng");

        // Load full base image
        using Image<T> image = Image.LoadPixelData<T>(_ddsImageFile.Data, _ddsImageFile.Width, _ddsImageFile.Height);
        using Image<T> apng = new(size.Width, size.Height);

        PngMetadata pngMetadata = apng.Metadata.GetPngMetadata();
        pngMetadata.RepeatCount = 0;
        pngMetadata.AnimateRootFrame = false;

        for (int i = 0; i < frames; i++)
        {
#pragma warning disable SA1407 // Arithmetic expressions should declare precedence
            int xPos = i % (image.Width / innerSize.Width) * innerSize.Width;
#pragma warning restore SA1407 // Arithmetic expressions should declare precedence
            int yPos = i / (image.Width / innerSize.Width) * innerSize.Height;

            using Image<T> imagePart = image.Clone();

            imagePart.Mutate(x => x.Crop(new Rectangle(new Point(xPos, yPos), size)));

            PngFrameMetadata pngFrameMetadata = imagePart.Frames.RootFrame.Metadata.GetPngMetadata();
            pngFrameMetadata.FrameDelay = new Rational((uint)frameDelay, 1000, false);

            apng.Frames.AddFrame(imagePart.Frames.RootFrame);
        }

        apng.Save(file, new PngEncoder()
        {
            ColorType = PngColorType.RgbWithAlpha,
            CompressionLevel = PngCompressionLevel.DefaultCompression,
            FilterMethod = PngFilterMethod.Adaptive,
            InterlaceMethod = PngInterlaceMode.None,
            BitDepth = PngBitDepth.Bit8,
        });
    }
}
