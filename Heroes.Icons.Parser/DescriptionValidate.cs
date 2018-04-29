using System.Collections.Generic;
using System.Text;

namespace Heroes.Icons.Parser
{
    public class DescriptionValidate
    {
        private string DescriptionString;

        private int Iterator = 0;
        private Stack<string> ElementStack = new Stack<string>();

        private DescriptionValidate(string descriptionString)
        {
            DescriptionString = descriptionString;
        }

        /// <summary>
        /// Takes a description string and removes unmatched and nested tags
        /// </summary>
        /// <param name="descriptionString">The description string</param>
        /// <returns></returns>
        public static string Validate(string descriptionString)
        {
            return new DescriptionValidate(descriptionString).Parse();
        }

        private string Parse()
        {
            ParseDescription(string.Empty);

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

        private void ParseDescription(string startTag)
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
                            ParseDescription(tag);

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
                    return true;
                }
            }

            isStartTag = false;
            tag = sb.ToString().ToLower();

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
    }
}
