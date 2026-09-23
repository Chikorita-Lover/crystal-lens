using CrystalLens.Models;
using System.Collections.ObjectModel;

namespace CrystalLens.ViewModels
{
    internal class PokemonSearchOptions : ObservableViewModel
    {
        private readonly ASMProjectViewModel _project;
        public static List<FieldType> FieldTypes { get; } = GenerateFieldTypes();
        public static List<Key> FilterKeys { get; } = GenerateFilterKeys();
        public List<Field> Fields { get; } = [];
        public bool IsFiltered
        {
            get; set { field = value; OnPropertyChanged(); }
        }
        public Key FilterKey
        {
            get;
            set
            {
                field = value;
                FilterValues.Clear();
                foreach (object o in FilterKey.ValueGetter.Invoke(_project))
                {
                    FilterValues.Add(o.ToString() ?? string.Empty);
                }
                FilterValue = FilterValues.First();
                OnPropertyChanged();
            }
        }
        public string FilterValue
        {
            get; set { field = value; OnPropertyChanged(); }
        }
        public ObservableCollection<string> FilterValues { get; } = [];

        internal PokemonSearchOptions(ASMProjectViewModel project)
        {
            _project = project;
            FilterKey = FilterKeys.First();
            foreach (FieldType type in FieldTypes)
            {
                Fields.Add(new(type));
            }
        }

        private static List<FieldType> GenerateFieldTypes()
        {
            List<FieldType> types = [];
            types.Add(new("Base stat total", [nameof(PokemonStatsViewModel.BaseStatTotal)]));
            types.Add(new("Type", [nameof(PokemonStatsViewModel.Type1), nameof(PokemonStatsViewModel.Type2)]));
            types.Add(new("Catch rate", [nameof(PokemonStatsViewModel.CatchRate)]));
            types.Add(new("Base EXP yield", [nameof(PokemonStatsViewModel.BaseExp)]));
            types.Add(new("Wild hold items", [nameof(PokemonStatsViewModel.Item1), nameof(PokemonStatsViewModel.Item2)]));
            types.Add(new("Gender ratio", [nameof(PokemonStatsViewModel.GenderRatio)]));
            types.Add(new("Egg cycles", [nameof(PokemonStatsViewModel.EggCycles)]));
            types.Add(new("Growth rate", [nameof(PokemonStatsViewModel.GrowthRate)]));
            types.Add(new("Egg Groups", [nameof(PokemonStatsViewModel.EggGroup1), nameof(PokemonStatsViewModel.EggGroup2)]));
            return types;
        }

        private static List<Key> GenerateFilterKeys()
        {
            List<Key> keys = [];
            keys.Add(new("Type", vm => [vm.Type1, vm.Type2], vm => vm.Model.Constants[ASMConstantGroup.Type].Keys));
            keys.Add(new("Wild hold item", vm => [vm.Item1, vm.Item2], vm => vm.Model.Constants[ASMConstantGroup.Item].Keys));
            keys.Add(new("Gender ratio", vm => [vm.GenderRatio], vm => ASMProject.GenderRatioDefinitions.Keys));
            keys.Add(new("Growth rate", vm => [vm.GrowthRate], vm => vm.Model.Constants[ASMConstantGroup.GrowthRate].Keys));
            keys.Add(new("Egg Group", vm => [vm.EggGroup1, vm.EggGroup2], vm => vm.Model.Constants[ASMConstantGroup.EggGroup].Keys));
            return keys;
        }

        internal record FieldType(string Name, string[] PropertyNames)
        { }

        internal class Field
        {
            public FieldType Type { get; }
            public bool IsShown { get; set; }

            internal Field(FieldType type)
            {
                Type = type;
            }
        }

        internal record Key(string Name, Func<PokemonStatsViewModel, object[]> Selector, Func<ASMProjectViewModel, IEnumerable<object>> ValueGetter);
    }
}
