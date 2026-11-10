using System.Text;
using System.Text.RegularExpressions;

namespace CrystalLens.Models
{
    public record ASMCommand(string Command, string[] Parameters, string Comment)
    {
        public int Count => Parameters.Length;

        public ASMCommand(string command, object[] parameters, string comment)
            : this(command, [.. parameters.Select(o => o.ToString() ?? "0")], comment)
        { }

        public ASMCommand(string command, string[] parameters) : this(command, parameters, string.Empty)
        { }

        public ASMCommand(string command, object[] parameters) : this(command, parameters, string.Empty)
        { }

        public ASMCommand(string command) : this(command, [])
        { }

        public ASMCommand() : this(string.Empty)
        { }

        public static ASMCommand FromLine(string line)
        {
            int commentIndex = IndexOfComment(line);
            string instruction = commentIndex >= 0 ? line[0..commentIndex].Trim() : line.Trim();
            string comment = commentIndex >= 0 && (commentIndex + 1) < line.Length ? line[(commentIndex + 1)..] : string.Empty;
            
            string[] commandParamSplit = instruction.Split(" ", 2);
            string command = commandParamSplit[0];
            string[] parameters;

            if (commandParamSplit.Length > 1)
            {
                parameters = commandParamSplit[1].Split(",");
                for (int i = 0; i < parameters.Length; i++)
                {
                    parameters[i] = parameters[i].Trim();
                }
            }
            else
            {
                parameters = [];
            }

            return new(command, parameters, comment);
        }

        private static int IndexOfComment(string line)
        {
            int i;
            char c;
            bool inQuotes = false;
            for (i = 0; i < line.Length && ((c = line[i]) != ';' || inQuotes); i++)
            {
                if (c == '"')
                {
                    inQuotes ^= true;
                }
                else if (c == '\\')
                {
                    i++;
                }
            }
            return i < line.Length ? i : -1;
        }

        public string Get(int index)
        {
            return Parameters[index];
        }

        public int GetInt(int index)
        {
            return int.Parse(Get(index));
        }

        public byte GetByte(int index)
        {
            return byte.Parse(Get(index));
        }

        public bool IsComment()
        {
            return Command.IsWhiteSpace();
        }

        internal ASMCommand WithComment(string comment)
        {
            return new(Command, Parameters, comment);
        }

        public void VerifyOrThrow(string expected)
        {
            if (Command != expected)
            {
                throw new InvalidOperationException("Unexpected command: " + this);
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new(Command);
            for (int i = 0; i < Parameters.Length; i++)
            {
                string parameter = Parameters[i];
                builder.Append(' ').Append(parameter);

                if (i < Parameters.Length - 1)
                {
                    builder.Append(',');
                }
            }
            if (!Comment.IsWhiteSpace())
            {
                if (builder.Length > 0)
                {
                    builder.Append(' ');
                }
                builder.Append($"; {Comment}");
            }
            return builder.ToString();
        }

        public static bool HasLabel(string line)
        {
            return Regex.IsMatch(line, "^[A-Za-z_][\\w#$@]+(\\.[A-Za-z_][\\w#$@]+)?::?");
        }
    }
}
