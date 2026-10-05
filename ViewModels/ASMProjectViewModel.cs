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
            foreach (ASMFile.Header header in model.FileHeaders.Where(file => file.Labels.Count > 0))
            {
                FileEntry entry = new(header, Model.Path);
                FileEntries.Add(entry);
            }
        }

        public class FileEntry
        {
            public ASMFile.Header Header { get; }
            public string Path { get; }
            public string Name { get; }
            public string ShortPath { get; }

            public FileEntry(ASMFile.Header header, string projectPath)
            {
                Header = header;
                Path = System.IO.Path.Combine(projectPath, header.RelativePath);
                Name = System.IO.Path.GetFileName(Path);
                ShortPath = System.IO.Path.GetDirectoryName(header.RelativePath) ?? string.Empty;
            }
        }
    }
}
