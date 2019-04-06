using CASCLib;
using Heroes.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.ExtractorImage
{
    public class ImageAnnouncer : ImageExtractorBase<Announcer>, IImage
    {
        private readonly HashSet<string> Announcers = new HashSet<string>();

        private readonly string AnnouncerDirectory = "announcers";

        public ImageAnnouncer(CASCHandler cascHandler, StorageMode storageMode)
            : base(cascHandler, storageMode)
        {
        }

        protected override void ExtractFiles()
        {
            if (App.ExtractFileOption.HasFlag(ExtractImageOption.Announcer))
                ExtractAnnouncerImages();
        }

        protected override void LoadFileData(Announcer announcer)
        {
            if (!string.IsNullOrEmpty(announcer.ImageFileName))
                Announcers.Add(announcer.ImageFileName.ToLower());
        }

        private void ExtractAnnouncerImages()
        {
            if (Announcers == null || Announcers.Count < 1)
                return;

            int count = 0;
            Console.Write($"Extracting announcer image files...{count}/{Announcers.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, AnnouncerDirectory);

            foreach (string announcer in Announcers)
            {
                if (ExtractStaticImageFile(Path.Combine(extractFilePath, announcer)))
                    count++;

                Console.Write($"\rExtracting announcer image files...{count}/{Announcers.Count}");
            }

            Console.WriteLine(" Done.");
        }
    }
}
