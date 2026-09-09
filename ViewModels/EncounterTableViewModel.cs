using CrystalLens.Models;
using System.Collections.ObjectModel;

namespace CrystalLens.ViewModels
{
    public class EncounterTableViewModel : IChangeTracking
    {
        private readonly ChangeTracker tracker = new();
        public EncounterTable EncounterTable
        {
            get; set { field = value; PopulateEncounterSets(); }
        }
        public EncounterSetViewModel EncounterSet => EncounterSets[1];
        public ObservableCollection<EncounterSetViewModel> EncounterSets { get; } = [];

        ChangeTracker IChangeTracking.Tracker => tracker;

        internal EncounterTableViewModel(EncounterTable encounterTable)
        {
            EncounterTable = encounterTable;
        }

        private void PopulateEncounterSets()
        {
            EncounterSets.Clear();
            tracker.ClearChildren();
            foreach (EncounterSet encounterSet in EncounterTable.EncounterSets.Values)
            {
                EncounterSetViewModel viewModel = new(encounterSet);
                EncounterSets.Add(viewModel);
                tracker.TryAddChildOf(viewModel);
            }
        }
    }
}
