using CASCLib;
using Heroes.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.ExtractorFiles
{
    public class FilesSpray : FilesExtractorBase<Spray>, IFile
    {
        private readonly SortedSet<string> Sprays = new SortedSet<string>();

        private readonly string SprayDirectory = "sprays";

        public FilesSpray(CASCHandler cascHandler, StorageMode storageMode)
            : base(cascHandler, storageMode)
        {
        }

        protected override void ExtractFiles()
        {
            if (App.ExtractFileOption.HasFlag(ExtractFileOption.Sprays))
                ExtractSprayImages();
        }

        protected override void LoadFileData(Spray spray)
        {
            if (!string.IsNullOrEmpty(spray.ImageFileName))
                Sprays.Add(spray.ImageFileName.ToLower());
        }

        private void ExtractSprayImages()
        {
            if (Sprays == null || Sprays.Count < 1)
                return;

            int count = 0;
            Console.Write($"Extracting spray image files...{count}/{Sprays.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, SprayDirectory);

            foreach (string spray in Sprays)
            {
                if (ExtractImageFile(extractFilePath, spray))
                    count++;

                Console.Write($"\rExtracting spray image files...{count}/{Sprays.Count}");
            }

            Console.WriteLine(" Done.");
        }
    }
}
