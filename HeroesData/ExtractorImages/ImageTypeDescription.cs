using CASCLib;
using Heroes.Models;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.ExtractorImages
{
    public class ImageTypeDescription : ImageExtractorBase<TypeDescription>, IImage
    {
        private readonly HashSet<TypeDescription> _typeDescriptions = new HashSet<TypeDescription>();

        private readonly string _typeDescriptionDirectory = "typedescriptions";

        public ImageTypeDescription(CASCHandler? cascHandler, string modsFolderPath)
            : base(cascHandler, modsFolderPath)
        {
        }

        protected override void ExtractFiles()
        {
            if (App.ExtractFileOption.HasFlag(ExtractImageOptions.TypeDescription))
                ExtractTypeDescriptionImages();
        }

        protected override void LoadFileData(TypeDescription data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            if (!string.IsNullOrEmpty(data.TextureSheet.Image))
                _typeDescriptions.Add(data);
        }

        private void ExtractTypeDescriptionImages()
        {
            if (_typeDescriptions == null || _typeDescriptions.Count < 1)
                return;

            int count = 0;
            Console.Write($"Extracting type description image files...{count}/{_typeDescriptions.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, _typeDescriptionDirectory);

            foreach (TypeDescription typeDesciption in _typeDescriptions)
            {
                if (string.IsNullOrEmpty(typeDesciption.TextureSheet.Image))
                    continue;

                string filePath = Path.Combine(extractFilePath, typeDesciption.TextureSheet.Image);
                using DDSImage? originalTextureSheetImage = GetDDSImage(filePath);
                if (originalTextureSheetImage == null)
                    continue;

                int imageHeight = originalTextureSheetImage.Height;
                if (typeDesciption.TextureSheet.Rows != null)
                    imageHeight = originalTextureSheetImage.Height / typeDesciption.TextureSheet.Rows.Value;

                int imageWidth = originalTextureSheetImage.Width;
                if (typeDesciption.TextureSheet.Columns != null)
                    imageWidth = originalTextureSheetImage.Width / typeDesciption.TextureSheet.Columns.Value;

                if (typeDesciption.TextureSheet.Columns.HasValue && !string.IsNullOrEmpty(typeDesciption.ImageFileName))
                {
#pragma warning disable SA1407 // Arithmetic expressions should declare precedence
                    int xPos = typeDesciption.IconSlot % typeDesciption.TextureSheet.Columns.Value * imageWidth;
#pragma warning restore SA1407 // Arithmetic expressions should declare precedence
                    int yPos = typeDesciption.IconSlot / typeDesciption.TextureSheet.Columns.Value * imageHeight;

                    if (!string.IsNullOrEmpty(typeDesciption.TextureSheet.Image) && ExtractStaticImageFile(Path.Combine(extractFilePath, typeDesciption.ImageFileName), typeDesciption.TextureSheet.Image, new Point(xPos, yPos), new Size(imageWidth, imageHeight)))
                        count++;
                }

                Console.Write($"\rExtracting type description image files...{count}/{_typeDescriptions.Count}");
            }

            Console.WriteLine(" Done.");
        }
    }
}
