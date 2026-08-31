using CrystalLens.Models;

namespace CrystalLens.ViewModels
{
    public class ASMFileViewModel
    {
        public readonly ASMFile File;
        public string Name
        {
            get => File.Name;
        }
        public string Path
        {
            get => File.Path;
        }
        public EncounterTableMapViewModel EncounterTables { get; }

        public ASMFileViewModel(ASMFile file)
        {
            File = file;
            EncounterTables = new(file.Get(file.Labels.First()));
        }
    }
}
