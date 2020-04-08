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
        private readonly HashSet<string> _units = new HashSet<string>();

        private readonly string _unitDirectory = "units";

        public ImageUnit(CASCHandler? cascHandler, string modsFolderPath)
            : base(cascHandler, modsFolderPath)
        {
        }

        protected override void ExtractFiles()
        {
            if (App.ExtractFileOption.HasFlag(ExtractImageOptions.Unit))
                ExtractUnitImages();
        }

        protected override void LoadFileData(Unit unit)
        {
            if (unit is null)
                throw new ArgumentNullException(nameof(unit));

            if (!string.IsNullOrEmpty(unit.UnitPortrait.MiniMapIconFileName))
                _units.Add(unit.UnitPortrait.MiniMapIconFileName);
            if (!string.IsNullOrEmpty(unit.UnitPortrait.TargetInfoPanelFileName))
                _units.Add(unit.UnitPortrait.TargetInfoPanelFileName);
        }

        private void ExtractUnitImages()
        {
            if (_units == null || _units.Count < 1)
                return;

            int count = 0;
            Console.Write($"Extracting unit image files...{count}/{_units.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, _unitDirectory);

            foreach (string unit in _units)
            {
                if (ExtractStaticImageFile(Path.Combine(extractFilePath, unit)))
                    count++;

                Console.Write($"\rExtracting unit image files...{count}/{_units.Count}");
            }

            Console.WriteLine(" Done.");
        }
    }
}
