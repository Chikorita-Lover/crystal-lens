using CrystalLens.Models;
using System.Collections.ObjectModel;

namespace CrystalLens.ViewModels
{
    public class EncounterSetViewModel
    {
        public EncounterSet EncounterSet
        {
            get; set { field = value; PopulateEncounters(); }
        }
        public ObservableCollection<EncounterViewModel> Encounters { get; } = [];

        internal EncounterSetViewModel(EncounterSet encounterSet)
        {
            EncounterSet = encounterSet;
        }

        private void PopulateEncounters()
        {
            Encounters.Clear();
            if (EncounterSet != null)
            {
                for (int i = 0; i < EncounterSet.Encounters.Count; i++)
                {
                    EncounterViewModel encounter = new(EncounterSet.Get(i), EncounterSet.GetProbability(i));
                    Encounters.Add(encounter);
                    encounter.PropertyChanged += Encounter_PropertyChanged;
                }
            }
        }

        private void Encounter_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            EncounterViewModel viewModel = (EncounterViewModel)sender;
            int index = Encounters.IndexOf(viewModel);
            EncounterSet.Encounters[index] = viewModel.ToModel();
        }
    }
}
