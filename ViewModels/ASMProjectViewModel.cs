using CrystalLens.Models;
using System.Collections.ObjectModel;

namespace CrystalLens.ViewModels
{
    public class ASMProjectViewModel
    {
        public ASMProject Model;
        public ObservableCollection<FileEntry> FileEntries { get; } = [];

        public ASMProjectViewModel(ASMProject model)
        {
            Model = model;
            foreach (ASMFile file in model.ProjectFiles)
            {
                FileEntry entry = new(file.Path, Model.Path);
                FileEntries.Add(entry);
            }
        }

        public class FileEntry
        {
            public string Path { get; }
            public string Name { get; }
            public string ShortPath { get; }

            public FileEntry(string path, string relativePath)
            {
                Path = path;
                Name = System.IO.Path.GetFileName(path);
                ShortPath = System.IO.Path.GetDirectoryName(System.IO.Path.GetRelativePath(relativePath, path)) ?? string.Empty;
            }
        }
    }
}
