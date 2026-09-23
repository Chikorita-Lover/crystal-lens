using System.Collections.ObjectModel;
using System.IO;

namespace CrystalLens.Models
{
    public class ASMProject
    {
        public static readonly Dictionary<string, byte> GenderRatioDefinitions = GenerateGenderRatioDefinitions();
        public readonly Dictionary<ASMConstantGroup, Dictionary<string, byte>> Constants = [];
        public string Path { get; }
        public readonly string Name;
        public ObservableCollection<ASMFile> ProjectFiles { get; private set; } = [];

        private ASMProject(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(path);
        }

        public static ASMProject OpenProject(string path)
        {
            ASMProject project = new(path);
            project.ProjectFiles = new(project.LoadFilesFromDirectory(System.IO.Path.Combine(path, "data")));
            project.LoadFilesFromDirectory(System.IO.Path.Combine(path, "constants")); // only for reading constants

            return project;
        }

        private List<ASMFile> LoadFilesFromDirectory(string path)
        {
            List<ASMFile> openFiles = [];

            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                openFiles.AddRange(LoadFilesFromDirectory(directory));
            }

            string[] files = Directory.GetFiles(path, "*.asm");
            foreach (string filePath in files)
            {
                try
                {
                    TryReadConstants(filePath);
                    ASMFile file = ASMFile.ReadFile(filePath, this);
                    openFiles.Add(file);
                }
                catch (FileFormatException)
                { }
                catch (FormatException) // TEMP
                { }
                catch (InvalidOperationException) // TEMP
                { }
            }

            return openFiles;
        }

        private void TryReadConstants(string path)
        {
            List<Dictionary<string, byte>> constants;
            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            switch (fileName)
            {
                case "item_constants":
                    constants = ASMFile.ReadGroupedConstants(path);
                    Constants.Add(ASMConstantGroup.Item, constants[0]);
                    break;
                case "pokemon_constants":
                    constants = ASMFile.ReadGroupedConstants(path);
                    Constants.Add(ASMConstantGroup.Species, constants[0]);
                    break;
                case "pokemon_data_constants":
                    constants = ASMFile.ReadGroupedConstants(path);
                    Constants.Add(ASMConstantGroup.GrowthRate, constants[0]);
                    Constants.Add(ASMConstantGroup.EggGroup, constants[1]);
                    break;
                case "type_constants":
                    constants = ASMFile.ReadGroupedConstants(path);
                    Constants.Add(ASMConstantGroup.Type, constants[0]);
                    break;
            }
        }

        private static Dictionary<string, byte> GenerateGenderRatioDefinitions()
        {
            Dictionary<string, byte> defs = [];
            defs.Add("GENDER_F0", 0);
            defs.Add("GENDER_F12_5", 32);
            defs.Add("GENDER_F25", 63);
            defs.Add("GENDER_F50", 127);
            defs.Add("GENDER_F75", 191);
            defs.Add("GENDER_F100", 254);
            defs.Add("GENDER_UNKNOWN", 255);
            return defs;
        }
    }
}
