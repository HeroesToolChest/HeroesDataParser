using CASCLib;
using DDSReader;
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

        public void ExtractFiles(IEnumerable<T> data)
        {
            if (CASCHandler == null || data == null || StorageMode != StorageMode.CASC || string.IsNullOrEmpty(App.OutputDirectory))
                return;

            foreach (T t in data)
            {
                LoadFileData(t);
            }

            ExtractFiles();
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
        }

        protected abstract void LoadFileData(T t);
        protected abstract void ExtractFiles();

        /// <summary>
        /// Extracts a file.
        /// </summary>
        /// <param name="path">The path to extract the file to.</param>
        /// <param name="fileName">The name of the file to extract.</param>
        protected void ExtractImageFile(string path, string fileName)
        {
            if (CASCHandler == null)
                return;

            if (Path.GetExtension(fileName) != ".dds")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine($"Could not extract image file {fileName} - is not a .dds file!");
                Console.ResetColor();
                return;
            }

            Directory.CreateDirectory(path);

            try
            {
                string cascFilepath = Path.Combine(CASCTexturesPath, fileName);
                if (CASCHandler.FileExists(cascFilepath))
                {
                    DDSImage image = new DDSImage(CASCHandler.OpenFile(cascFilepath));
                    image.Save(Path.Combine(path, $"{Path.GetFileNameWithoutExtension(fileName)}.png"));
                }
                else
                {
                    Console.WriteLine($"CASC file not found: {fileName}");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine($"Error extracting file: {fileName}");
                Console.WriteLine($"--> {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
