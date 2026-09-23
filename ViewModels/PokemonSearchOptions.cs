namespace CrystalLens.ViewModels
{
    internal class PokemonSearchOptions : ObservableViewModel
    {
        public static List<FieldType> FieldTypes { get; } = GenerateFieldTypes();
        public static List<Key> FilterKeys { get; } = GenerateFilterKeys();
        public List<Field> Fields { get; } = [];
        public bool IsFiltered
        {
            get; set { field = value; OnPropertyChanged(); }
        }
        public Key FilterKey
        {
            get; set { field = value; OnPropertyChanged(); }
        }
        public string FilterValue
        {
            get; set { field = value; OnPropertyChanged(); }
        }

        internal PokemonSearchOptions()
        {
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
            keys.Add(new("Type", vm => [vm.Type1, vm.Type2]));
            keys.Add(new("Wild hold item", vm => [vm.Item1, vm.Item2]));
            keys.Add(new("Gender ratio", vm => [vm.GenderRatio]));
            keys.Add(new("Growth rate", vm => [vm.GrowthRate]));
            keys.Add(new("Egg Group", vm => [vm.EggGroup1, vm.EggGroup2]));
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

        internal record Key(string Name, Func<PokemonStatsViewModel, object[]> ValueFunction)
        { }
    }
}
