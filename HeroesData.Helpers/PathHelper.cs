using System;
using System.IO;
using System.Runtime.InteropServices;

namespace HeroesData.Helpers
{
    public static class PathHelper
    {
        /// <summary>
        /// Modifies a string file path to use the current platform's directory separator character.
        /// </summary>
        /// <param name="filePath">A file path.</param>
        /// <returns></returns>
        public static string GetFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Argument cannot be null or empty.", nameof(filePath));
            }

            if (Path.DirectorySeparatorChar != '\\')
            {
                filePath = filePath.Replace('\\', Path.DirectorySeparatorChar);
            }

            return filePath;
        }

        /// <summary>
        /// Modifies a file path by lowercasing the file name.
        /// </summary>
        /// <param name="filePath">A file path.</param>
        public static void FileNameToLower(ReadOnlyMemory<char> filePath)
        {
            if (filePath.IsEmpty)
                return;

            Memory<char> filePathMemory = MemoryMarshal.AsMemory(filePath);
            int current = filePath.Span.LastIndexOf(Path.DirectorySeparatorChar) + 1;

            if (current < 1)
                return;

            while (current < filePath.Length)
            {
                ref char start = ref filePathMemory.Span[current];

                if (start >= 'A' && start <= 'Z')
                {
                    start = (char)(start + 32);
                }

                current++;
            }
        }
    }
}
