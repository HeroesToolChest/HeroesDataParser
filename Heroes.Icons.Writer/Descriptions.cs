using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Heroes.Icons.FileWriter
{
    public static class Descriptions
    {
        public static void WriteShortDescriptions(List<string> list)
        {
            using (StreamWriter writer = new StreamWriter("_ShortTalentTooltips.txt"))
            {
                foreach (var item in list)
                {
                    writer.WriteLine(item);
                }
            }
        }

        public static void WriteFullDescriptions(List<string> list)
        {
            using (StreamWriter writer = new StreamWriter("_FullTalentTooltips.txt"))
            {
                foreach (var item in list)
                {
                    writer.WriteLine(CleanDescription(item));
                }
            }
        }

        public static void WriteHeroDescriptions(List<string> list)
        {
            using (StreamWriter writer = new StreamWriter("_HeroDescriptions.txt"))
            {
                foreach (var item in list)
                {
                    writer.WriteLine(item);
                }
            }
        }

        private static string CleanDescription(string description)
        {
            Stack<string> stack = new Stack<string>();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < description.Length; i++)
            {
                if (description[i] == '<' && sb.Length > 0)
                {
                    stack.Push(sb.ToString());
                    sb = new StringBuilder();
                }
                else if (description[i] == '>')
                {
                    sb.Append(description[i]);
                    string currentReadText = sb.ToString();

                    // is it an ending tag
                    if (currentReadText == "</c>")
                    {
                        StringBuilder coloredText = new StringBuilder();
                        coloredText.Append(currentReadText);
                        bool found = false;
                        while (stack.Count > 0)
                        {
                            string text = stack.Pop();
                            coloredText.Insert(0, text);
                            if (text.ToLower().StartsWith("<c ") && !text.ToLower().EndsWith("</c>"))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (found)
                            stack.Push(coloredText.ToString());
                        else
                            stack.Push(coloredText.ToString().Remove(coloredText.ToString().Length - 4, 4));
                    }
                    else if (currentReadText == "</s>")
                    {
                        StringBuilder coloredText = new StringBuilder();
                        coloredText.Append(currentReadText);
                        bool found = false;
                        while (stack.Count > 0)
                        {
                            string text = stack.Pop();
                            coloredText.Insert(0, text);
                            if (text.ToLower().StartsWith("<s ") && !text.ToLower().EndsWith("</s>"))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (found)
                            stack.Push(coloredText.ToString());
                        else
                            stack.Push(coloredText.ToString().Remove(coloredText.ToString().Length - 4, 4));
                    }
                    else
                    {
                        stack.Push(currentReadText);
                    }

                    sb = new StringBuilder();
                    continue;
                }

                sb.Append(description[i]);
            }

            stack.Push(sb.ToString());

            StringBuilder parsedSb = new StringBuilder();
            while (stack.Count > 0)
            {
                parsedSb.Insert(0, stack.Pop());
            }

            return parsedSb.ToString();
        }
    }
}
