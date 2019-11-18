using CASCLib;
using Heroes.Models;
using HeroesData.Helpers;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.ExtractorImage
{
    public abstract class ImageExtractorBase<T>
        where T : IExtractable
    {
        public ImageExtractorBase(CASCHandler? cascHandler, string modsFolderPath)
        {
            CASCHandler = cascHandler;
            ModsFolderPath = modsFolderPath;
        }

        protected CASCHandler? CASCHandler { get; }
        protected StorageMode StorageMode { get; private set; }
        protected string ModsFolderPath { get; }
        protected string ExtractDirectory { get; } = Path.Combine(App.OutputDirectory, "images");
        protected string TexturesPath { get; } = Path.Combine("mods", "heroes.stormmod", "base.stormassets", "assets", "textures");
        protected List<string> FailedFileMessages { get; } = new List<string>();

        public void ExtractFiles(IEnumerable<IExtractable> data)
        {
            if (string.IsNullOrEmpty(App.OutputDirectory) || data == null)
                return;

            if (CASCHandler != null)
                StorageMode = StorageMode.CASC;
            else if (CASCHandler == null && !string.IsNullOrEmpty(ModsFolderPath))
                StorageMode = StorageMode.Mods;

            foreach (T t in data)
            {
                LoadFileData(t);
            }

            ExtractFiles();
            DisplayFailedExtractedFiles();
        }

        protected abstract void LoadFileData(T t);
        protected abstract void ExtractFiles();

        /// <summary>
        /// Extracts a static image file. Returns true if successful.
        /// </summary>
        /// <param name="filePath">The file path the file will be saved to.</param>
        protected bool ExtractStaticImageFile(string filePath)
        {
            string fileName = Path.GetFileName(filePath);

            return ExtractImageFile(filePath, () =>
            {
                string textureFilepath = Path.Combine(TexturesPath, fileName);
                if (FileExists(textureFilepath))
                {
                    using Stream stream = OpenFile(textureFilepath);
                    using DDSImage image = new DDSImage(stream);

                    PathHelper.FileNameToLower(filePath.AsMemory());

                    image.Save(Path.ChangeExtension(filePath, "png"));

                    return true;
                }
                else
                {
                    FailedFileMessages.Add($"File not found: {fileName}");
                    return false;
                }
            });
        }

        /// <summary>
        /// Extracts a static image file. Returns true if successful.
        /// </summary>
        /// <param name="filePath">The file path the file will be saved to.</param>
        /// <param name="fileName">The file name of the original base image.</param>
        /// <param name="point">The point coordinates that the extracted image from the base image.</param>
        /// <param name="size">The size of the extracted image.</param>
        /// <returns></returns>
        protected bool ExtractStaticImageFile(string filePath, string fileName, Point point, Size size)
        {
            return ExtractImageFile(filePath, () =>
            {
                string textureFilepath = Path.Combine(TexturesPath, fileName);
                if (FileExists(textureFilepath))
                {
                    using Stream stream = OpenFile(textureFilepath);
                    using DDSImage image = new DDSImage(stream);

                    PathHelper.FileNameToLower(filePath.AsMemory());

                    image.Save(Path.ChangeExtension(filePath, "png"), point, size);

                    return true;
                }
                else
                {
                    FailedFileMessages.Add($"File not found: {fileName}");
                    return false;
                }
            });
        }

        /// <summary>
        /// Extracts an animated image file. Returns true if successful.
        /// </summary>
        /// <param name="filePath">The file path the file will be saved to.</param>
        /// <param name="size">The size of the extracted image.</param>
        /// <param name="maxSize">The maximum size from the base image. Not the base image size.</param>
        /// <param name="frames">The amount of frames the animated image has.</param>
        /// <param name="frameDelay">The amount of delay for each frame.</param>
        /// <returns></returns>
        protected bool ExtractAnimatedImageFile(string filePath, Size size, Size maxSize, int frames, int frameDelay)
        {
            string fileName = Path.GetFileName(filePath);

            return ExtractImageFile(filePath, () =>
            {
                string textureFilepath = Path.Combine(TexturesPath, fileName);
                if (FileExists(textureFilepath))
                {
                    using Stream stream = OpenFile(textureFilepath);
                    using DDSImage image = new DDSImage(stream);

                    PathHelper.FileNameToLower(filePath.AsMemory());

                    image.SaveAsGif(Path.ChangeExtension(filePath, "gif"), size, maxSize, frames, frameDelay);

                    return true;
                }
                else
                {
                    FailedFileMessages.Add($"File not found: {fileName}");
                    return false;
                }
            });
        }

        protected bool FileExists(string filePath)
        {
            if (StorageMode == StorageMode.CASC)
                return CASCHandler!.FileExists(filePath);
            else if (StorageMode == StorageMode.Mods)
                return File.Exists(Path.Combine(ModsFolderPath, filePath.Substring(5)));
            else
                return false;
        }

        protected Stream OpenFile(string filePath)
        {
            if (StorageMode == StorageMode.CASC)
                return CASCHandler!.OpenFile(filePath);
            else if (StorageMode == StorageMode.Mods)
                return File.Open(Path.Combine(ModsFolderPath, filePath.Substring(5)), FileMode.Open);
            else
                throw new NotSupportedException();
        }

        private bool ExtractImageFile(string filePath, Func<bool> extractImage)
        {
            string fileName = Path.GetFileName(filePath);

            if (Path.GetExtension(fileName) != ".dds")
            {
                FailedFileMessages.Add($"Could not extract image file {fileName} - is not a .dds file!");
                return false;
            }

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                return extractImage();
            }
            catch (Exception ex)
            {
                FailedFileMessages.Add($"Error extracting file: {fileName}");
                FailedFileMessages.Add($"--> {ex.Message}");
            }

            return false;
        }

        private void DisplayFailedExtractedFiles()
        {
            foreach (string failedFileMessages in FailedFileMessages)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(failedFileMessages);
                Console.ResetColor();
            }
        }
    }
}
