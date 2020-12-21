using CASCLib;
using Heroes.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.ExtractorImages
{
    public class ImageBundle : ImageExtractorBase<Bundle>, IImage
    {
        private readonly HashSet<string> _bundles = new HashSet<string>();

        private readonly string _bundleDirectory = "bundles";

        public ImageBundle(CASCHandler? cascHandler, string modsFolderPath)
            : base(cascHandler, modsFolderPath)
        {
        }

        protected override void ExtractFiles()
        {
            if (App.ExtractFileOption.HasFlag(ExtractImageOptions.Bundle))
                ExtractBundleImages();
        }

        protected override void LoadFileData(Bundle data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            if (!string.IsNullOrEmpty(data.ImageFileName))
                _bundles.Add(data.ImageFileName);
        }

        private void ExtractBundleImages()
        {
            if (_bundles == null || _bundles.Count < 1)
                return;

            int count = 0;
            Console.Write($"Extracting bundle image files...{count}/{_bundles.Count}");

            string extractFilePath = Path.Combine(ExtractDirectory, _bundleDirectory);

            foreach (string bundle in _bundles)
            {
                if (ExtractStaticImageFile(Path.Combine(extractFilePath, bundle)))
                    count++;

                Console.Write($"\rExtracting bundle image files...{count}/{_bundles.Count}");
            }

            Console.WriteLine(" Done.");
        }
    }
}
