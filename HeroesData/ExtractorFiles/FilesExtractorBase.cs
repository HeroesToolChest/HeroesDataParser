using CASCLib;
using Heroes.Models;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.ExtractorFiles
{
    public abstract class FilesExtractorBase<T>
        where T : IExtractable
    {
        public FilesExtractorBase(CASCHandler cascHandler, StorageMode storageMode)
        {
            CASCHandler = cascHandler;
            StorageMode = storageMode;
        }

        protected CASCHandler CASCHandler { get; }
        protected StorageMode StorageMode { get; }
        protected string ExtractDirectory { get; } = Path.Combine(App.OutputDirectory, "images");
        protected string CASCTexturesPath { get; } = Path.Combine("mods", "heroes.stormmod", "base.stormassets", "assets", "textures");
        protected List<string> FailedFileMessages { get; } = new List<string>();

        public void ExtractFiles(IEnumerable<T> data)
        {
            if (CASCHandler == null || data == null || StorageMode != StorageMode.CASC || string.IsNullOrEmpty(App.OutputDirectory))
                return;

            foreach (T t in data)
            {
                LoadFileData(t);
            }

            ExtractFiles();
            DisplayFailedExtractedFiles();
        }

        public void ExtractFiles(IEnumerable<IExtractable> data)
        {
            if (CASCHandler == null || data == null || StorageMode != StorageMode.CASC || string.IsNullOrEmpty(App.OutputDirectory))
                return;

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
                string cascFilepath = Path.Combine(CASCTexturesPath, fileName);
                if (CASCHandler.FileExists(cascFilepath))
                {
                    DDSImage image = new DDSImage(CASCHandler.OpenFile(cascFilepath));
                    image.Save(Path.ChangeExtension(filePath, "png"));

                    return true;
                }
                else
                {
                    FailedFileMessages.Add($"CASC file not found: {fileName}");
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
                string cascFilepath = Path.Combine(CASCTexturesPath, fileName);
                if (CASCHandler.FileExists(cascFilepath))
                {
                    DDSImage image = new DDSImage(CASCHandler.OpenFile(cascFilepath));
                    image.Save(Path.ChangeExtension(filePath, "png"), point, size);

                    return true;
                }
                else
                {
                    FailedFileMessages.Add($"CASC file not found: {fileName}");
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
                string cascFilepath = Path.Combine(CASCTexturesPath, fileName);
                if (CASCHandler.FileExists(cascFilepath))
                {
                    DDSImage image = new DDSImage(CASCHandler.OpenFile(cascFilepath));
                    image.SaveAsGif(Path.ChangeExtension(filePath, "gif"), size, maxSize, frames, frameDelay);

                    return true;
                }
                else
                {
                    FailedFileMessages.Add($"CASC file not found: {fileName}");
                    return false;
                }
            });
        }

        private bool ExtractImageFile(string filePath, Func<bool> extractImage)
        {
            if (CASCHandler == null)
                return false;

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
