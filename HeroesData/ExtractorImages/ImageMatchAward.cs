using CASCLib;
using Heroes.Models;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.ExtractorImage
{
    public class ImageMatchAward : ImageExtractorBase<MatchAward>, IImage
    {
        private readonly HashSet<(string OriginalName, string NewName)> _awards = new HashSet<(string Original, string NewName)>();

        private readonly string _matchAwardsDirectory = "matchawards";

        public ImageMatchAward(CASCHandler? cascHandler, string modsFolderPath)
            : base(cascHandler, modsFolderPath)
        {
        }

        protected override void LoadFileData(MatchAward matchAward)
        {
            if (matchAward is null)
                throw new ArgumentNullException(nameof(matchAward));

            if (!string.IsNullOrEmpty(matchAward.MVPScreenImageFileNameOriginal))
                _awards.Add((matchAward.MVPScreenImageFileNameOriginal, matchAward.MVPScreenImageFileName));
            if (!string.IsNullOrEmpty(matchAward.ScoreScreenImageFileNameOriginal))
                _awards.Add((matchAward.ScoreScreenImageFileNameOriginal, matchAward.ScoreScreenImageFileName));
        }

        protected override void ExtractFiles()
        {
            if (App.ExtractFileOption.HasFlag(ExtractImageOptions.MatchAward))
                ExtractMatchAwardIcons();
        }

        private void ExtractMatchAwardIcons()
        {
            if (_awards == null || _awards.Count < 1)
                return;

            int count = 0;
            Console.Write($"Extracting match award icon files...{count}/{_awards.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, _matchAwardsDirectory);

            foreach ((string originalName, string newName) in _awards)
            {
                if (originalName.StartsWith("storm_ui_mvp_icons_rewards_", StringComparison.OrdinalIgnoreCase) || originalName == "storm_ui_mvp_icon.dds")
                {
                    if (ExtractMVPAwardFile(extractFilePath, originalName, newName))
                        count++;

                    Console.Write($"\rExtracting match award icon files...{count}/{_awards.Count}");
                }
                else
                {
                    if (ExtractScoreAwardFile(extractFilePath, originalName, newName, "red") && ExtractScoreAwardFile(extractFilePath, originalName, newName, "blue"))
                        count++;

                    Console.Write($"\rExtracting match award icon files...{count}/{_awards.Count}");
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

                fileName = fileName.Replace("%team%", color, StringComparison.OrdinalIgnoreCase);
                string textureFilepath = Path.Combine(TexturesPath, fileName);
                if (FileExists(textureFilepath))
                {
                    using Stream stream = OpenFile(textureFilepath);
                    using DDSImage image = new DDSImage(stream);

                    image.Save(Path.Combine(path, $"{Path.GetFileNameWithoutExtension(newFileName.Replace("%team%", color, StringComparison.OrdinalIgnoreCase))}.png"));

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

                string textureFilepath = Path.Combine(TexturesPath, fileName);
                if (FileExists(textureFilepath))
                {
                    using Stream stream = OpenFile(textureFilepath);
                    using DDSImage image = new DDSImage(stream);

                    int newWidth = image.Width / 3;

                    image.Save(Path.Combine(path, $"{Path.GetFileNameWithoutExtension(newFileName.Replace("%color%", "blue", StringComparison.OrdinalIgnoreCase))}.png"), new Point(0, 0), new Size(newWidth, image.Height));
                    image.Save(Path.Combine(path, $"{Path.GetFileNameWithoutExtension(newFileName.Replace("%color%", "red", StringComparison.OrdinalIgnoreCase))}.png"), new Point(newWidth, 0), new Size(newWidth, image.Height));
                    image.Save(Path.Combine(path, $"{Path.GetFileNameWithoutExtension(newFileName.Replace("%color%", "gold", StringComparison.OrdinalIgnoreCase))}.png"), new Point(newWidth * 2, 0), new Size(newWidth, image.Height));

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
