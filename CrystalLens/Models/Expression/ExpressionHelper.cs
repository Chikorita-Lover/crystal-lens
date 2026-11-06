namespace CrystalLens.Models.Expression
{
    public static class ExpressionHelper
    {
        internal static readonly Dictionary<string, Func<int, int>> BuiltInFunctions;
        private static readonly Dictionary<string, Func<int, int>> UnaryOperations;
        private static readonly Dictionary<string, Func<int, int, int>> BinaryOperations;
        private static readonly Dictionary<string, int> OperatorPrecedence;

        static ExpressionHelper()
        {
            BuiltInFunctions = new()
            {
                { "HIGH", n => (n & 0xFF00) >> 8 },
                { "LOW", n => n & 0xFF },
                { "BITWIDTH", n => (int)Math.Floor(Math.Log2(n)) + 1 }
            };
            UnaryOperations = new()
            {
                { "+", n => n },
                { "-", n => -n },
                { "~", n => ~n },
                { "!", n => n == 0 ? 1 : 0 }
            };
            BinaryOperations = new()
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
            OperatorPrecedence = new()
            {
                { "||", 0 },
                { "&&", 1 },
                { "==", 2 }, { "!=", 2 }, { "<", 2 }, { ">", 2 }, { "<=", 2 }, { ">=", 2 },
                { "+", 3 }, { "-", 3 },
                { "&", 4 }, { "|", 4 }, { "^", 4 },
                { "<<", 5 }, { ">>", 5 }, { ">>>", 5 },
                { "*", 6 }, { "/", 6 }, { "%", 6 },
                { "~", 7 }, { "!", 7 },
                { "**", 8 }
            };
        }

        public static List<string> Tokenize(string expression)
        {
            ExpressionReader reader = new(expression);
            List<string> tokens = [];
            string? token;
            while ((token = reader.Read()) != null)
            {
                tokens.Add(token);
            }
            return tokens;
        }

        public static int Evaluate(List<string> tokens)
        {
            int index;
            if ((index = tokens.IndexOf("(")) >= 0)
            {
                int endIndex = IndexOfClosing(tokens, index);
                List<string> slice = tokens[(index + 1)..endIndex];
                tokens.RemoveRange(index + 1, slice.Count + 1);
                int evaluation = Evaluate(slice);

                if (index > 0 && BuiltInFunctions.TryGetValue(tokens[index - 1], out Func<int, int>? operation))
                {
                    evaluation = operation.Invoke(evaluation);
                    tokens.RemoveAt(index--);
                }

                tokens[index] = evaluation.ToString();
                return Evaluate(tokens);
            }

            int lowestOpIndex = -1;
            int lowestPrecedence = int.MaxValue;
            for (index = tokens.Count - 1; index >= 0; index--)
            {
                string token = tokens[index];
                if (OperatorPrecedence.TryGetValue(token, out int precedence))
                {
                    if ((token == "+" || token == "-") && (index == 0 || !int.TryParse(tokens[index - 1], out _)))
                    {
                        precedence = 7; // unary plus/minus
                    }
                    if (precedence < lowestPrecedence || precedence == 8 && precedence == lowestPrecedence) // ** is right-associative
                    {
                        lowestOpIndex = index;
                        lowestPrecedence = precedence;
                    }
                }
            }
            if (lowestOpIndex >= 0)
            {
                string op = tokens[lowestOpIndex];
                if (lowestPrecedence == 7) // any unary operator
                {
                    return UnaryOperations[op].Invoke(Evaluate(tokens[(lowestOpIndex + 1)..]));
                }
                else
                {
                    return BinaryOperations[op].Invoke(Evaluate(tokens[..lowestOpIndex]), Evaluate(tokens[(lowestOpIndex + 1)..]));
                }
            }

            return int.Parse(tokens[0]);
        }

        private static int IndexOfClosing(List<string> tokens, int startIndex)
        {
            int i;
            string token;
            int layer = 0;
            for (i = startIndex + 1; i < tokens.Count && ((token = tokens[i]) != ")" || (layer > 0)); i++)
            {
                switch (token)
                {
                    case "(":
                        layer++;
                        break;
                    case ")":
                        layer--;
                        break;
                }
            }
            return i;
        }
    }
}
