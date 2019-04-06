using CASCLib;
using Heroes.Models;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.ExtractorImage
{
    public class ImageEmoticon : ImageExtractorBase<Emoticon>, IImage
    {
        private readonly int ImageMaxHeight = 32;
        private readonly int ImageMaxWidth = 40;
        private readonly HashSet<Emoticon> Emoticons = new HashSet<Emoticon>();
        private readonly string EmoticonDirectory = "emoticons";

        public ImageEmoticon(CASCHandler cascHandler, StorageMode storageMode)
            : base(cascHandler, storageMode)
        {
        }

        protected override void ExtractFiles()
        {
            if (App.ExtractFileOption.HasFlag(ExtractImageOption.Emoticon))
                ExtractEmoticonImages();
        }

        protected override void LoadFileData(Emoticon emoticon)
        {
            if (!string.IsNullOrEmpty(emoticon.TextureSheet.Image))
                Emoticons.Add(emoticon);
        }

        private void ExtractEmoticonImages()
        {
            if (Emoticons == null || Emoticons.Count < 1)
                return;

            int count = 0;
            Console.Write($"Extracting emoticon image files...{count}/{Emoticons.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, EmoticonDirectory);

            foreach (Emoticon emoticon in Emoticons)
            {
                if (emoticon.Image.Count.HasValue)
                {
                    if (ExtractAnimatedImageFile(Path.Combine(extractFilePath, emoticon.TextureSheet.Image.ToLower()), new Size(emoticon.Image.Width, ImageMaxHeight), new Size(ImageMaxWidth, ImageMaxHeight), emoticon.Image.Count.Value, emoticon.Image.DurationPerFrame.Value) &&
                        ExtractStaticImageFile(Path.Combine(extractFilePath, emoticon.TextureSheet.Image.ToLower())))
                    {
                        count++;
                    }
                }
                else
                {
                    int xPos = (emoticon.Image.Index % emoticon.TextureSheet.Columns) * ImageMaxWidth;
                    int yPos = (emoticon.Image.Index / emoticon.TextureSheet.Columns) * ImageMaxHeight;

                    if (ExtractStaticImageFile(Path.Combine(extractFilePath, emoticon.Image.FileName.ToLower()), emoticon.TextureSheet.Image.ToLower(), new Point(xPos, yPos), new Size(emoticon.Image.Width, ImageMaxHeight)))
                        count++;
                }

                Console.Write($"\rExtracting emoticon image files...{count}/{Emoticons.Count}");
            }

            Console.WriteLine(" Done.");
        }
    }
}
