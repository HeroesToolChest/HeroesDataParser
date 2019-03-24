using CASCLib;
using Heroes.Models;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.ExtractorFiles
{
    public class FilesEmoticon : FilesExtractorBase<Emoticon>, IFile
    {
        private readonly int ImageMaxHeight = 32;
        private readonly int ImageMaxWidth = 40;
        private readonly HashSet<Emoticon> Emoticons = new HashSet<Emoticon>();

        private readonly string EmoticonDirectory = "emoticons";

        public FilesEmoticon(CASCHandler cascHandler, StorageMode storageMode)
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
                if (ExtractIndividualEmoticonImage(extractFilePath, emoticon.TextureSheet.Image.ToLower(), emoticon))
                    count++;

                Console.Write($"\rExtracting emoticon image files...{count}/{Emoticons.Count}");
            }

            Console.WriteLine(" Done.");
        }

        private bool ExtractIndividualEmoticonImage(string path, string textureSheetFileName, Emoticon emoticon)
        {
            try
            {
                Directory.CreateDirectory(path);

                string cascFilepath = Path.Combine(CASCTexturesPath, textureSheetFileName);
                if (CASCHandler.FileExists(cascFilepath))
                {
                    DDSImage image = new DDSImage(CASCHandler.OpenFile(cascFilepath));

                    int xPos = (emoticon.Image.Index % emoticon.TextureSheet.Columns) * ImageMaxWidth;
                    int yPos = (emoticon.Image.Index / emoticon.TextureSheet.Columns) * ImageMaxHeight;

                    image.Save(Path.Combine(path, $"{Path.GetFileNameWithoutExtension(emoticon.Image.FileName)}.png"), new Point(xPos, yPos), new Size(emoticon.Image.Width, ImageMaxHeight));

                    return true;
                }
                else
                {
                    FailedFileMessages.Add($"CASC file not found: {textureSheetFileName}");
                }
            }
            catch (Exception ex)
            {
                FailedFileMessages.Add($"Error extracting file: {textureSheetFileName}");
                FailedFileMessages.Add($"--> {ex.Message}");
            }

            return false;
        }
    }
}
