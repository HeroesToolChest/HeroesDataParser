using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace HeroesData.Parser
{
    public static class MathEval
    {
        public static double CalculatePathEquation(string input)
        {
            DataTable datatable = new DataTable();

            input = ModifyOperators(input);

            var regex = new Regex(@"\(([^()]+|(?<Level>\()|(?<-Level>\)))+(?(Level)(?!))\)", RegexOptions.IgnorePatternWhitespace);

            var matches = regex.Matches(input);

            foreach (Match match in matches)
            {
                double value = double.Parse(datatable.Compute(match.Value, string.Empty).ToString());

                int position = input.IndexOf(match.Value);
                if (position >= 0)
                {
                    input = input.Substring(0, position) + value + input.Substring(position + match.Length);
                }
            }

            input = RemoveAllSpaces(input);
            input = RemoveDoubleNegative(input);
            string[] parts = SplitExpression(input);

            while (parts.Length > 2)
            {
                string toBeComputed = string.Empty;

                if (parts[0] == "-") // first is negative number
                {
                    toBeComputed = parts[0] + parts[1] + parts[2];

                    if (parts[3] == "-") // second is negative number
                        toBeComputed += parts[3] + parts[4];
                    else
                        toBeComputed += parts[3];
                }
                else
                {
                    if (parts[2] == "-")
                        toBeComputed = parts[0] + parts[1] + parts[2] + parts[3];
                    else
                        toBeComputed = parts[0] + parts[1] + parts[2];
                }

                double value = double.Parse(datatable.Compute(toBeComputed, string.Empty).ToString());

                int pos = input.IndexOf(toBeComputed);
                if (pos >= 0)
                {
                    input = input.Substring(0, pos) + value + input.Substring(pos + toBeComputed.Length);
                }

                parts = SplitExpression(input);
            }

            return double.Parse(input);
        }

        private static string[] SplitExpression(string input)
        {
            return Regex.Split(input, @"([+\-*/])").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        }

        private static string ModifyOperators(string pathLookup)
        {
            pathLookup = RemoveDoubleNegative(pathLookup);

            string pattern1 = @"\(\-*\d*\.*\d*\+\(\-*\d*\.*\d*\)";
            string pattern2 = @"\(\d*/\d*\)\-\d*";

            if (Regex.IsMatch(pathLookup, pattern1))
                return Regex.Replace(pathLookup, pattern1, "($0)");

            if (Regex.IsMatch(pathLookup, @"\(\d*/\d*\)\-\d*\*\d*\)"))
                return Regex.Replace(pathLookup, pattern2, "($0)").TrimEnd(')');

            return pathLookup;
        }

        private static string RemoveDoubleNegative(string input)
        {
            if (input.Length > 2 && input.StartsWith("--"))
                input = input.Remove(0, 2);

            return input.Replace("--", "+");
        }

        private static string RemoveAllSpaces(string input)
        {
            return Regex.Replace(input, @"\s+", string.Empty);
        }
    }
}
