using CrystalLens.Models;
using System.Collections.ObjectModel;

namespace CrystalLens.ViewModels
{
    public class EncounterTableViewModel
    {
        public TimedEncounterTable EncounterTable
        {
            get;
            set
            {
                field = value;
                PopulateEncounters();
            }
        }
        public ObservableCollection<EncounterViewModel> Encounters { get; } = [];

        internal EncounterTableViewModel(TimedEncounterTable encounterTable)
        {
            EncounterTable = encounterTable;
        }

        private void PopulateEncounters()
        {
            Encounters.Clear();
            if (EncounterTable != null)
            {
                EncounterSet encounterSet = EncounterTable.Get(DayTime.Day);
                for (int i = 0; i < encounterSet.Encounters.Count; i++)
                {
                    EncounterViewModel encounter = new(encounterSet.Get(i), encounterSet.GetProbability(i));
                    Encounters.Add(encounter);
                    encounter.PropertyChanged += Encounter_PropertyChanged;
                }
            }
        }

        private void Encounter_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            EncounterViewModel viewModel = (EncounterViewModel)sender;
            int index = Encounters.IndexOf(viewModel);
            EncounterTable.EncounterSets[DayTime.Day].Encounters[index] = viewModel.ToModel();
        }
    }
}
