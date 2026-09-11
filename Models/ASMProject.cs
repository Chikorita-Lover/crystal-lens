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
            ASMProject project = new(path);
            project.ProjectFiles = new(project.LoadFilesFromDirectory(System.IO.Path.Combine(path, "data")));

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
    }
}
