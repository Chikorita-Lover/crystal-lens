using CrystalLens.Models;
using System.Collections.ObjectModel;
using System.IO;

namespace CrystalLens.ViewModels
{
    public class ASMProjectViewModel
    {
        public ASMProject Model;
        public ObservableCollection<ExplorerEntry> Entries { get; } = [];

        public ASMProjectViewModel(ASMProject model)
        {
            Model = model;

            PopulateExplorer();
        }

        private void PopulateExplorer()
        {
            Dictionary<string, FolderEntry> pathToEntry = [];
            foreach (ASMFile.Header header in Model.FileHeaders.Values.Where(file => file.Labels.Count > 0).Reverse())
            {
                FileEntry entry = new(header, Model.Path);

                string[] subdirs = header.RelativePath.Split('\\')[..^1];
                string path = Model.Path;
                ObservableCollection<ExplorerEntry> entries = Entries;
                foreach (string subdir in subdirs)
                {
                    path = Path.Combine(path, subdir);
                    FolderEntry? folder = pathToEntry.GetValueOrDefault(path);
                    if (folder == null)
                    {
                        folder = new FolderEntry(path);
                        pathToEntry.Add(path, folder);
                        entries.Insert(0, folder);
                    }
                    entries = folder.Children;
                }
                entries.Insert(0, entry);

                path = Path.Combine(path, entry.Name);
                foreach (string label in header.Labels)
                {
                    entry.Children.Add(new(header, path, label));
                }
            }
        }

        public abstract class ExplorerEntry
        {
            public string Path { get; }
            public string Name { get; }

            protected ExplorerEntry(string path, string name)
            {
                Path = path;
                Name = name;
            }

            protected ExplorerEntry(string path) : this(path, System.IO.Path.GetFileName(path))
            { }
        }

        public class FolderEntry : ExplorerEntry
        {
            public ObservableCollection<ExplorerEntry> Children { get; } = [];

            public FolderEntry(string path) : base(path)
            { }
        }

        public class FileEntry : ExplorerEntry
        {
            public ASMFile.Header Header { get; }
            public ObservableCollection<LabelEntry> Children { get; } = [];

            public FileEntry(ASMFile.Header header, string projectPath) : base(System.IO.Path.Combine(projectPath, header.RelativePath))
            {
                Header = header;
            }
        }

        public class LabelEntry : ExplorerEntry
        {
            public ASMFile.Header Header { get; }
            public string Label { get; }

            public LabelEntry(ASMFile.Header header, string path, string label) : base(path, label.Length == 0 ? "<top>" : label)
            {
                Header = header;
                Label = label;
            }
        }
    }
}
