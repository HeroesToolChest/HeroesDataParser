using CASCLib;
using Heroes.Models;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.ExtractorImages
{
    public class ImageEmoticon : ImageExtractorBase<Emoticon>, IImage
    {
        private readonly HashSet<Emoticon> _emoticons = new HashSet<Emoticon>();
        private readonly string _emoticonDirectory = "emoticons";

        public ImageEmoticon(CASCHandler? cascHandler, string modsFolderPath)
            : base(cascHandler, modsFolderPath)
        {
        }

        protected override void ExtractFiles()
        {
            if (App.ExtractFileOption.HasFlag(ExtractImageOptions.Emoticon))
                ExtractEmoticonImages();
        }

        protected override void LoadFileData(Emoticon data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            if (!string.IsNullOrEmpty(data.TextureSheet.Image))
                _emoticons.Add(data);
        }

        private void ExtractEmoticonImages()
        {
            if (_emoticons == null || _emoticons.Count < 1)
                return;

            int count = 0;
            Console.Write($"Extracting emoticon image files...{count}/{_emoticons.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, _emoticonDirectory);

            foreach (Emoticon emoticon in _emoticons)
            {
                if (string.IsNullOrEmpty(emoticon.TextureSheet.Image))
                    continue;

                string filePath = Path.Combine(extractFilePath, emoticon.TextureSheet.Image);
                using DDSImage? originalTextureSheetImage = GetDDSImage(filePath);
                if (originalTextureSheetImage == null)
                    continue;

                int imageHeight = originalTextureSheetImage.Height;
                if (emoticon.TextureSheet.Rows != null)
                    imageHeight = originalTextureSheetImage.Height / emoticon.TextureSheet.Rows.Value;

                int imageWidth = originalTextureSheetImage.Width;
                if (emoticon.TextureSheet.Columns != null)
                    imageWidth = originalTextureSheetImage.Width / emoticon.TextureSheet.Columns.Value;

                if (emoticon.Image.Count.HasValue && emoticon.Image.DurationPerFrame != null)
                {
                    if (ExtractAnimatedImageFile(filePath, originalTextureSheetImage, new Size(emoticon.Image.Width, imageHeight), new Size(imageWidth, imageHeight), emoticon.Image.Count.Value, emoticon.Image.DurationPerFrame.Value) &&
                        ExtractStaticImageFile(filePath, originalTextureSheetImage))
                    {
                        count++;
                    }
                }
                else if (emoticon.TextureSheet.Columns.HasValue)
                {
#pragma warning disable SA1407 // Arithmetic expressions should declare precedence
                    int xPos = emoticon.Image.Index % emoticon.TextureSheet.Columns.Value * imageWidth;
#pragma warning restore SA1407 // Arithmetic expressions should declare precedence
                    int yPos = emoticon.Image.Index / emoticon.TextureSheet.Columns.Value * imageHeight;

                    if (!string.IsNullOrEmpty(emoticon.Image.FileName) && ExtractStaticImageFile(Path.Combine(extractFilePath, emoticon.Image.FileName), emoticon.TextureSheet.Image, new Point(xPos, yPos), new Size(emoticon.Image.Width, imageHeight)))
                        count++;
                }

                Console.Write($"\rExtracting emoticon image files...{count}/{_emoticons.Count}");
            }

            Console.WriteLine(" Done.");
        }
    }
}
