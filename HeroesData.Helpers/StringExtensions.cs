using System;

namespace HeroesData.Helpers
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a new string in which the first occurence of a specified string is replaced with another specified string.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="oldValue">The string to be replaced.</param>
        /// <param name="newValue">The string to replace the first occurence of the oldValue.</param>
        /// <returns></returns>
        public static string ReplaceFirst(this string text, string oldValue, string newValue)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));
            if (oldValue is null)
                throw new ArgumentNullException(nameof(oldValue));

            int pos = text.IndexOf(oldValue, StringComparison.Ordinal);
            if (pos < 0)
            {
                return text;
            }

            return string.Concat(text.AsSpan(0, pos), newValue, text.AsSpan(pos + oldValue.Length));
        }
    }
}
