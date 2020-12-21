using CASCLib;
using Heroes.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.ExtractorImages
{
    public class ImageVoiceLine : ImageExtractorBase<VoiceLine>, IImage
    {
        private readonly HashSet<string> _voiceLines = new HashSet<string>();

        private readonly string _voiceDirectory = "voicelines";

        public ImageVoiceLine(CASCHandler? cascHandler, string modsFolderPath)
            : base(cascHandler, modsFolderPath)
        {
        }

        protected override void ExtractFiles()
        {
            if (App.ExtractFileOption.HasFlag(ExtractImageOptions.VoiceLine))
                ExtractVoiceLineImages();
        }

        protected override void LoadFileData(VoiceLine data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            if (!string.IsNullOrEmpty(data.ImageFileName))
                _voiceLines.Add(data.ImageFileName);
        }

        private void ExtractVoiceLineImages()
        {
            if (_voiceLines == null || _voiceLines.Count < 1)
                return;

            int count = 0;
            Console.Write($"Extracting voiceline image files...{count}/{_voiceLines.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, _voiceDirectory);

            foreach (string voiceline in _voiceLines)
            {
                if (ExtractStaticImageFile(Path.Combine(extractFilePath, voiceline)))
                    count++;

                Console.Write($"\rExtracting voiceline image files...{count}/{_voiceLines.Count}");
            }

            Console.WriteLine(" Done.");
        }
    }
}
