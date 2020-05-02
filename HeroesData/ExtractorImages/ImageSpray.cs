using CASCLib;
using Heroes.Models;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.ExtractorImage
{
    public class ImageSpray : ImageExtractorBase<Spray>, IImage
    {
        private readonly int _imageMaxHeight = 256;
        private readonly int _imageMaxWidth = 256;
        private readonly HashSet<Spray> _sprays = new HashSet<Spray>();
        private readonly string _sprayDirectory = "sprays";

        public ImageSpray(CASCHandler? cascHandler, string modsFolderPath)
            : base(cascHandler, modsFolderPath)
        {
        }

        protected override void ExtractFiles()
        {
            if (App.ExtractFileOption.HasFlag(ExtractImageOptions.Spray))
                ExtractSprayImages();
        }

        protected override void LoadFileData(Spray spray)
        {
            if (spray is null)
                throw new ArgumentNullException(nameof(spray));

            if (!string.IsNullOrEmpty(spray.ImageFileName))
                _sprays.Add(spray);
        }

        private void ExtractSprayImages()
        {
            if (_sprays == null || _sprays.Count < 1)
                return;

            int count = 0;
            Console.Write($"Extracting spray image files...{count}/{_sprays.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, _sprayDirectory);

            foreach (Spray spray in _sprays)
            {
                bool success = false;
                string filePath = Path.Combine(extractFilePath, spray.ImageFileName);

                if (ExtractStaticImageFile(filePath))
                    success = true;

                if (success && spray.AnimationCount > 0)
                {
                    success = ExtractAnimatedImageFile(filePath, new Size(_imageMaxWidth, _imageMaxHeight), new Size(_imageMaxWidth, _imageMaxHeight), spray.AnimationCount, spray.AnimationDuration / 2);
                }

                if (success)
                    count++;

                Console.Write($"\rExtracting spray image files...{count}/{_sprays.Count}");
            }

            Console.WriteLine(" Done.");
        }
    }
}
