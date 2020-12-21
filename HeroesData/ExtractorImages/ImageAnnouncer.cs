using CASCLib;
using Heroes.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.ExtractorImages
{
    public class ImageAnnouncer : ImageExtractorBase<Announcer>, IImage
    {
        private readonly HashSet<string> _announcers = new HashSet<string>();

        private readonly string _announcerDirectory = "announcers";

        public ImageAnnouncer(CASCHandler? cascHandler, string modsFolderPath)
            : base(cascHandler, modsFolderPath)
        {
        }

        protected override void ExtractFiles()
        {
            if (App.ExtractFileOption.HasFlag(ExtractImageOptions.Announcer))
                ExtractAnnouncerImages();
        }

        protected override void LoadFileData(Announcer data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            if (!string.IsNullOrEmpty(data.ImageFileName))
                _announcers.Add(data.ImageFileName);
        }

        private void ExtractAnnouncerImages()
        {
            if (_announcers == null || _announcers.Count < 1)
                return;

            int count = 0;
            Console.Write($"Extracting announcer image files...{count}/{_announcers.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, _announcerDirectory);

            foreach (string announcer in _announcers)
            {
                if (ExtractStaticImageFile(Path.Combine(extractFilePath, announcer)))
                    count++;

                Console.Write($"\rExtracting announcer image files...{count}/{_announcers.Count}");
            }

            Console.WriteLine(" Done.");
        }
    }
}
