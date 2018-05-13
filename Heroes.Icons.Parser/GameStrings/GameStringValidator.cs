using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Heroes.Icons.Parser.GameStrings
{
    public class GameStringValidator
    {
        private readonly int SmallSize = 51;
        private readonly int LargeSize = 501;

        private string GameString;

        private int Iterator = 0;
        private Stack<string> TextStack = new Stack<string>(101);

        private GameStringValidator(string gameString)
        {
            GameString = gameString;
        }

        /// <summary>
        /// Takes a game string and removes unmatched and nested tags.
        /// </summary>
        /// <param name="gameString">The game string text.</param>
        /// <returns></returns>
        public static string Validate(string gameString)
        {
            return new GameStringValidator(gameString).Validate();
        }

        /// <summary>
        /// Returns a plain text string without any tags.
        /// </summary>
        /// <param name="gameString">The game string text.</param>
        /// <param name="includeNewLineTags">If true, includes the newline tags.</param>
        /// <param name="includeScaling">If true, includes the scaling info.</param>
        /// <returns></returns>
        public static string GetPlainText(string gameString, bool includeNewLineTags, bool includeScaling)
        {
            return new GameStringValidator(gameString).ParsePlainText(includeNewLineTags, includeScaling);
        }

        /// <summary>
        /// Returns the string with all tags.
        /// </summary>
        /// <param name="gameString">The game string text.</param>
        /// <param name="includeScaling">If true, includes the scaling info</param>
        /// <returns></returns>
        public static string GetColoredText(string gameString, bool includeScaling)
        {
            return new GameStringValidator(gameString).ParseColoredText(includeScaling);
        }

        private string Validate()
        {
            ValideGameString(string.Empty);

            StringBuilder sb = new StringBuilder(LargeSize);

            if (TextStack.Count < 1)
                return string.Empty;

            string endTag = string.Empty;
            string firstItem = TextStack.Peek();

            // remove unmatched start tag
            if (firstItem.StartsWith("<") && firstItem.EndsWith(">") && !firstItem.EndsWith("/>") && !firstItem.StartsWith("</"))
                TextStack.Pop();

            // remove empty elements
            while (TextStack.Count > 0)
            {
                string item = TextStack.Pop();

                if (item.StartsWith("</") && item.EndsWith(">") && !item.EndsWith("/>")) // end tag
                {
                    endTag = item;
                    continue;
                }
                else if (item.StartsWith("<") && item.EndsWith(">") && !item.EndsWith("/>")) // check if start tag
                {
                    if (string.IsNullOrEmpty(endTag))
                    {
                        sb.Insert(0, item);
                        continue;
                    }
                    else // dont save, empty tag
                    {
                        endTag = string.Empty;
                        continue;
                    }
                }
                else if (!string.IsNullOrEmpty(endTag))
                {
                    sb.Insert(0, endTag);
                    endTag = string.Empty;
                }

                sb.Insert(0, item);
            }

            return sb.ToString();
        }

        // modifies game string, remove unmatched tags, nested tags
        private void ValideGameString(string startTag)
        {
            StringBuilder sb = new StringBuilder(LargeSize);
            for (; Iterator < GameString.Length; Iterator++)
            {
                if (GameString[Iterator] == '<' && GameString[Iterator + 1] != ' ')
                {
                    if (sb.Length > 0)
                    {
                        TextStack.Push(sb.ToString());
                        sb = new StringBuilder(LargeSize);
                    }

                    if (TryParseTag(out string tag, out bool isStartTag))
                    {
                        if (isStartTag)
                        {
                            Iterator++;

                            // nested
                            if (!string.IsNullOrEmpty(startTag))
                                TextStack.Push(CreateEndTag(startTag));

                            TextStack.Push(tag);
                            ValideGameString(tag);

                            if (!string.IsNullOrEmpty(startTag))
                                TextStack.Push(startTag);

                            continue;
                        }
                        else if (tag == "<n/>" || tag == "</n>") // line breakers
                        {
                            tag = "<n/>";

                            // nested
                            if (!string.IsNullOrEmpty(startTag))
                            {
                                TextStack.Push(CreateEndTag(startTag));
                                TextStack.Push(tag);
                                TextStack.Push(startTag);
                            }
                            else
                            {
                                TextStack.Push(tag);
                            }

                            continue;
                        }
                        else // end tag
                        {
                            if (MatchElement(startTag, tag))
                            {
                                if (TextStack.Peek() != startTag)
                                    TextStack.Push(tag);
                                else
                                    TextStack.Pop();
                                return;
                            }
                            else if (tag.Length > 4 && tag.EndsWith("/>")) // self close tag
                            {
                                TextStack.Push(tag);
                                continue;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }

                if (Iterator >= GameString.Length)
                    break;

                sb.Append(GameString[Iterator]);
            }

            if (sb.Length > 0)
            {
                if (TextStack.Count > 0 && TextStack.Peek() == startTag)
                    TextStack.Pop();

                TextStack.Push(sb.ToString());
            }
        }

        // removes all tags
        private string ParsePlainText(bool includeNewlineTags, bool includeScaling)
        {
            StringBuilder sb = new StringBuilder(LargeSize);
            for (; Iterator < GameString.Length; Iterator++)
            {
                if (GameString[Iterator] == '<')
                {
                    if (sb.Length > 0)
                    {
                        TextStack.Push(sb.ToString());
                        sb = new StringBuilder(LargeSize);
                    }

                    if (TryParseTag(out string tag, out bool isStartTag))
                    {
                        if (tag == "<n/>")
                        {
                            if (includeNewlineTags)
                                TextStack.Push(tag);
                            else
                                TextStack.Push(" ");
                        }

                        continue;
                    }
                }
                else if (GameString[Iterator] == '~' && Iterator + 1 < GameString.Length && GameString[Iterator + 1] == '~')
                {
                    TextStack.Push(sb.ToString());
                    sb = new StringBuilder(LargeSize);

                    if (ParseScalingTag(out string scaleText, includeScaling))
                    {
                        TextStack.Push(scaleText);
                    }

                    continue;
                }

                if (Iterator >= GameString.Length)
                    break;

                sb.Append(GameString[Iterator]);
            }

            if (sb.Length > 0)
                TextStack.Push(sb.ToString());

            sb = new StringBuilder(LargeSize);

            while (TextStack.Count > 0)
            {
                sb.Insert(0, TextStack.Pop());
            }

            return sb.ToString().Trim();
        }

        // keeps colored tags and new lines
        private string ParseColoredText(bool includeScaling)
        {
            StringBuilder sb = new StringBuilder(SmallSize);
            for (; Iterator < GameString.Length; Iterator++)
            {
                if (GameString[Iterator] == '<')
                {
                    if (sb.Length > 0)
                    {
                        TextStack.Push(sb.ToString());
                        sb = new StringBuilder(SmallSize);
                    }

                    if (TryParseTag(out string tag, out bool isStartTag))
                    {
                        TextStack.Push(tag);

                        continue;
                    }
                }
                else if (GameString[Iterator] == '~' && Iterator + 1 < GameString.Length && GameString[Iterator + 1] == '~')
                {
                    TextStack.Push(sb.ToString());
                    sb = new StringBuilder(SmallSize);

                    if (ParseScalingTag(out string scaleText, includeScaling))
                    {
                        TextStack.Push(scaleText);
                    }

                    continue;
                }

                if (Iterator >= GameString.Length)
                    break;

                sb.Append(GameString[Iterator]);
            }

            if (sb.Length > 0)
                TextStack.Push(sb.ToString());

            sb = new StringBuilder(SmallSize);

            while (TextStack.Count > 0)
            {
                sb.Insert(0, TextStack.Pop());
            }

            return sb.ToString().Trim();
        }

        // checks if the end tag matches the start tag
        private bool MatchElement(string startTag, string endTag)
        {
            if (string.IsNullOrEmpty(startTag))
                return false;

            string end = endTag.TrimEnd('>').TrimStart('<').TrimStart('/');
            string[] parts = startTag.Split(new char[] { ' ' }, 2);

            if (parts[0].TrimStart('<') == end)
                return true;
            else
                return false;
        }

        // get whole tag, determine if it's a start tag
        private bool TryParseTag(out string tag, out bool isStartTag)
        {
            StringBuilder sb = new StringBuilder(SmallSize);
            for (; Iterator < GameString.Length; Iterator++)
            {
                sb.Append(GameString[Iterator]);
                if (GameString[Iterator] == '>')
                {
                    string[] parts = sb.ToString().Split(new char[] { ' ' }, 2);
                    if (parts.Length > 1)
                        tag = $"{parts[0].ToLower()} {parts[1]}";
                    else if (parts.Length == 1)
                        tag = parts[0].ToLower();
                    else
                        tag = sb.ToString();

                    // check if its a start tag
                    if (tag[1] != '/' && tag[tag.Length - 2] != '/')
                        isStartTag = true;
                    else
                        isStartTag = false;

                    tag = Regex.Replace(tag, @"\s+", " ");
                    return true;
                }
            }

            isStartTag = false;
            tag = sb.ToString().ToLower();
            tag = Regex.Replace(tag, @"\s+", " ");

            return false;
        }

        private string CreateEndTag(string startTag)
        {
            string start = startTag.TrimStart('<');
            string[] parts = start.Split(new char[] { ' ' }, 2);

            if (parts.Length > 1)
                return "</" + parts[0].ToLower() + ">";
            else
                return "</" + parts[0].ToLower();
        }

        /// <summary>
        /// Parse the scaling tag, removes the tag or replaces it
        /// </summary>
        /// <param name="scaleText"></param>
        /// <param name="replace">If true, replace the tag, else return an empty string</param>
        private bool ParseScalingTag(out string scaleText, bool replace)
        {
            int tagCount = 0;
            StringBuilder sb = new StringBuilder(SmallSize);

            for (; Iterator < GameString.Length; Iterator++)
            {
                if (GameString[Iterator] == '~')
                    tagCount++;
                else
                    sb.Append(GameString[Iterator]);

                if (tagCount == 4)
                {
                    if (replace)
                    {
                        scaleText = $" (+{double.Parse(sb.ToString()) * 100}% per level)";
                        return true;
                    }
                    else
                    {
                        scaleText = string.Empty;
                        return true;
                    }
                }
            }

            scaleText = string.Empty;
            return false;
        }
    }
}
