using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Heroes.Icons.Parser.Descriptions
{
    public class DescriptionValidation
    {
        private string DescriptionString;

        private int Iterator = 0;
        private Stack<string> ElementStack = new Stack<string>();

        private DescriptionValidation(string descriptionString)
        {
            DescriptionString = descriptionString;
        }

        /// <summary>
        /// Takes a description string and removes unmatched and nested tags.
        /// </summary>
        /// <param name="descriptionString">The description string.</param>
        /// <returns></returns>
        public static string Validate(string descriptionString)
        {
            return new DescriptionValidation(descriptionString).ParseValidate();
        }

        /// <summary>
        /// Returns a plain text string without any tags.
        /// </summary>
        /// <param name="descriptionString">The description string.</param>
        /// <param name="includeNewLineTags">If true, includes the newline tags.</param>
        /// <param name="includeScaling">If true, includes the scaling info.</param>
        /// <returns></returns>
        public static string GetPlainText(string descriptionString, bool includeNewLineTags, bool includeScaling)
        {
            return new DescriptionValidation(descriptionString).ParsePlainText(includeNewLineTags, includeScaling);
        }

        /// <summary>
        /// Returns the description with all tags.
        /// </summary>
        /// <param name="descriptionString">The description string.</param>
        /// <param name="includeScaling">If true, includes the scaling info</param>
        /// <returns></returns>
        public static string GetColoredText(string descriptionString, bool includeScaling)
        {
            return new DescriptionValidation(descriptionString).ParseColoredText(includeScaling);
        }

        private string ParseValidate()
        {
            ParseValidateDescription(string.Empty);

            StringBuilder sb = new StringBuilder();

            if (ElementStack.Count < 1)
                return string.Empty;

            string endTag = string.Empty;
            string firstItem = ElementStack.Peek();

            // remove unmatched start tag
            if (firstItem.StartsWith("<") && firstItem.EndsWith(">") && !firstItem.EndsWith("/>") && !firstItem.StartsWith("</"))
                ElementStack.Pop();

            // remove empty elements
            while (ElementStack.Count > 0)
            {
                string item = ElementStack.Pop();

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

        // modifies description, remove unmatched tags, nested tags
        private void ParseValidateDescription(string startTag)
        {
            StringBuilder sb = new StringBuilder();
            for (; Iterator < DescriptionString.Length; Iterator++)
            {
                if (DescriptionString[Iterator] == '<' && DescriptionString[Iterator + 1] != ' ')
                {
                    if (sb.Length > 0)
                    {
                        ElementStack.Push(sb.ToString());
                        sb = new StringBuilder();
                    }

                    if (TryParseTag(out string tag, out bool isStartTag))
                    {
                        if (isStartTag)
                        {
                            Iterator++;

                            // nested
                            if (!string.IsNullOrEmpty(startTag))
                                ElementStack.Push(CreateEndTag(startTag));

                            ElementStack.Push(tag);
                            ParseValidateDescription(tag);

                            if (!string.IsNullOrEmpty(startTag))
                                ElementStack.Push(startTag);

                            continue;
                        }
                        else if (tag == "<n/>" || tag == "</n>") // line breakers
                        {
                            tag = "<n/>";

                            // nested
                            if (!string.IsNullOrEmpty(startTag))
                            {
                                ElementStack.Push(CreateEndTag(startTag));
                                ElementStack.Push(tag);
                                ElementStack.Push(startTag);
                            }
                            else
                            {
                                ElementStack.Push(tag);
                            }

                            continue;
                        }
                        else // end tag
                        {
                            if (MatchElement(startTag, tag))
                            {
                                if (ElementStack.Peek() != startTag)
                                    ElementStack.Push(tag);
                                else
                                    ElementStack.Pop();
                                return;
                            }
                            else if (tag.Length > 4 && tag.EndsWith("/>")) // self close tag
                            {
                                ElementStack.Push(tag);
                                continue;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }

                if (Iterator >= DescriptionString.Length)
                    break;

                sb.Append(DescriptionString[Iterator]);
            }

            if (sb.Length > 0)
            {
                if (ElementStack.Count > 0 && ElementStack.Peek() == startTag)
                    ElementStack.Pop();

                ElementStack.Push(sb.ToString());
            }
        }

        // removes all tags
        private string ParsePlainText(bool includeNewlineTags, bool includeScaling)
        {
            StringBuilder sb = new StringBuilder();
            for (; Iterator < DescriptionString.Length; Iterator++)
            {
                if (DescriptionString[Iterator] == '<')
                {
                    if (sb.Length > 0)
                    {
                        ElementStack.Push(sb.ToString());
                        sb = new StringBuilder();
                    }

                    if (TryParseTag(out string tag, out bool isStartTag))
                    {
                        if (tag == "<n/>")
                        {
                            if (includeNewlineTags)
                                ElementStack.Push(tag);
                            else
                                ElementStack.Push(" ");
                        }

                        continue;
                    }
                }
                else if (DescriptionString[Iterator] == '~' && Iterator + 1 < DescriptionString.Length && DescriptionString[Iterator + 1] == '~')
                {
                    ElementStack.Push(sb.ToString());
                    sb = new StringBuilder();

                    if (ParseScalingTag(out string scaleText, includeScaling))
                    {
                        ElementStack.Push(scaleText);
                    }

                    continue;
                }

                if (Iterator >= DescriptionString.Length)
                    break;

                sb.Append(DescriptionString[Iterator]);
            }

            if (sb.Length > 0)
                ElementStack.Push(sb.ToString());

            sb = new StringBuilder();

            while (ElementStack.Count > 0)
            {
                sb.Insert(0, ElementStack.Pop());
            }

            return sb.ToString().Trim();
        }

        // keeps colored tags and new lines
        private string ParseColoredText(bool includeScaling)
        {
            StringBuilder sb = new StringBuilder();
            for (; Iterator < DescriptionString.Length; Iterator++)
            {
                if (DescriptionString[Iterator] == '<')
                {
                    if (sb.Length > 0)
                    {
                        ElementStack.Push(sb.ToString());
                        sb = new StringBuilder();
                    }

                    if (TryParseTag(out string tag, out bool isStartTag))
                    {
                        ElementStack.Push(tag);

                        continue;
                    }
                }
                else if (DescriptionString[Iterator] == '~' && Iterator + 1 < DescriptionString.Length && DescriptionString[Iterator + 1] == '~')
                {
                    ElementStack.Push(sb.ToString());
                    sb = new StringBuilder();

                    if (ParseScalingTag(out string scaleText, includeScaling))
                    {
                        ElementStack.Push(scaleText);
                    }

                    continue;
                }

                if (Iterator >= DescriptionString.Length)
                    break;

                sb.Append(DescriptionString[Iterator]);
            }

            if (sb.Length > 0)
                ElementStack.Push(sb.ToString());

            sb = new StringBuilder();

            while (ElementStack.Count > 0)
            {
                sb.Insert(0, ElementStack.Pop());
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
            StringBuilder sb = new StringBuilder();
            for (; Iterator < DescriptionString.Length; Iterator++)
            {
                sb.Append(DescriptionString[Iterator]);
                if (DescriptionString[Iterator] == '>')
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
            StringBuilder sb = new StringBuilder();

            for (; Iterator < DescriptionString.Length; Iterator++)
            {
                if (DescriptionString[Iterator] == '~')
                    tagCount++;
                else
                    sb.Append(DescriptionString[Iterator]);

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
