using CrystalLens.Models;

namespace CrystalLens.ViewModels
{
    public class EncounterTableViewModel : ObservableViewModel, IChangeTracking
    {
        private readonly Dictionary<DayTime, EncounterSetViewModel> _encounterSets = [];
        private readonly ChangeTracker tracker = new();
        public EncounterTable EncounterTable
        {
            get; set { field = value; PopulateEncounterSets(); }
        }
        public EncounterSetViewModel EncounterSet
        {
            get; private set { field = value; OnPropertyChanged(); }
        }
        public ICollection<DayTime> Keys => EncounterTable.EncounterSets.Keys;
        public DayTime SelectedKey
        {
            get; set { field = value; EncounterSet = _encounterSets[value]; OnPropertyChanged(); }
        }

        ChangeTracker IChangeTracking.Tracker => tracker;

        internal EncounterTableViewModel(EncounterTable encounterTable)
        {
            EncounterTable = encounterTable;
            SelectedKey = Keys.First();
        }

        private void PopulateEncounterSets()
        {
            _encounterSets.Clear();
            tracker.ClearChildren();
            foreach (KeyValuePair<DayTime, EncounterSet> pair in EncounterTable.EncounterSets)
            {
                EncounterSetViewModel viewModel = new(pair.Value);
                _encounterSets.Add(pair.Key, viewModel);
                tracker.TryAddChildOf(viewModel);
            }
        }
    }
}
