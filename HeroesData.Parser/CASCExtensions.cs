using CASCLib;
using System;
using System.IO;

namespace HeroesData.Parser
{
    public static class CASCExtensions
    {
        public static CASCFolder GetDirectory(CASCFolder cascFolderData, string folderPath)
        {
            CASCFolder currentFolder = cascFolderData;

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
