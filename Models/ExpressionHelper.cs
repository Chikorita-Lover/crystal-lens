using System.Text;

namespace CrystalLens.Models
{
    internal static class ExpressionHelper
    {
        private static readonly Dictionary<string, Func<int, int, int>> OperatorFunctions;
        private static readonly string[][] PrioritizedOperators;

        static ExpressionHelper()
        {
            OperatorFunctions = new()
            {
                { "+", (a, b) => a + b },
                { "-", (a, b) => a - b },
                { "*", (a, b) => a * b },
                { "/", (a, b) => a / b },
                { "%", (a, b) => a % b },
                { "&", (a, b) => a & b },
                { "|", (a, b) => a | b },
                { "^", (a, b) => a ^ b },
                { "<<", (a, b) => a << b },
                { ">>", (a, b) => a >> b },
                { ">>>", (a, b) => a >>> b },
                { "**", (a, b) => (int)Math.Pow(a, b) },
                { "&&", (a, b) => (a != 0) && (b != 0) ? 1 : 0 },
                { "||", (a, b) => (a != 0) || (b != 0) ? 1 : 0 },
                { "==", (a, b) => a == b ? 1 : 0 },
                { "!=", (a, b) => a != b ? 1 : 0 },
                { "<", (a, b) => a < b ? 1 : 0 },
                { ">", (a, b) => a > b ? 1 : 0 },
                { "<=", (a, b) => a <= b ? 1 : 0 },
                { ">=", (a, b) => a >= b ? 1 : 0 }
            };
            PrioritizedOperators = [
                [ "(", ")" ],
                [ "**" ],
                [ "~", "!" ],
                [ "*", "/", "%" ],
                [ "<<", ">>", ">>>" ],
                [ "&", "|", "^" ],
                [ "+", "-" ],
                [ "==", "!=", "<", ">", "<=", ">=" ],
                [ "&&" ],
                [ "||" ]
                ];
        }

        internal static int Evaluate(string expression)
        {
            int index;
            if ((index = expression.IndexOf('(')) >= 0)
            {
                int endIndex = IndexOfClosing(expression, index + 1);
                string slice = expression[(index + 1)..endIndex];
                string newExpression = expression[0..index] + Evaluate(slice).ToString();
                if (endIndex < expression.Length - 1)
                {
                    newExpression += expression[(endIndex + 1)..expression.Length];
                }
                return Evaluate(newExpression);
            }

            bool isNumber = char.IsAsciiDigit(expression[^1]);
            List<string> symbols = [];
            StringBuilder sb = new();
            for (index = expression.Length - 1; index >= 0; index--)
            {
                char c = expression[index];
                switch (c)
                {
                    case '_':
                        if (!isNumber || sb.Length == 0)
                        {
                            throw new FormatException();
                        }
                        continue;
                    case ' ':
                        if (sb.Length > 0)
                        {
                            isNumber ^= true;
                            symbols.Insert(0, sb.ToString());
                            sb.Clear();
                        }
                        break;
                    default:
                        if (char.IsNumber(c) != isNumber)
                        {
                            if (!isNumber || c != '-' || index != 0 && char.IsNumber(expression[index - 1]))
                            {
                                isNumber ^= true;
                                symbols.Insert(0, sb.ToString());
                                sb.Clear();
                            }
                        }
                        sb.Insert(0, c);
                        break;
                }
            }
            if (sb.Length > 0)
            {
                symbols.Insert(0, sb.ToString());
            }

            for (int i = 0; i < PrioritizedOperators.Length; i++)
            {
                string[] operators = PrioritizedOperators[i];
                while ((index = symbols.FindIndex(operators.Contains)) >= 0)
                {
                    string match = symbols[index];
                    symbols.RemoveAt(index);
                    if (match == "~" || match == "!")
                    {
                        int a = int.Parse(symbols[index]);

                        if (match == "~")
                        {
                            symbols[index] = (~a).ToString();
                        }
                        else
                        {
                            symbols[index] = a == 0 ? "1" : "0";
                        }
                    }
                    else
                    {
                        int a = int.Parse(symbols[index - 1]);
                        int b = int.Parse(symbols[index]);
                        symbols.RemoveAt(index);

                        symbols[--index] = OperatorFunctions[match].Invoke(a, b).ToString();
                    }
                }
            }
            return int.Parse(symbols[0]);
        }

        private static int IndexOfClosing(string expression, int startIndex)
        {
            int i;
            char c;
            int layer = 0;
            for (i = startIndex; i < expression.Length && ((c = expression[i]) != ')' || (layer > 0)); i++)
            {
                switch (c)
                {
                    case '(':
                        layer++;
                        break;
                    case ')':
                        layer--;
                        break;
                }
            }
            return i;
        }
    }
}
