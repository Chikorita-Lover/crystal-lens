using System.IO;
using System.Text;

namespace CrystalLens.Models.Expression
{
    internal class ExpressionReader
    {
        private readonly StringReader _reader;

        internal ExpressionReader(string expression)
        {
            _reader = new(expression);
        }

        internal void Close()
        {
            _reader.Close();
        }

        internal string? Read()
        {
            int i = _reader.Read();
            if (i < 0)
            {
                return null;
            }
            char c = (char)i;
            switch (c)
            {
                case ' ':
                    break;
                case '~' or '?' or '@' or '(' or ')' or ',' or '+' or '-' or '/' or '^':
                    return c.ToString();
                case '*':
                    return OneOrTwo('*', "**");
                case '|':
                    return OneOrTwo('|', "||");
                case '=':
                    if (_reader.Read() == '=')
                    {
                        return "==";
                    }
                    else
                    {
                        throw new FormatException();
                    }
                case '!':
                    return OneOrTwo('!', "!=");
                case '<':
                    return OneOrTwo('<', "<=", "<<");
                case '>':
                    switch (_reader.Peek())
                    {
                        case '=':
                            _reader.Read();
                            return ">=";
                        case '>':
                            _reader.Read();
                            if (_reader.Peek() == '>')
                            {
                                _reader.Read();
                                return ">>>";
                            }
                            else
                            {
                                return ">>";
                            }
                        default:
                            return ">";
                    }
                case '0':
                    switch (_reader.Peek())
                    {
                        case 'x' or 'X':
                            _reader.Read();
                            return ReadNumber(16).ToString();
                        case 'o' or 'O':
                            _reader.Read();
                            return ReadNumber(8).ToString();
                        case 'b' or 'B':
                            _reader.Read();
                            return ReadNumber(2).ToString();
                    }
                    goto case '1';
                case '1':
                case '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9':
                    return ReadNumber(10, c.ToString()).ToString();
                case '&':
                    i = _reader.Peek();
                    if (i >= 0)
                    {
                        c = (char)i;
                        if (IsBaseDigit(c, 8))
                        {
                            return ReadNumber(8).ToString();
                        }
                        if (_reader.Peek() == '&')
                        {
                            _reader.Read();
                            return "&&";
                        }
                    }
                    return "&";
                case '%':
                    i = _reader.Peek();
                    if (i >= 0)
                    {
                        c = (char)i;
                        if (IsBaseDigit(c, 2))
                        {
                            return ReadNumber(2).ToString();
                        }
                    }
                    return "%";
                case '$':
                    return ReadNumber(16).ToString();
                // TODO case '`'
                default:
                    if (c != '_' && !char.IsAsciiLetter(c))
                    {
                        throw new FormatException();
                    }
                    return ReadSymbol(c);
            }
            return Read();
        }

        private string OneOrTwo(char c, params string[] twos)
        {
            int next = _reader.Peek();
            foreach (string two in twos)
            {
                if (next == two[1])
                {
                    _reader.Read();
                    return two;
                }
            }
            return c.ToString();
        }

        private int ReadNumber(int fromBase = 10, string start = "")
        {
            int i;
            char c;
            while ((i = _reader.Peek()) >= 0 && IsBaseDigit(c = (char)i, fromBase))
            {
                _reader.Read();
                if (c != '_')
                {
                    start += c;
                }
            }
            return Convert.ToInt32(start, fromBase);
        }

        private string ReadSymbol(char start)
        {
            StringBuilder builder = new(start.ToString());
            int i;
            char c;
            while ((i = _reader.Peek()) >= 0 && IsSymbolLetter(c = (char)i))
            {
                _reader.Read();
                builder.Append(c);
            }
            return builder.ToString();
        }

        private static bool IsBaseDigit(char c, int fromBase)
        {
            if (fromBase == 16)
            {
                return c == '_' || char.IsAsciiHexDigit(c);
            }
            else if (fromBase >= 2 && fromBase <= 10)
            {
                return c == '_' || char.IsBetween(c, '0', (char)('0' + fromBase - 1));
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(fromBase));
            }
        }

        private static bool IsSymbolLetter(char c)
        {
            return char.IsAsciiLetter(c) || char.IsNumber(c) || c == '_' || c == '#' || c == '$' || c == '@';
        }
    }
}
