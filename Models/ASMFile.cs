using System.IO;

namespace CrystalLens.Models
{
    public class ASMFile
    {
        public readonly string Path;
        public readonly string Name;
        private readonly Dictionary<string, EncounterTableMap> labeledData = [];

        public ICollection<string> Labels => labeledData.Keys;

        private ASMFile(string path)
        {
            Path = path;
            Name = path.Split('\\').Last();
        }

        public EncounterTableMap Get(string label)
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
                EncounterTableMap data = ASMSerializers.EncounterTableMap.ReadAssembly(commands);
                file.labeledData.Add(label, data);
            }

            reader.Close();
            return file;
        }

        public void WriteFile(StreamWriter writer)
        {
            Queue<ASMCommand> commands = [];
            commands.Enqueue(new("", [], "Pokémon in grass"));
            foreach (string label in Labels)
            {
                commands.Enqueue(new());
                commands.Enqueue(new($"{label}:"));
                ASMSerializers.EncounterTableMap.WriteAssembly(commands, Get(label));
            }
            
            foreach (ASMCommand command in commands)
            {
                writer.WriteLine(command);
            }
        }
    }
}
