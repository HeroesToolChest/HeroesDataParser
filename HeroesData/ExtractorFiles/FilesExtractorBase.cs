using CASCLib;
using Heroes.Models;
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
        /// Extracts a file. Returns true if successful.
        /// </summary>
        /// <param name="path">The path to extract the file to.</param>
        /// <param name="fileName">The name of the file to extract.</param>
        protected bool ExtractImageFile(string path, string fileName)
        {
            if (CASCHandler == null)
                return false;

            if (Path.GetExtension(fileName) != ".dds")
            {
                FailedFileMessages.Add($"Could not extract image file {fileName} - is not a .dds file!");
                return false;
            }

            try
            {
                Directory.CreateDirectory(path);

                string cascFilepath = Path.Combine(CASCTexturesPath, fileName);
                if (CASCHandler.FileExists(cascFilepath))
                {
                    DDSImage image = new DDSImage(CASCHandler.OpenFile(cascFilepath));
                    image.Save(Path.Combine(path, $"{Path.GetFileNameWithoutExtension(fileName)}.png"));

                    return true;
                }
                else
                {
                    FailedFileMessages.Add($"CASC file not found: {fileName}");
                }
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
