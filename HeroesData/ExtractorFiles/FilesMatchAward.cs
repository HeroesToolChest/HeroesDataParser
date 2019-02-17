using CASCLib;
using DDSReader;
using Heroes.Models;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.ExtractorFiles
{
    public class FilesMatchAward : FilesExtractorBase<MatchAward>, IFile
    {
        private readonly string MatchAwardsDirectory = "matchAwards";

        private SortedSet<(string OriginalName, string NewName)> Awards = new SortedSet<(string Original, string NewName)>();

        public FilesMatchAward(CASCHandler cascHandler, StorageMode storageMode)
            : base(cascHandler, storageMode)
        {
        }

        protected override void LoadFileData(MatchAward matchAward)
        {
            if (!string.IsNullOrEmpty(matchAward.MVPScreenImageFileNameOriginal))
                Awards.Add((matchAward.MVPScreenImageFileNameOriginal.ToLower(), matchAward.MVPScreenImageFileName.ToLower()));
            if (!string.IsNullOrEmpty(matchAward.ScoreScreenImageFileNameOriginal))
                Awards.Add((matchAward.ScoreScreenImageFileNameOriginal.ToLower(), matchAward.ScoreScreenImageFileName.ToLower()));
        }

        protected override void ExtractFiles()
        {
            if (App.ExtractFileOption.HasFlag(ExtractFileOption.MatchAwards))
                ExtractMatchAwardIcons();
        }

        private void ExtractMatchAwardIcons()
        {
            if (Awards == null || Awards.Count < 1)
                return;

            Console.Write("Extracting match award icon files...");

            string extractFilePath = Path.Combine(ExtractDirectory, MatchAwardsDirectory);

            foreach ((string originalName, string newName) in Awards)
            {
                if (originalName.StartsWith("storm_ui_mvp_icons_rewards_") || originalName == "storm_ui_mvp_icon.dds")
                {
                    ExtractMVPAwardFile(extractFilePath, originalName, newName);
                }
                else
                {
                    ExtractScoreAwardFile(extractFilePath, originalName, newName, "red");
                    ExtractScoreAwardFile(extractFilePath, originalName, newName, "blue");
                }
            }

            Console.WriteLine("Done.");
        }

        /// <summary>
        /// Extracts a score screen match award file.
        /// </summary>
        /// <param name="path">The path to extract the file to.</param>
        /// <param name="fileName">The name of the file to extract.</param>
        /// <param name="newFileName">The new file name of the award.</param>
        /// <param name="color">The color of the award.</param>
        private void ExtractScoreAwardFile(string path, string fileName, string newFileName, string color)
        {
            Directory.CreateDirectory(path);

            try
            {
                fileName = fileName.Replace("%team%", color);
                string cascFilepath = Path.Combine(CASCTexturesPath, fileName);
                if (CASCHandler.FileExists(cascFilepath))
                {
                    DDSImage image = new DDSImage(CASCHandler.OpenFile(cascFilepath));

                    image.Save(Path.Combine(path, $"{Path.GetFileNameWithoutExtension(newFileName.Replace("%team%", color))}.png"));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"CASC file not found: {fileName}");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine($"Error extracting file: {fileName}");
                Console.WriteLine($"--> {ex.Message}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Extracts a MVP match award file.
        /// </summary>
        /// <param name="path">The path to extract the file to.</param>
        /// <param name="fileName">The name of the file to extract.</param>
        /// <param name="newFileName">The new file name of the award.</param>
        private void ExtractMVPAwardFile(string path, string fileName, string newFileName)
        {
            Directory.CreateDirectory(path);

            try
            {
                string cascFilepath = Path.Combine(CASCTexturesPath, fileName);
                if (CASCHandler.FileExists(cascFilepath))
                {
                    DDSImage image = new DDSImage(CASCHandler.OpenFile(cascFilepath));

                    int newWidth = image.Width / 3;

                    image.Save(Path.Combine(path, $"{Path.GetFileNameWithoutExtension(newFileName.Replace("%color%", "blue"))}.png"), new Point(0, 0), new Size(newWidth, image.Height));
                    image.Save(Path.Combine(path, $"{Path.GetFileNameWithoutExtension(newFileName.Replace("%color%", "red"))}.png"), new Point(newWidth, 0), new Size(newWidth, image.Height));
                    image.Save(Path.Combine(path, $"{Path.GetFileNameWithoutExtension(newFileName.Replace("%color%", "gold"))}.png"), new Point(newWidth * 2, 0), new Size(newWidth, image.Height));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"CASC file not found: {fileName}");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine($"Error extracting file: {fileName}");
                Console.WriteLine($"--> {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
