namespace HeroesData.Parser
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a new string in which the first occurence of a specified string is relaced with another specified string.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="oldValue">The string to be replaced.</param>
        /// <param name="newValue">The string to replace the first occurence of the oldValue.</param>
        /// <returns></returns>
        public static string ReplaceFirst(this string text, string oldValue, string newValue)
        {
            int pos = text.IndexOf(oldValue);
            if (pos < 0)
            {
                return text;
            }

            return text.Substring(0, pos) + newValue + text.Substring(pos + oldValue.Length);
        }
    }
}
