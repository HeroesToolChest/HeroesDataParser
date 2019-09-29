using CASCLib;
using Heroes.Models;
using HeroesData.ExtractorImage;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.ExtractorImages
{
    public class ImageUnit : ImageExtractorBase<Unit>, IImage
    {
        private readonly HashSet<string> Units = new HashSet<string>();

        private readonly string UnitDirectory = "units";

        public ImageUnit(CASCHandler? cascHandler, string modsFolderPath)
            : base(cascHandler, modsFolderPath)
        {
        }

        protected override void ExtractFiles()
        {
            if (App.ExtractFileOption.HasFlag(ExtractImageOption.Unit))
                ExtractUnitImages();
        }

        protected override void LoadFileData(Unit unit)
        {
            if (!string.IsNullOrEmpty(unit.UnitPortrait.MiniMapIconFileName))
                Units.Add(unit.UnitPortrait.MiniMapIconFileName);
            if (!string.IsNullOrEmpty(unit.UnitPortrait.TargetInfoPanelFileName))
                Units.Add(unit.UnitPortrait.TargetInfoPanelFileName);
        }

        private void ExtractUnitImages()
        {
            if (Units == null || Units.Count < 1)
                return;

            int count = 0;
            Console.Write($"Extracting unit image files...{count}/{Units.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, UnitDirectory);

            foreach (string unit in Units)
            {
                if (ExtractStaticImageFile(Path.Combine(extractFilePath, unit)))
                    count++;

                Console.Write($"\rExtracting unit image files...{count}/{Units.Count}");
            }

            Console.WriteLine(" Done.");
        }
    }
}
