using CrystalLens.Models;

namespace CrystalLens.ViewModels
{
    public class ASMFileViewModel : ObservableViewModel
    {
        public readonly ASMFile File;
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

        public ASMFileViewModel(ASMFile file)
        {
            File = file;
            Path = file.Path;
            Data = CreateDataViewModel(file.Get(file.Labels.First()));
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
