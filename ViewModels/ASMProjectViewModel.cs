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
            foreach (ASMFile.Header header in model.FileHeaders.Where(file => file.Labels.Count > 0).Reverse())
            {
                FileEntry entry = new(header, Model.Path);

                string[] subdirs = header.RelativePath.Split('\\')[..^1];
                string path = model.Path;
                ObservableCollection<ExplorerEntry> entries = Entries;
                foreach (string subdir in subdirs)
                {
                    path = Path.Combine(path, subdir);
                    FolderEntry? folder = entries.OfType<FolderEntry>().FirstOrDefault(e => e.Path.EndsWith($@"\{subdir}"));
                    if (folder == null)
                    {
                        folder = new FolderEntry(path);
                        entries.Insert(0, folder);
                    }
                    entries = folder.Children;
                }
                entries.Insert(0, entry);
            }
        }

        public abstract class ExplorerEntry
        {
            public string Path { get; }

            protected ExplorerEntry(string path)
            {
                Path = path;
            }
        }

        public class FolderEntry : ExplorerEntry
        {
            public string Name { get; }
            public ObservableCollection<ExplorerEntry> Children { get; } = [];

            public FolderEntry(string path) : base(path)
            {
                Name = System.IO.Path.GetFileName(path);
            }
        }

        public class FileEntry : ExplorerEntry
        {
            public ASMFile.Header Header { get; }
            public string Name { get; }
            public string ShortPath { get; }

            public FileEntry(ASMFile.Header header, string projectPath) : base(System.IO.Path.Combine(projectPath, header.RelativePath))
            {
                Header = header;
                Name = System.IO.Path.GetFileName(Path);
                ShortPath = System.IO.Path.GetDirectoryName(header.RelativePath) ?? string.Empty;
            }
        }
    }
}
