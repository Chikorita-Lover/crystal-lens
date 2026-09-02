using CrystalLens.Models;

namespace CrystalLens.ViewModels
{
    public class EncounterSetMapViewModel : ObservableViewModel
    {
        public EncounterSetMap EncounterSets { get; set; }
        public string SelectedMap
        {
            get;
            set
            {
                field = value;
                SelectedEncounters = new(EncounterSets.Get(value));
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

        internal EncounterSetMapViewModel(EncounterSetMap encounterSets)
        {
            EncounterSets = encounterSets;
            SelectedMap = encounterSets.GetNames().First();
        }
    }
}
