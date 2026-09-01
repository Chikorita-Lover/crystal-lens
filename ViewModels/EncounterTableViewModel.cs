using CrystalLens.Models;
using System.Collections.ObjectModel;

namespace CrystalLens.ViewModels
{
    public class EncounterTableViewModel : ObservableViewModel
    {
        public TimedEncounterTable EncounterTable
        {
            get; set { field = value; PopulateEncounterSets(); }
        }
        public EncounterSetViewModel EncounterSet => EncounterSets[1];
        public ObservableCollection<EncounterSetViewModel> EncounterSets { get; } = [];

        internal EncounterTableViewModel(TimedEncounterTable encounterTable)
        {
            EncounterTable = encounterTable;
        }

        private void PopulateEncounterSets()
        {
            EncounterSets.Clear();
            foreach (EncounterSet encounterSet in EncounterTable.EncounterSets.Values)
            {
                EncounterSets.Add(new(encounterSet));
            }
        }
    }
}
