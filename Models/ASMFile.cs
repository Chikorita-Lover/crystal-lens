using System.IO;

namespace CrystalLens.Models
{
    internal class ASMFile
    {
        public readonly string Path;
        public string Name
        {
            get => Path.Split('\\').Last();
        }
        private readonly Dictionary<string, Queue<ASMCommand>> labelToCommands = [];

        public ASMFile(string path)
        {
            Path = path;
        }

        public Queue<ASMCommand> GetCommands(string label)
        {
            return new(labelToCommands[label]);
        }

        public ICollection<string> GetLabels()
        {
            return labelToCommands.Keys;
        }

        public void ReadFile()
        {
            StreamReader sr = new(Path);
            string? line;
            Queue<ASMCommand> commands = [];
            while ((line = sr.ReadLine()) != null)
            {
                ASMCommand command = ASMCommand.FromLine(line);
                if (command.Command.EndsWith(':'))
                {
                    string label = command.Command.Split(':')[0];
                    commands = [];
                    labelToCommands.Add(label, commands);
                }
                else if (!command.Command.IsWhiteSpace())
                {
                    commands.Enqueue(command);
                }
            }
        }
    }
}
