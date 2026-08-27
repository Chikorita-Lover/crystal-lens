using CrystalLens.Models;

namespace CrystalLens.ViewModels
{
    public class ASMFileViewModel
    {
        public string Name
        {
            get => file.Name;
        }
        public string Path
        {
            get => file.Path;
        }
        public EncounterTableMap EncounterTables { get; }
        private readonly ASMFile file;

        public ASMFileViewModel(ASMFile file, EncounterTableMap encounterTables)
        {
            this.file = file;
            EncounterTables = encounterTables;
        }
    }
}
