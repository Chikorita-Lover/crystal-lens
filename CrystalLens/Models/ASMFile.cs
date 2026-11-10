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
                if (ASMCommand.HasLabel(line))
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
            ASMByteReader reader = new(project, new StreamReader(file.Path));

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
                foreach (string label in header.Labels)
                {
                    reader.JumpTo(label);

                    if (TryReadData(reader, file, out IASMData data))
                    {
                        file._labelsToData.Add(label, data);
                    }
                }
            }
            reader.Close();

            return file;
        }

        private static bool TryReadData(ASMByteReader reader, ASMFile file, out IASMData data)
        {
            ASMDataType? type = InferDataType(file);

            if (type != null)
            {
                data = type.Serializer.ReadAssembly(reader, file);
                return true;
            }
            else
            {
                data = null;
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
                data.Type.Serializer.WriteAssembly(writer, data);
            }
        }

        public record Header(string RelativePath, List<string> Labels);
    }
}
