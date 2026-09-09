using CrystalLens.Models;

namespace CrystalLens.ViewModels
{
    public class ASMFileViewModel : ObservableViewModel, IChangeTracking
    {
        public readonly ASMFile File;
        private readonly ChangeTracker tracker = new();
        public string Path
        {
            get;
            set
            {
                field = value;
                File.Path = value;
                Name = System.IO.Path.GetFileName(value);
                OnPropertyChanged();
            }
        }
        public string Name
        {
            get; set { field = value; OnPropertyChanged(); }
        }
        public object Data { get; }
        public bool HasUnsavedChanges
        {
            get; set { field = value; OnPropertyChanged(); }
        }

        ChangeTracker IChangeTracking.Tracker => tracker;

        public ASMFileViewModel(ASMFile file)
        {
            File = file;
            Path = file.Path;
            Data = CreateDataViewModel(file.Get(file.Labels.First()));
            tracker.TryAddChildOf(Data);
            tracker.StatusChanged += Tracker_StatusChanged;
        }

        private void Tracker_StatusChanged(object? sender, EventArgs e)
        {
            HasUnsavedChanges = tracker.HasUnsavedChanges;
        }

        private static object CreateDataViewModel(IASMData data)
        {
            if (data is EncounterTableMap encounterTableMap)
            {
                return new EncounterTableMapViewModel(encounterTableMap);
            }
            if (data is EncounterSetMap encounterSetMap)
            {
                return new EncounterSetMapViewModel(encounterSetMap);
            }
            if (data is EncounterTable encounterTable)
            {
                return new EncounterTableViewModel(encounterTable);
            }
            if (data is EncounterSet encounterSet)
            {
                return new EncounterSetViewModel(encounterSet);
            }
            if (data is PokemonStats pokemonStats)
            {
                return new PokemonStatsViewModel(pokemonStats);
            }
            return null;
        }
    }
}
