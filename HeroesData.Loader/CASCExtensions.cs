using CASCLib;
using System;
using System.IO;

namespace HeroesData.Loader
{
    public static class CASCExtensions
    {
        public static CASCFolder GetDirectory(this CASCFolder cascFolder, string folderPath)
        {
            if (cascFolder is null)
                throw new ArgumentNullException(nameof(cascFolder));
            if (folderPath is null)
                throw new ArgumentNullException(nameof(folderPath));

            CASCFolder currentFolder = cascFolder;

            foreach (string directory in EnumeratedStringPath(folderPath))
            {
#pragma warning disable CA1062 // Validate arguments of public methods
                currentFolder = (CASCFolder)currentFolder.GetEntry(directory);
#pragma warning restore CA1062 // Validate arguments of public methods
            }

            return currentFolder;
        }

        public static bool DirectoryExists(this CASCFolder cascFolder, string folderPath)
        {
            if (cascFolder is null)
                throw new ArgumentNullException(nameof(cascFolder));
            if (folderPath is null)
                throw new ArgumentNullException(nameof(folderPath));

            CASCFolder currentFolder = cascFolder;

            foreach (string directory in EnumeratedStringPath(folderPath))
            {
                if ((CASCFolder)currentFolder.GetEntry(directory) == null)
                    return false;
            }

            return true;
        }

        private static string[] EnumeratedStringPath(string filePath)
        {
            return filePath.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
