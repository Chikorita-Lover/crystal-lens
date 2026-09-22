using CrystalLens.Models;

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
            keys.Add(new("Type", p => p.Stats.Type1 == p.Value || p.Stats.Type2 == p.Value));
            keys.Add(new("Wild hold item", p => p.Stats.Item1 == p.Value || p.Stats.Item2 == p.Value));
            keys.Add(new("Gender ratio", p => p.Stats.GenderRatio == p.Value));
            keys.Add(new("Growth rate", p => p.Stats.GrowthRate == p.Value));
            keys.Add(new("Egg Group", p => p.Stats.EggGroup1 == p.Value || p.Stats.EggGroup2 == p.Value));
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

        internal record Key(string Name, Predicate<(PokemonStats Stats, string Value)> MatchPredicate)
        { }
    }
}
