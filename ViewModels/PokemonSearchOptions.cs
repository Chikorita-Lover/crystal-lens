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
            List<FieldType> fields = [];
            fields.Add(new("Base stat total", stats => stats.BaseStatTotal));
            fields.Add(new("Type", stats => (stats.Type1, stats.Type2)));
            fields.Add(new("Catch rate", stats => stats.CatchRate));
            fields.Add(new("Base EXP yield", stats => stats.BaseExp));
            fields.Add(new("Wild hold items", stats => (stats.Item1, stats.Item2)));
            fields.Add(new("Gender ratio", stats => stats.GenderRatio));
            fields.Add(new("Egg cycles", stats => stats.EggCycles));
            fields.Add(new("Growth rate", stats => stats.GrowthRate));
            fields.Add(new("Egg Groups", stats => (stats.EggGroup1, stats.EggGroup2)));
            return fields;
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

        internal record FieldType(string Name, Func<PokemonStats, object> ValueFunction)
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
