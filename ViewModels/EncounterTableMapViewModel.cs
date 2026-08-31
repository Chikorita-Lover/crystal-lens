using CrystalLens.Models;

namespace CrystalLens.ViewModels
{
    public class EncounterTableMapViewModel : ObservableViewModel
    {
        public EncounterTableMap EncounterTables { get; set; }
        public string SelectedMap
        {
            get;
            set
            {
                field = value;
                SelectedEncounters = new(EncounterTables.Get(value));
            }
        }
        public ICollection<string> MapNames
        {
            get => EncounterTables.GetNames();
        }
        public EncounterTableViewModel SelectedEncounters
        {
            get;
            private set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        internal EncounterTableMapViewModel(EncounterTableMap encounterTables)
        {
            EncounterTables = encounterTables;
            SelectedMap = encounterTables.GetNames().First();
        }
    }
}
