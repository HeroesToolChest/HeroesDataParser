using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace HeroesData.Helpers
{
    public static class PathHelper
    {
        /// <summary>
        /// Modifies a string file path to use the current platform's directory separator character.
        /// </summary>
        /// <param name="filePath">A file path.</param>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(filePath))]
        public static string? GetFilePath(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return filePath;
            }

            bool platformIsBackslash = Path.DirectorySeparatorChar == '\\';

            StringBuilder sb = new();
            for (var i = 0; i < filePath.Length; i++)
            {
                if (platformIsBackslash && filePath[i] == '/')
                    sb.Append(Path.DirectorySeparatorChar);
                else if (!platformIsBackslash && filePath[i] == '\\')
                    sb.Append(Path.DirectorySeparatorChar);
                else
                    sb.Append(filePath[i]);
            }

            return sb.ToString();
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
