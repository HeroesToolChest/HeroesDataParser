using CASCLib;
using Heroes.Models;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.ExtractorImage
{
    public class ImageMatchAward : ImageExtractorBase<MatchAward>, IImage
    {
        private readonly HashSet<(string OriginalName, string NewName)> Awards = new HashSet<(string Original, string NewName)>();

        private readonly string MatchAwardsDirectory = "matchAwards";

        public ImageMatchAward(CASCHandler cascHandler, StorageMode storageMode)
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
            if (App.ExtractFileOption.HasFlag(ExtractImageOption.MatchAward))
                ExtractMatchAwardIcons();
        }

        private void ExtractMatchAwardIcons()
        {
            if (Awards == null || Awards.Count < 1)
                return;

            int count = 0;
            Console.Write($"Extracting match award icon files...{count}/{Awards.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, MatchAwardsDirectory);

            foreach ((string originalName, string newName) in Awards)
            {
                if (originalName.StartsWith("storm_ui_mvp_icons_rewards_") || originalName == "storm_ui_mvp_icon.dds")
                {
                    if (ExtractMVPAwardFile(extractFilePath, originalName, newName))
                        count++;

                    Console.Write($"\rExtracting match award icon files...{count}/{Awards.Count}");
                }
                else
                {
                    if (ExtractScoreAwardFile(extractFilePath, originalName, newName, "red") && ExtractScoreAwardFile(extractFilePath, originalName, newName, "blue"))
                        count++;

                    Console.Write($"\rExtracting match award icon files...{count}/{Awards.Count}");
                }
            }

            Console.WriteLine(" Done.");
        }

        /// <summary>
        /// Extracts a score screen match award file.
        /// </summary>
        /// <param name="path">The path to extract the file to.</param>
        /// <param name="fileName">The name of the file to extract.</param>
        /// <param name="newFileName">The new file name of the award.</param>
        /// <param name="color">The color of the award.</param>
        private bool ExtractScoreAwardFile(string path, string fileName, string newFileName, string color)
        {
            try
            {
                Directory.CreateDirectory(path);

                fileName = fileName.Replace("%team%", color);
                string cascFilepath = Path.Combine(CASCTexturesPath, fileName);
                if (CASCHandler.FileExists(cascFilepath))
                {
                    DDSImage image = new DDSImage(CASCHandler.OpenFile(cascFilepath));

                    image.Save(Path.Combine(path, $"{Path.GetFileNameWithoutExtension(newFileName.Replace("%team%", color))}.png"));

                    return true;
                }
                else
                {
                    FailedFileMessages.Add($"CASC file not found: {fileName}");
                }
            }
            catch (Exception ex)
            {
                FailedFileMessages.Add($"Error extracting file: {fileName}");
                FailedFileMessages.Add($"--> {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Extracts a MVP match award file.
        /// </summary>
        /// <param name="path">The path to extract the file to.</param>
        /// <param name="fileName">The name of the file to extract.</param>
        /// <param name="newFileName">The new file name of the award.</param>
        private bool ExtractMVPAwardFile(string path, string fileName, string newFileName)
        {
            try
            {
                Directory.CreateDirectory(path);

                string cascFilepath = Path.Combine(CASCTexturesPath, fileName);
                if (CASCHandler.FileExists(cascFilepath))
                {
                    DDSImage image = new DDSImage(CASCHandler.OpenFile(cascFilepath));

                    int newWidth = image.Width / 3;

                    image.Save(Path.Combine(path, $"{Path.GetFileNameWithoutExtension(newFileName.Replace("%color%", "blue"))}.png"), new Point(0, 0), new Size(newWidth, image.Height));
                    image.Save(Path.Combine(path, $"{Path.GetFileNameWithoutExtension(newFileName.Replace("%color%", "red"))}.png"), new Point(newWidth, 0), new Size(newWidth, image.Height));
                    image.Save(Path.Combine(path, $"{Path.GetFileNameWithoutExtension(newFileName.Replace("%color%", "gold"))}.png"), new Point(newWidth * 2, 0), new Size(newWidth, image.Height));

                    return true;
                }
                else
                {
                    FailedFileMessages.Add($"CASC file not found: {fileName}");
                }
            }
            catch (Exception ex)
            {
                FailedFileMessages.Add($"Error extracting file: {fileName}");
                FailedFileMessages.Add($"--> {ex.Message}");
            }

            return false;
        }
    }
}
