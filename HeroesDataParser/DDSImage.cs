using Pfim;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Size = SixLabors.ImageSharp.Size;

namespace HeroesDataParser;

public sealed class DDSImage : IDisposable
{
    private readonly Pfim.IImage _ddsImageFile;
    private Image? _decodedImage;

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
        _decodedImage?.Dispose();
    }

    /// <summary>
    /// Writes the image to the file path.
    /// </summary>
    /// <param name="file">The file path the image will be written to. The file extension determine to conversion to perform.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown when the pixel format is unsupported.</exception>
    public Task Save(string file)
    {
        if (_ddsImageFile.Format == ImageFormat.Rgba32)
        {
            return Save<Bgra32>(file);
        }
        else if (_ddsImageFile.Format == ImageFormat.Rgb24)
        {
            return Save<Bgr24>(file);
        }
        else if (_ddsImageFile.Format == ImageFormat.Rgba16)
        {
            return Save<Bgra4444>(file);
        }
        else if (_ddsImageFile.Format == ImageFormat.R5g5b5)
        {
            // Turn the alpha channel on for image sharp
            for (int i = 1; i < _ddsImageFile.Data.Length; i += 2)
            {
                _ddsImageFile.Data[i] |= 128;
            }

            return Save<Bgra5551>(file);
        }
        else if (_ddsImageFile.Format == ImageFormat.R5g5b5a1)
        {
            return Save<Bgra5551>(file);
        }
        else if (_ddsImageFile.Format == ImageFormat.R5g6b5)
        {
            return Save<Bgr565>(file);
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
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown when the pixel format is unsupported.</exception>
    public Task Save(string file, Point point, Size size)
    {
        if (_ddsImageFile.Format == ImageFormat.Rgba32)
        {
            return Save<Bgra32>(file, point, size);
        }
        else if (_ddsImageFile.Format == ImageFormat.Rgb24)
        {
            return Save<Bgr24>(file, point, size);
        }
        else if (_ddsImageFile.Format == ImageFormat.Rgba16)
        {
            return Save<Bgra4444>(file, point, size);
        }
        else if (_ddsImageFile.Format == ImageFormat.R5g5b5)
        {
            // Turn the alpha channel on for image sharp
            for (int i = 1; i < _ddsImageFile.Data.Length; i += 2)
            {
                _ddsImageFile.Data[i] |= 128;
            }

            return Save<Bgra5551>(file, point, size);
        }
        else if (_ddsImageFile.Format == ImageFormat.R5g5b5a1)
        {
            return Save<Bgra5551>(file, point, size);
        }
        else if (_ddsImageFile.Format == ImageFormat.R5g6b5)
        {
            return Save<Bgr565>(file, point, size);
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
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown when the pixel format is unsupported.</exception>
    public Task SaveAsGif(string file, Size size, Size innerSize, int frames, int frameDelay)
    {
        if (Path.GetExtension(file) != ".gif")
            throw new Exception("File is not a gif");

        if (_ddsImageFile.Format == ImageFormat.Rgba32)
        {
            return SaveAsGif<Bgra32>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.Rgb24)
        {
            return SaveAsGif<Bgr24>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.Rgba16)
        {
            return SaveAsGif<Bgra4444>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.R5g5b5)
        {
            // Turn the alpha channel on for image sharp
            for (int i = 1; i < _ddsImageFile.Data.Length; i += 2)
            {
                _ddsImageFile.Data[i] |= 128;
            }

            return SaveAsGif<Bgra5551>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.R5g5b5a1)
        {
            return SaveAsGif<Bgra5551>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.R5g6b5)
        {
            return SaveAsGif<Bgr565>(file, size, innerSize, frames, frameDelay);
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
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown when the pixel format is unsupported.</exception>
    public Task SaveAsAPNG(string file, Size size, Size innerSize, int frames, int frameDelay)
    {
        if (_ddsImageFile.Format == ImageFormat.Rgba32)
        {
            return SaveAsAPNG<Bgra32>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.Rgb24)
        {
            return SaveAsAPNG<Bgr24>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.Rgba16)
        {
            return SaveAsAPNG<Bgra4444>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.R5g5b5)
        {
            // Turn the alpha channel on for image sharp
            for (int i = 1; i < _ddsImageFile.Data.Length; i += 2)
            {
                _ddsImageFile.Data[i] |= 128;
            }

            return SaveAsAPNG<Bgra5551>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.R5g5b5a1)
        {
            return SaveAsAPNG<Bgra5551>(file, size, innerSize, frames, frameDelay);
        }
        else if (_ddsImageFile.Format == ImageFormat.R5g6b5)
        {
            return SaveAsAPNG<Bgr565>(file, size, innerSize, frames, frameDelay);
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
    /// Checks if the image is a solid color in the specified area.
    /// </summary>
    /// <param name="point">The top-left coordinates where the area will be checked.</param>
    /// <param name="size">The size of the area to check.</param>
    /// <returns><see langword="true"/> if the area is a solid color, otherwise <see langword="false"/>.</returns>
    /// <exception cref="Exception">Thrown when the pixel format is unsupported.</exception>
    public bool IsSolidColor(Point point, Size size)
    {
        if (_ddsImageFile.Format == ImageFormat.Rgba32)
        {
            return IsSolidColor<Bgra32>(point, size);
        }
        else if (_ddsImageFile.Format == ImageFormat.Rgb24)
        {
            return IsSolidColor<Bgr24>(point, size);
        }
        else if (_ddsImageFile.Format == ImageFormat.Rgba16)
        {
            return IsSolidColor<Bgra4444>(point, size);
        }
        else if (_ddsImageFile.Format == ImageFormat.R5g5b5)
        {
            // Turn the alpha channel on for image sharp
            for (int i = 1; i < _ddsImageFile.Data.Length; i += 2)
            {
                _ddsImageFile.Data[i] |= 128;
            }

            return IsSolidColor<Bgra5551>(point, size);
        }
        else if (_ddsImageFile.Format == ImageFormat.R5g5b5a1)
        {
            return IsSolidColor<Bgra5551>(point, size);
        }
        else if (_ddsImageFile.Format == ImageFormat.R5g6b5)
        {
            return IsSolidColor<Bgr565>(point, size);
        }
        else
        {
            throw new Exception($"Unsupported pixel format ({_ddsImageFile.Format})");
        }
    }

    private static Task SaveNewFile<T>(string file, Image<T> image)
        where T : unmanaged, IPixel<T>
    {
        string extension = Path.GetExtension(file);
        if (extension == ".png")
        {
            return image.SaveAsync(file, new PngEncoder()
            {
                CompressionLevel = PngCompressionLevel.DefaultCompression, // default
            });
        }
        else if (extension == ".jpg")
        {
            return image.SaveAsync(file, new JpegEncoder()
            {
                Quality = 85,
            });
        }
        else
        {
            throw new Exception($"Unsupported file extension ({extension})");
        }
    }

    private Image<T> GetDecodedImage<T>()
        where T : unmanaged, IPixel<T>
    {
        if (_decodedImage is Image<T> decodedImage)
            return decodedImage;

        Image<T> image = Image.LoadPixelData<T>(_ddsImageFile.Data, _ddsImageFile.Width, _ddsImageFile.Height);

        _decodedImage = image;

        return image;
    }

    private Task Save<T>(string file)
        where T : unmanaged, IPixel<T>
    {
        Image<T> image = GetDecodedImage<T>();

        return SaveNewFile(file, image);
    }

    private Task Save<T>(string file, Point point, Size size)
        where T : unmanaged, IPixel<T>
    {
        Image<T> image = GetDecodedImage<T>();

        using Image<T> cropped = image.Clone(x => x.Crop(new Rectangle(point, size)));

        return SaveNewFile(file, cropped);
    }

    private Task SaveAsGif<T>(string file, Size size, Size innerSize, int frames, int frameDelay)
        where T : unmanaged, IPixel<T>
    {
        int gifFrameDelay = frameDelay / 10;

        // Load full base image
        Image<T> image = GetDecodedImage<T>();

        // with first frame
        using Image<T> gif = image.Clone(x => x.Crop(new Rectangle(new Point(0, 0), size)));

        GifMetadata gifMetadata = gif.Metadata.GetGifMetadata();
        gifMetadata.RepeatCount = 0;

        GifFrameMetadata firstFrameMetadata = gif.Frames.RootFrame.Metadata.GetGifMetadata();
        firstFrameMetadata.FrameDelay = gifFrameDelay;
        firstFrameMetadata.DisposalMode = FrameDisposalMode.RestoreToBackground;

        int calcWidth = image.Width / innerSize.Width;

        // Add remaining frames
        for (int i = 1; i < frames; i++)
        {
            int xPos = (i % calcWidth) * innerSize.Width;
            int yPos = (i / calcWidth) * innerSize.Height;

            using Image<T> imagePart = image.Clone(x => x.Crop(new Rectangle(new Point(xPos, yPos), size)));

            GifFrameMetadata gifFrameMetadata = imagePart.Frames.RootFrame.Metadata.GetGifMetadata();
            gifFrameMetadata.FrameDelay = gifFrameDelay;
            gifFrameMetadata.DisposalMode = FrameDisposalMode.RestoreToBackground;

            gif.Frames.AddFrame(imagePart.Frames.RootFrame);
        }

        return gif.SaveAsync(file, new GifEncoder());
    }

    private Task SaveAsAPNG<T>(string file, Size size, Size innerSize, int frames, int frameDelay)
        where T : unmanaged, IPixel<T>
    {
        Rational apngFrameDelay = new((uint)frameDelay, 1000, false);

        // Load full base image
        Image<T> image = GetDecodedImage<T>();

        // with first frame
        using Image<T> apng = image.Clone(x => x.Crop(new Rectangle(new Point(0, 0), size)));

        PngMetadata pngMetadata = apng.Metadata.GetPngMetadata();
        pngMetadata.RepeatCount = 0;

        PngFrameMetadata firstFrameMetadata = apng.Frames.RootFrame.Metadata.GetPngMetadata();
        firstFrameMetadata.FrameDelay = apngFrameDelay;
        firstFrameMetadata.DisposalMode = FrameDisposalMode.RestoreToBackground;
        firstFrameMetadata.BlendMode = FrameBlendMode.Source;

        int calcWidth = image.Width / innerSize.Width;

        // Add remaining frames
        for (int i = 1; i < frames; i++)
        {
            int xPos = (i % calcWidth) * innerSize.Width;
            int yPos = (i / calcWidth) * innerSize.Height;

            using Image<T> imagePart = image.Clone(x => x.Crop(new Rectangle(new Point(xPos, yPos), size)));

            PngFrameMetadata pngFrameMetadata = imagePart.Frames.RootFrame.Metadata.GetPngMetadata();
            pngFrameMetadata.FrameDelay = apngFrameDelay;
            pngFrameMetadata.DisposalMode = FrameDisposalMode.RestoreToBackground;
            pngFrameMetadata.BlendMode = FrameBlendMode.Source;

            apng.Frames.AddFrame(imagePart.Frames.RootFrame);
        }

        return apng.SaveAsync(file, new PngEncoder()
        {
            ColorType = PngColorType.RgbWithAlpha,
            CompressionLevel = PngCompressionLevel.DefaultCompression,
            FilterMethod = PngFilterMethod.Adaptive,
            InterlaceMethod = PngInterlaceMode.None,
            BitDepth = PngBitDepth.Bit16,
        });
    }

    private bool IsSolidColor<T>(Point point, Size size)
        where T : unmanaged, IPixel<T>
    {
        Image<T> source = GetDecodedImage<T>();

        Rgba32 firstColor = default;
        bool isSolid = true;
        bool first = true;

        source.ProcessPixelRows(accessor =>
        {
            for (int y = point.Y; y < point.Y + size.Height && isSolid; y++)
            {
                Span<T> row = accessor.GetRowSpan(y).Slice(point.X, size.Width);

                for (int x = 0; x < row.Length; x++)
                {
                    Rgba32 pixelColor = row[x].ToRgba32();

                    if (first)
                    {
                        firstColor = pixelColor;
                        first = false;
                        continue;
                    }

                    if (pixelColor.R != firstColor.R || pixelColor.G != firstColor.G || pixelColor.B != firstColor.B)
                    {
                        isSolid = false;
                        break;
                    }
                }
            }
        });

        return isSolid;
    }
}
