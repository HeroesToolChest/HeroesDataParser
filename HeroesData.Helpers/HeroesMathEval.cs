using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HeroesData.Helpers
{
    public static class HeroesMathEval
    {
        private static readonly DataTable _dataTable = new DataTable();

        public static double CalculatePathEquation(string input)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            input = Validate(input);
            input = GetInnerParenthesisValues(input);

            // finalize calculations
            input = CalculateInput(input);

            return double.Parse(input);
        }

        private static string GetInnerParenthesisValues(string input)
        {
            MatchCollection matches = Regex.Matches(input, @"\(([^()]+|(?<Level>\()|(?<-Level>\)))+(?(Level)(?!))\)", RegexOptions.IgnorePatternWhitespace);

            foreach (Match? match in matches)
            {
                if (match == null)
                    continue;

                string matchInput = match.Value;

                if (matchInput.StartsWith("(", StringComparison.OrdinalIgnoreCase))
                    matchInput = matchInput[1..];
                if (matchInput.EndsWith(")", StringComparison.OrdinalIgnoreCase))
                    matchInput = matchInput.Remove(matchInput.Length - 1);

                matchInput = GetInnerParenthesisValues(matchInput);

                int position = input.IndexOf(match.Value, StringComparison.OrdinalIgnoreCase);
                if (position >= 0)
                {
                    input = input.Substring(0, position) + CalculateInput(matchInput) + input[(position + match.Length)..];
                }
            }

            return input;
        }

        // order of operations is not followed, calculate left to right
        private static string CalculateInput(string input)
        {
            string[] parts = SplitExpression(input);

            while (parts.Length > 2)
            {
                StringBuilder stringBuilder = new StringBuilder();

                int numberCount = 0;

                // loop through, get the first two operands
                foreach (string part in parts)
                {
                    stringBuilder.Append(part);

                    // check if it's a number
                    if (double.TryParse(part, out _))
                    {
                        numberCount++;
                    }

                    // got two, we're done
                    if (numberCount >= 2)
                        break;
                }

                string toBeComputed = stringBuilder.ToString();
                double value = double.Parse(_dataTable.Compute(toBeComputed, string.Empty).ToString()!);

                int pos = input.IndexOf(toBeComputed, StringComparison.OrdinalIgnoreCase);
                if (pos >= 0)
                {
                    input = input.Substring(0, pos) + value + input[(pos + toBeComputed.Length)..];
                }

                parts = SplitExpression(input);
            }

            return input;
        }

        private static string[] SplitExpression(string input)
        {
            return Regex.Split(input, @"([+\-*/])").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        }

        private static string RemoveDoubleNegative(string input)
        {
            if (input.Length > 2 && input.StartsWith("*--", StringComparison.OrdinalIgnoreCase))
                input = input.Remove(0, 1);
            if (input.Length > 2 && input.StartsWith("--", StringComparison.OrdinalIgnoreCase))
                input = input.Remove(0, 2);

            return input.Replace("--", "+", StringComparison.OrdinalIgnoreCase);
        }

        // remove all white space and multi operators
        private static string Validate(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            int leftParenthesesCount = 0;
            int rightParenthesesCount = 0;
            char? lastOperator = null;

            StringBuilder builder = new StringBuilder();

            foreach (char character in input)
            {
                // remove whitespace
                if (char.IsWhiteSpace(character))
                    continue;

                if (character == '(')
                {
                    leftParenthesesCount++;
                }
                else if (character == ')')
                {
                    rightParenthesesCount++;
                }

                if (IsOperator(character))
                {
                    // keep track of the last operator
                    lastOperator = character;
                }
                else if (lastOperator is not null)
                {
                    // only append the last operator
                    builder.Append(lastOperator);
                    builder.Append(character);

                    lastOperator = null;
                }
                else
                {
                    builder.Append(character);
                }
            }

            // add parenthesis to left
            while (leftParenthesesCount < rightParenthesesCount)
            {
                builder.Insert(0, '(');
                leftParenthesesCount++;
            }

            // add parenthesis to right
            while (leftParenthesesCount > rightParenthesesCount)
            {
                builder.Append(')');
                rightParenthesesCount++;
            }

            return builder.ToString();
        }

        private static bool IsOperator(char value) => value == '*' || value == '-' || value == '+' || value == '/';
    }
}
