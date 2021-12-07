using System;
using System.Collections.Generic;
using System.Data;

namespace HeroesData.Helpers
{
    public static class HeroesMathEval
    {
        public static double CalculatePathEquation(string input)
        {
            try
            {
                return Compute(input);
            }
            catch
            {
                throw new SyntaxErrorException($"Syntax error in expression: {input}");
            }
        }

        /* Only parentheses have precedence
         * Divide by 0 results in the numberator
         * With multiple operators in a row, only take the last one
         * account for mismatch parenthesis (too many or too little)
         */
        private static double Compute(string expression)
        {
            Stack<double> values = new();
            Stack<char> operators = new();
            ReadOnlySpan<char> tokens = expression.AsSpan();

            char lastReadToken = '\0';

            for (int i = 0; i < tokens.Length; i++)
            {
                // ignore space
                if (tokens[i] == ' ')
                    continue;

                // check if digit
                if (IsDigit(tokens[i]) || tokens[i] == '.')
                {
                    GetNumber(tokens, values, ref i, false);
                }
                else if (!IsOperator(lastReadToken) && lastReadToken != ')' && tokens[i] == '-' && !IsDigit(lastReadToken) && (i + 1) < tokens.Length && IsDigit(tokens[i + 1])) // negative number check
                {
                    if (operators.Count > 0 && operators.Peek() != '(' && values.Count < 1) // no values, then its a negative number
                    {
                        operators.Pop();
                        GetNumber(tokens, values, ref i, true);
                    }
                    else
                    {
                        GetNumber(tokens, values, ref i, true);
                    }
                }
                else if (tokens[i] == '(')
                {
                    operators.Push(tokens[i]);
                }
                else if (tokens[i] == ')')
                {
                    while (operators.Count > 0 && operators.Peek() != '(')
                    {
                        if (operators.Peek() == 'b')
                        {
                            values.Push(values.Pop() * -1);
                            operators.Pop();
                        }
                        else
                        {
                            values.Push(ApplyOperator(operators.Pop(), values.Pop(), values.Pop()));
                        }
                    }

                    if (operators.Count > 0)
                        operators.Pop(); // this pops the left parentheses
                }
                else if (IsOperator(tokens[i]))
                {
                    if (IsOperator(lastReadToken))
                    {
                        // multiple operators in a row, only take the last one
                        operators.Pop();
                        operators.Push(tokens[i]);
                    }
                    else if (values.Count == 1 && operators.Count > 0 && operators.Peek() == '-')
                    {
                        operators.Pop();
                        values.Push(values.Pop() * -1);
                        operators.Push(tokens[i]);
                    }
                    else if (tokens[i] == '-' && lastReadToken == '(' && (i + 1) < tokens.Length && tokens[i + 1] == '(') // - in between ( and (
                    {
                        operators.Push('b');
                    }
                    else
                    {
                        while (operators.Count > 0 && CheckPrecedence(tokens[i], operators.Peek()))
                        {
                            if (operators.Peek() == 'b')
                            {
                                values.Push(values.Pop() * -1);
                                operators.Pop();
                            }
                            else
                            {
                                values.Push(ApplyOperator(operators.Pop(), values.Pop(), values.Pop()));
                            }
                        }

                        operators.Push(tokens[i]);
                    }
                }

                lastReadToken = tokens[i];
            }

            // pop remaining operators
            while (operators.Count > 0)
            {
                if (values.Count == 1 && operators.Count == 1 && operators.Peek() == '-')
                {
                    operators.Pop();
                    values.Push(values.Pop() * -1);
                }
                else
                {
                    if (operators.Peek() == '(')
                        operators.Pop();
                    else if (values.Count > 1)
                        values.Push(ApplyOperator(operators.Pop(), values.Pop(), values.Pop()));
                    else
                        operators.Pop();
                }
            }

            while (values.Count > 1)
            {
                values.Push(ApplyOperator('+', values.Pop(), values.Pop()));
            }

            // final result value
            return values.Pop();
        }

        private static void GetNumber(ReadOnlySpan<char> tokens, Stack<double> values, ref int i, bool isNegative)
        {
            int beginning = i;

            if (isNegative)
                i++; // negative sign

            bool hasDecimal = false;

            // get entire number
            while (i < tokens.Length)
            {
                if (IsDigit(tokens[i])) // digit
                {
                    i++;
                }
                else if (!hasDecimal && tokens[i] == '.') // decimal
                {
                    hasDecimal = true;
                    i++;
                }
                else
                {
                    break;
                }
            }

            values.Push(double.Parse(tokens[beginning..i]));

            i--;
        }

        private static bool IsDigit(char token)
        {
            if (token >= '0' && token <= '9')
                return true;
            else
                return false;
        }

        // only parentheses has precedence
        // if operator2 has same or higher then return true, otherwise return false
#pragma warning disable IDE0060 // Remove unused parameter
        private static bool CheckPrecedence(char operator1, char operator2)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (operator2 == '(' || operator2 == ')')
                return false;
            ////else if ((operator1 == '*' || operator1 == '/') && (operator2 == '+' || operator2 == '-'))
            ////   return false;
            else
                return true;
        }

        private static double ApplyOperator(char op, double b, double a)
        {
            switch (op)
            {
                case '+':
                    return a + b;
                case '-':
                    return a - b;
                case '*':
                    return a * b;
                case '/':
                    if (b == 0)
                    {
                        // if divide by 0, return the numerator
                        return a;
                    }

                    return a / b;
            }

            throw new InvalidOperationException($"Invalid operator: {op}");
        }

        private static bool IsOperator(char value) => value == '*' || value == '-' || value == '+' || value == '/';
    }
}
