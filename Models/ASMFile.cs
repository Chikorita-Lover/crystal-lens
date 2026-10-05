using System.IO;
using System.Text.RegularExpressions;

namespace CrystalLens.Models
{
    public class ASMFile
    {
        private readonly Dictionary<string, IASMData> _labelsToData = [];
        public ASMProject Project { get; }
        public string Path { get; set; }
        public string RelativePath => System.IO.Path.GetRelativePath(Project.Path, Path);

        public ICollection<string> Labels => _labelsToData.Keys;

        private ASMFile(ASMProject project, string path)
        {
            Project = project;
            Path = path;
        }

        public IASMData Get(string label)
        {
            return _labelsToData[label];
        }

        public static Header ReadFileHeader(string path, ASMProject project)
        {
            List<string> labels = [];

            StreamReader reader = new(path);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (ASMCommand.IsLabel(line))
                {
                    string label = Regex.Match(line, "[A-Za-z_][\\w#$@]+(\\.[A-Za-z_][\\w#$@]+)?").Value;
                    labels.Add(label);
                }
            }
            reader.Close();

            if (labels.Count == 0)
            {
                labels.Add(string.Empty);
            }

            return new(project.GetRelativePath(path), labels);
        }

        public static ASMFile ReadFile(Header header, ASMProject project)
        {
            ASMFile file = new(project, project.GetAbsolutePath(header.RelativePath));

            StreamReader reader = new(file.Path);
            bool isSingleton = header.Labels.Count == 1 && header.Labels[0] == string.Empty;
            if (isSingleton)
            {
                if (TryReadData(reader, file, out IASMData data))
                {
                    file._labelsToData.Add(string.Empty, data);
                }
            }
            else
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (ASMCommand.IsLabel(line))
                    {
                        string label = Regex.Match(line, "[A-Za-z_][\\w#$@]+(\\.[A-Za-z_][\\w#$@]+)?").Value;

                        if (TryReadData(reader, file, out IASMData data))
                        {
                            file._labelsToData.Add(label, data);
                        }
                    }
                }
            }
            reader.Close();

            return file;
        }

        private static bool TryReadData(StreamReader reader, ASMFile file, out IASMData data)
        {
            ASMDataType? type = InferDataType(file);

            if (type != null)
            {
                ASMReader asmReader = new(file.Project, reader);
                long position = reader.BaseStream.Position;
                data = type.Serializer.ReadAssembly(asmReader, file);

                asmReader.CloseChildren();
                reader.BaseStream.Position = position;
                reader.DiscardBufferedData();
                return true;
            }
            else
            {
                data = null;
                return false;
            }
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

        public static bool TryReadFile(ASMProject project, Header header, out ASMFile data)
        {
            try
            {
                data = ReadFile(header, project);
                return true;
            }
            catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException)
            {
                data = new(project, project.GetAbsolutePath(header.RelativePath));
                return false;
            }
        }

        private static ASMDataType? InferDataType(ASMFile file)
        {
            foreach (ASMDataType type in ASMDataType.Values)
            {
                if (type.PathPredicate.Invoke(file.RelativePath))
                {
                    return type;
                }
            }
            return null;
        }

        public void WriteFile(ASMWriter writer)
        {
            foreach (string label in Labels)
            {
                if (!label.IsWhiteSpace())
                {
                    writer.NewLine();
                    writer.Label(label);
                }
                IASMData data = Get(label);
                data.GetSerializer().WriteAssembly(writer, data);
            }
        }

        public record Header(string RelativePath, List<string> Labels);
    }
}
