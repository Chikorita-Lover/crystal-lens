using System.Text;

namespace CrystalLens.Models
{
    internal record ASMCommand(string Command, string[] Parameters, string Comment)
    {
        public ASMCommand(string command, string[] parameters) : this(command, parameters, string.Empty)
        { }

        public static ASMCommand FromLine(string line)
        {
            string[] codeCommentSplit = line.Trim().Split(";", 2);
            string[] commandParamSplit = codeCommentSplit[0].Split(" ", 2);
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

            string comment = codeCommentSplit.Length > 1 ? codeCommentSplit[1] : string.Empty;
            return new(command, parameters, comment);
        }

        public string Get(int index)
        {
            return Parameters[index];
        }

        public int GetInt(int index)
        {
            return int.Parse(Get(index));
        }

        public bool IsComment()
        {
            return Command.IsWhiteSpace();
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
            return builder.ToString();
        }
    }
}
