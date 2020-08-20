using Microsoft.Extensions.CommandLineUtils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;

namespace HeroesData.Commands
{
    internal class ImageCommand : CommandBase, ICommand
    {
        private const int _defaultWidth = -1;
        private const int _defaultHeight = -1;

        private int _width = _defaultWidth;
        private int _height = _defaultHeight;

        private string _outputDirectory = string.Empty;
        private bool _compress;

        public ImageCommand(CommandLineApplication app)
            : base(app)
        {
        }

        public static ImageCommand Add(CommandLineApplication app)
        {
            return new ImageCommand(app);
        }

        public void SetCommand()
        {
            CommandLineApplication.Command("image", config =>
            {
                config.HelpOption("-?|-h|--help");
                config.Description = "Performs image processing.";

                CommandArgument filePathArgument = config.Argument("file-name", "The filename, file path, or directory containing the images to process.");

                CommandOption dimensionWidthOption = config.Option("--width <VALUE>", "Sets the new width.", CommandOptionType.SingleValue);
                CommandOption dimensionHeightOption = config.Option("--height <VALUE>", "Sets the new height.", CommandOptionType.SingleValue);
                CommandOption pngCompressOption = config.Option("--png-compress", "Sets a png image bit depth to 8 bits", CommandOptionType.NoValue);
                CommandOption outputOption = config.Option("-o|--output-directory <DIRECTORYPATH>", "Sets the output directory.", CommandOptionType.SingleValue);

                config.OnExecute(() =>
                {
                    if (string.IsNullOrEmpty(filePathArgument.Value))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Must provide a file name, relative file path, or directory.");
                        Console.ResetColor();

                        return 0;
                    }

                    if (dimensionWidthOption.HasValue() && (!int.TryParse(dimensionWidthOption.Value(), out _width) || _width <= 0))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid width. Must be an integer greater than 0.");
                        Console.ResetColor();

                        return 0;
                    }

                    if (dimensionHeightOption.HasValue() && (!int.TryParse(dimensionHeightOption.Value(), out _height) || _height <= 0))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid height. Must be an integer greater than 0.");
                        Console.ResetColor();

                        return 0;
                    }

                    if (outputOption.HasValue())
                        _outputDirectory = outputOption.Value();

                    _compress = pngCompressOption.HasValue();

                    if (Directory.Exists(filePathArgument.Value))
                    {
                        int count = 0;
                        foreach (string file in Directory.EnumerateFiles(filePathArgument.Value.TrimEnd(Path.DirectorySeparatorChar)))
                        {
                            if (ProcessImage(file))
                            {
                                count++;
                                Console.Write($"\rProcessed {count}");
                            }
                        }
                    }
                    else if (File.Exists(filePathArgument.Value))
                    {
                        if (ProcessImage(filePathArgument.Value))
                            Console.WriteLine("Image processed.");
                        else
                            Console.WriteLine("Image did not get processed.");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("File or directory does not exist.");
                        Console.ResetColor();
                    }

                    return 0;
                });
            });
        }

        private bool ProcessImage(string filePath)
        {
            string newFilePath = filePath;
            string fileType = Path.GetExtension(newFilePath);

            if (fileType == ".png" || fileType == ".gif")
            {
                try
                {
                    using Image image = Image.Load(filePath);

                    if (_width == _defaultWidth)
                        _width = image.Width;

                    if (_height == _defaultHeight)
                        _height = image.Height;

                    image.Mutate(x => x.Resize(_width, _height));

                    if (!string.IsNullOrEmpty(_outputDirectory))
                    {
                        Directory.CreateDirectory(_outputDirectory);
                        newFilePath = Path.Combine(_outputDirectory, Path.GetFileName(filePath));
                    }

                    if (fileType == ".png" && _compress)
                    {
                        image.Save(newFilePath, new PngEncoder()
                        {
                            BitDepth = PngBitDepth.Bit8,
                            ColorType = PngColorType.Palette,
                        });

                        return true;
                    }
                    else
                    {
                        image.Save(newFilePath);

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{Path.GetFileName(newFilePath)} Unable to process image -> {ex.Message}");
                    Console.ResetColor();
                }
            }

            return false;
        }
    }
}
