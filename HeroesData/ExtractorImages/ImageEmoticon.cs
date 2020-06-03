using CASCLib;
using Heroes.Models;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.ExtractorImage
{
    public class ImageEmoticon : ImageExtractorBase<Emoticon>, IImage
    {
        private readonly int _imageMaxHeight = 32;
        private readonly int _imageMaxWidth = 40;
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

        protected override void LoadFileData(Emoticon emoticon)
        {
            if (emoticon is null)
                throw new ArgumentNullException(nameof(emoticon));

            if (!string.IsNullOrEmpty(emoticon.TextureSheet.Image))
                _emoticons.Add(emoticon);
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
                if (emoticon.Image.Count.HasValue && emoticon.Image.DurationPerFrame != null)
                {
                    if (ExtractAnimatedImageFile(Path.Combine(extractFilePath, emoticon.TextureSheet.Image), new Size(emoticon.Image.Width, _imageMaxHeight), new Size(_imageMaxWidth, _imageMaxHeight), emoticon.Image.Count.Value, emoticon.Image.DurationPerFrame.Value) &&
                        ExtractStaticImageFile(Path.Combine(extractFilePath, emoticon.TextureSheet.Image)))
                    {
                        count++;
                    }
                }
                else if (emoticon.TextureSheet.Columns.HasValue)
                {
#pragma warning disable SA1407 // Arithmetic expressions should declare precedence
                    int xPos = emoticon.Image.Index % emoticon.TextureSheet.Columns.Value * _imageMaxWidth;
#pragma warning restore SA1407 // Arithmetic expressions should declare precedence
                    int yPos = emoticon.Image.Index / emoticon.TextureSheet.Columns.Value * _imageMaxHeight;

                    if (ExtractStaticImageFile(Path.Combine(extractFilePath, emoticon.Image.FileName), emoticon.TextureSheet.Image, new Point(xPos, yPos), new Size(emoticon.Image.Width, _imageMaxHeight)))
                        count++;
                }

                Console.Write($"\rExtracting emoticon image files...{count}/{_emoticons.Count}");
            }

            Console.WriteLine(" Done.");
        }
    }
}
