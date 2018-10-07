using CASCLib;
using System;
using System.IO;

namespace HeroesData.Parser
{
    public static class CASCExtensions
    {
        public static CASCFolder GetDirectory(this CASCFolder cascFolder, string folderPath)
        {
            CASCFolder currentFolder = cascFolder;

            foreach (string directory in EnumeratedStringPath(folderPath))
            {
                currentFolder = (CASCFolder)currentFolder.GetEntry(directory);
            }

            return currentFolder;
        }

        private static string[] EnumeratedStringPath(string filePath)
        {
            return filePath.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
