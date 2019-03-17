using CASCLib;
using Heroes.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.ExtractorFiles
{
    public class FilesVoiceLine : FilesExtractorBase<VoiceLine>, IFile
    {
        private readonly SortedSet<string> VoiceLines = new SortedSet<string>();

        private readonly string VoiceDirectory = "voicelines";

        public FilesVoiceLine(CASCHandler cascHandler, StorageMode storageMode)
            : base(cascHandler, storageMode)
        {
        }

        protected override void ExtractFiles()
        {
            if (App.ExtractFileOption.HasFlag(ExtractFileOption.VoiceLines))
                ExtractVoiceLineImages();
        }

        protected override void LoadFileData(VoiceLine voiceLine)
        {
            if (!string.IsNullOrEmpty(voiceLine.ImageFileName))
                VoiceLines.Add(voiceLine.ImageFileName.ToLower());
        }

        private void ExtractVoiceLineImages()
        {
            if (VoiceLines == null || VoiceLines.Count < 1)
                return;

            int count = 0;
            Console.Write($"Extracting voiceline image files...{count}/{VoiceLines.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, VoiceDirectory);

            foreach (string voiceline in VoiceLines)
            {
                if (ExtractImageFile(extractFilePath, voiceline))
                    count++;

                Console.Write($"\rExtracting voiceline image files...{count}/{VoiceLines.Count}");
            }

            Console.WriteLine(" Done.");
        }
    }
}
