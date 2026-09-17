using System.IO;

namespace CrystalLens.Models
{
    public class ASMFile
    {
        public ASMProject? Project { get; }
        public string Path { get; set; }
        public string? RelativePath => Project != null ? System.IO.Path.GetRelativePath(Project.Path, Path) : null;
        private readonly Dictionary<string, IASMData> labeledData = [];

        public ICollection<string> Labels => labeledData.Keys;

        private ASMFile(ASMProject? project, string path)
        {
            Project = project;
            Path = path;
        }

        public IASMData Get(string label)
        {
            return labeledData[label];
        }

        public static ASMFile ReadFile(string path, ASMProject? project)
        {
            ASMFile file = new(project, path);

            StreamReader reader = new(path);
            string? line;
            Queue<ASMCommand> commands = [];
            Dictionary<string, Queue<ASMCommand>> labeledCommands = [];
            labeledCommands.Add(string.Empty, commands);
            while ((line = reader.ReadLine()) != null)
            {
                ASMCommand command = ASMCommand.FromLine(line);
                if (command.Command.EndsWith(':'))
                {
                    if (!command.Command.Contains('.')) // TEMP
                    {
                        string label = command.Command.Split(':')[0];
                        commands = [];
                        labeledCommands.Add(label, commands);
                    }
                }
                else if (!command.Command.IsWhiteSpace())
                {
                    commands.Enqueue(command);
                }
            }

            foreach (string label in labeledCommands.Keys)
            {
                commands = labeledCommands[label];
                if (commands.Count == 0)
                {
                    continue;
                }

                ASMDataType type = InferDataType(file)
                    ?? throw new FileFormatException($"Cannot infer the data type of the provided file.");
                IASMData data = type.Serializer.ReadAssembly(commands, file);
                file.labeledData.Add(label, data);
            }

            reader.Close();
            return file;
        }

        public static List<Dictionary<string, byte>> ReadGroupedConstants(string path)
        {
            List<Dictionary<string, byte>> constants = [];
            Dictionary<string, byte> constantGroup = [];
            constants.Add(constantGroup);
            byte constValue = 0;

            StreamReader reader = new(path);
            string? line;
            ASMCommand command;
            byte previousValue;
            bool inMacroDef = false;
            while ((line = reader.ReadLine()) != null)
            {
                command = ASMCommand.FromLine(line);
                previousValue = constValue;
                switch (command.Command)
                {
                    case "const" when !inMacroDef:
                        constantGroup.Add(command.Get(0), constValue++);
                        break;
                    case "const_def" when !inMacroDef:
                        constValue = command.Count == 0 ? (byte)0 : command.GetByte(0);
                        if (constValue < previousValue)
                        {
                            constantGroup = [];
                            constants.Add(constantGroup);
                        }
                        break;
                    case "const_next" when !inMacroDef:
                        constValue += command.GetByte(0);
                        break;
                    case "const_skip" when !inMacroDef:
                        constValue++;
                        break;

                    case "MACRO": // TEMP
                        inMacroDef = true;
                        break;
                    case "ENDM": // TEMP
                        inMacroDef = false;
                        break;
                    case "add_tm": // TEMP
                        constantGroup.Add($"TM_{command.Get(0)}", constValue++);
                        break;
                    case "add_hm": // TEMP
                        constantGroup.Add($"HM_{command.Get(0)}", constValue++);
                        break;
                }
            }

            reader.Close();
            return constants;
        }

        public static bool TryReadFile(ASMProject? project, string path, out ASMFile data)
        {
            try
            {
                data = ReadFile(path, project);
                return true;
            }
            catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException)
            {
                data = new(project, path);
                return false;
            }
        }

        private static ASMDataType? InferDataType(ASMFile file)
        {
            if (file.Project != null)
            {
                foreach (ASMDataType type in ASMDataType.Values)
                {
                    if (type.PathPredicate.Invoke(file.RelativePath))
                    {
                        return type;
                    }
                }
            }
            return null;
        }

        public void WriteFile(StreamWriter writer)
        {
            Queue<ASMCommand> commands = [];
            foreach (string label in Labels)
            {
                if (!label.IsWhiteSpace())
                {
                    commands.Enqueue(new());
                    commands.Enqueue(new($"{label}:"));
                }
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
