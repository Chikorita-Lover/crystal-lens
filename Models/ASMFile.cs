using System.IO;

namespace CrystalLens.Models
{
    public class ASMFile
    {
        public string Path { get; set; }
        private readonly Dictionary<string, IASMData> labeledData = [];

        public ICollection<string> Labels => labeledData.Keys;

        private ASMFile(string path)
        {
            Path = path;
        }

        public IASMData Get(string label)
        {
            return labeledData[label];
        }

        public static ASMFile ReadFile(string path)
        {
            ASMFile file = new(path);

            StreamReader reader = new(path);
            string? line;
            Queue<ASMCommand> commands = [];
            Dictionary<string, Queue<ASMCommand>> labeledCommands = [];
            while ((line = reader.ReadLine()) != null)
            {
                ASMCommand command = ASMCommand.FromLine(line);
                if (command.Command.EndsWith(':'))
                {
                    string label = command.Command.Split(':')[0];
                    commands = [];
                    labeledCommands.Add(label, commands);
                }
                else if (!command.Command.IsWhiteSpace())
                {
                    commands.Enqueue(command);
                }
            }

            foreach (string label in labeledCommands.Keys)
            {
                commands = labeledCommands[label];
                ASMDataType? type = DetermineDataType(commands);
                if (type == null)
                {
                    throw new FileFormatException($"ASM file \"{path}\" contains unsupported data format");
                }
                IASMData data = type.Serializer.ReadAssembly(commands);
                file.labeledData.Add(label, data);
            }

            reader.Close();
            return file;
        }

        private static ASMDataType? DetermineDataType(Queue<ASMCommand> commands)
        {
            foreach (ASMDataType type in ASMDataType.Values)
            {
                Queue<ASMCommand> copy = new(commands);
                if (type.CommandPredicate.Invoke(copy))
                {
                    return type;
                }
            }
            return null;
        }

        public void WriteFile(StreamWriter writer)
        {
            Queue<ASMCommand> commands = [];
            commands.Enqueue(new("", [], "Pokémon"));
            foreach (string label in Labels)
            {
                commands.Enqueue(new());
                commands.Enqueue(new($"{label}:"));
                IASMData data = Get(label);
                data.GetSerializer().WriteAssembly(commands, data);
            }

            foreach (ASMCommand command in commands)
            {
                writer.WriteLine(command);
            }
        }
    }
}
