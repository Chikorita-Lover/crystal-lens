using System.Collections.ObjectModel;
using System.IO;

namespace CrystalLens.Models
{
    public class ASMProject
    {
        public string Path { get; }
        public ObservableCollection<ASMFile> ProjectFiles { get; private set; } = [];
        
        private ASMProject(string path)
        {
            Path = path;
        }

        public static ASMProject OpenProject(string path)
        {
            ASMProject project = new(path)
            {
                ProjectFiles = new(GetFilesInDirectory(System.IO.Path.Combine(path, "data")))
            };

            return project;
        }

        private static List<ASMFile> GetFilesInDirectory(string path)
        {
            List<ASMFile> openFiles = [];

            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                openFiles.AddRange(GetFilesInDirectory(directory));
            }

            string[] files = Directory.GetFiles(path, "*.asm");
            foreach (string filePath in files)
            {
                try
                {
                    ASMFile file = ASMFile.ReadFile(filePath);
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
    }
}
