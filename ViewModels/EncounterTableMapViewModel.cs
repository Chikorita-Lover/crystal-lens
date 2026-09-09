using CrystalLens.Models;

namespace CrystalLens.ViewModels
{
    public class EncounterTableMapViewModel : ObservableViewModel, IChangeTracking
    {
        private readonly ChangeTracker tracker = new();
        public EncounterTableMap EncounterTables { get; set; }
        public string SelectedMap
        {
            get;
            set
            {
                field = value;
                SelectedEncounters = new(EncounterTables.Get(value));
                tracker.ClearChildren();
                tracker.TryAddChildOf(SelectedEncounters);
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

        ChangeTracker IChangeTracking.Tracker => tracker;

        internal EncounterTableMapViewModel(EncounterTableMap encounterTables)
        {
            EncounterTables = encounterTables;
            SelectedMap = encounterTables.GetNames().First();
        }
    }
}
