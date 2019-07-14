using Microsoft.Extensions.CommandLineUtils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;

namespace HeroesData.Commands
{
    internal class ImageCommand : CommandBase, ICommand
    {
        private const int DefaultWidth = -1;
        private const int DefaultHeight = -1;

        private int Width = DefaultWidth;
        private int Height = DefaultHeight;

        private string OutputDirectory;
        private bool Compress = false;

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
                CommandOption pngCompressOption = config.Option("--png-compress", "Sets an png image bit depth to 8 bits", CommandOptionType.NoValue);
                CommandOption outputOption = config.Option("-o|--output-directory <FILEPATH>", "Sets the output directory.", CommandOptionType.SingleValue);

                config.OnExecute(() =>
                {
                    if (string.IsNullOrEmpty(filePathArgument.Value))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Must provide a file name, relative file path, or directory.");
                        Console.ResetColor();

                        return 0;
                    }

                    if (dimensionWidthOption.HasValue() && (!int.TryParse(dimensionWidthOption.Value(), out Width) || Width <= 0))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid width. Must be an integer greater than 0.");
                        Console.ResetColor();

                        return 0;
                    }

                    if (dimensionHeightOption.HasValue() && (!int.TryParse(dimensionHeightOption.Value(), out Height) || Height <= 0))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid height. Must be an integer greater than 0.");
                        Console.ResetColor();

                        return 0;
                    }

                    if (outputOption.HasValue())
                        OutputDirectory = outputOption.Value();

                    Compress = pngCompressOption.HasValue();

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
                    using (Image<Rgba32> image = Image.Load(filePath))
                    {
                        if (Width == DefaultWidth)
                            Width = image.Width;

                        if (Height == DefaultHeight)
                            Height = image.Height;

                        image.Mutate(x => x.Resize(Width, Height));

                        if (!string.IsNullOrEmpty(OutputDirectory))
                        {
                            Directory.CreateDirectory(OutputDirectory);
                            newFilePath = Path.Combine(OutputDirectory, Path.GetFileName(filePath));
                        }

                        if (fileType == ".png" && Compress)
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
