using CrystalLens.Models;
using System.IO;

namespace CrystalLens.ViewModels
{
    public class ASMFileViewModel : DataTabViewModel
    {
        public readonly ASMFile File;
        public readonly string Label;
        public override string Path
        {
            get;
            set
            {
                field = value;
                Tooltip = value;
                File.Path = value;
                if (Label.Length == 0)
                {
                    Name = System.IO.Path.GetFileName(value);
                }
                OnPropertyChanged();
            }
        }
        public object Data { get; }

        public ASMFileViewModel(ASMFile file, string label)
        {
            File = file;
            Label = label;
            Path = file.Path;
            if (label.Length > 0)
            {
                Name = label;
            }
            Data = CreateDataViewModel(file.Get(label));
            tracker.TryAddChildOf(Data);
            tracker.StatusChanged += Tracker_StatusChanged;
        }

        public ASMFileViewModel(ASMFile file) : this(file, file.Labels.First())
        { }

        public override void Save()
        {
            base.Save();
            using StreamWriter output = new(Path);
            File.WriteFile(new(output));
            output.Close();
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
            if (data is EvolutionMoveSet evosAttacks)
            {
                return new EvolutionMoveSetViewModel(evosAttacks);
            }
            return null;
        }
    }
}
