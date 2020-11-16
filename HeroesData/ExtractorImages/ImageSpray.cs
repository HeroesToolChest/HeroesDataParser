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

            if (!string.IsNullOrEmpty(spray.TextureSheet.Image))
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
                if (string.IsNullOrEmpty(spray.TextureSheet.Image))
                    continue;

                bool success = false;
                string filePath = Path.Combine(extractFilePath, spray.TextureSheet.Image);

                using DDSImage? originalTextureSheetImage = GetDDSImage(filePath);
                if (originalTextureSheetImage == null)
                    continue;

                int imageHeight = originalTextureSheetImage.Height;
                if (spray.TextureSheet.Rows != null)
                    imageHeight = originalTextureSheetImage.Height / spray.TextureSheet.Rows.Value;

                int imageWidth = originalTextureSheetImage.Width;
                if (spray.AnimationCount > 0)
                    imageWidth = originalTextureSheetImage.Width / spray.AnimationCount;

                if (ExtractStaticImageFile(filePath, originalTextureSheetImage))
                    success = true;

                if (success && spray.AnimationCount > 0)
                {
                    success = ExtractAnimatedImageFile(filePath, originalTextureSheetImage, new Size(imageWidth, imageHeight), new Size(imageWidth, imageHeight), spray.AnimationCount, spray.AnimationDuration / 2);
                }

                if (success)
                    count++;

                Console.Write($"\rExtracting spray image files...{count}/{_sprays.Count}");
            }

            Console.WriteLine(" Done.");
        }
    }
}
