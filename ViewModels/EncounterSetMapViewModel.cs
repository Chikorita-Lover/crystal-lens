using CrystalLens.Models;

namespace CrystalLens.ViewModels
{
    public class EncounterSetMapViewModel : ObservableViewModel, IChangeTracking
    {
        private readonly ChangeTracker tracker = new();
        public EncounterSetMap EncounterSets { get; set; }
        public string SelectedMap
        {
            get;
            set
            {
                field = value;
                SelectedEncounters = new(EncounterSets.Get(value));
                tracker.ClearChildren();
                tracker.TryAddChildOf(SelectedEncounters);
            }
        }
        public ICollection<string> MapNames
        {
            get => EncounterSets.GetNames();
        }
        public EncounterSetViewModel SelectedEncounters
        {
            get;
            private set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        ChangeTracker IChangeTracking.Tracker => tracker;

        internal EncounterSetMapViewModel(EncounterSetMap encounterSets)
        {
            EncounterSets = encounterSets;
            SelectedMap = encounterSets.GetNames().First();
        }
    }
}
