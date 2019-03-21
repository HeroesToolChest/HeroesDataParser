using System.IO;

namespace HeroesData.Helpers
{
    public static class PathHelpers
    {
        /// <summary>
        /// Modifies a string file path to use the current platform's directory separator character.
        /// </summary>
        /// <param name="filePath">A file path.</param>
        /// <returns></returns>
        public static string GetFilePath(string filePath)
        {
            if (Path.DirectorySeparatorChar != '\\')
            {
                filePath = filePath.Replace('\\', Path.DirectorySeparatorChar);
            }

            return filePath;
        }
    }
}
