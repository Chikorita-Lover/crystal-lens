using CrystalLens.Models;
using System.Collections.ObjectModel;
using System.IO;

namespace CrystalLens.ViewModels
{
    public class PokemonStatsViewModel
    {
        public PokemonStats Model { get; }
        public string Name { get; set; }
        public string Type1 { get; set; }
        public string Type2 { get; set; }
        public byte CatchRate { get; set; }
        public byte BaseExp { get; set; }
        public string Item1 { get; set; }
        public string Item2 { get; set; }
        public string GenderRatio { get; set; }
        public byte EggCycles { get; set; }
        public string GrowthRate { get; set; }
        public string EggGroup1 { get; set; }
        public string EggGroup2 { get; set; }
        public ObservableCollection<StatEntry> BaseStats { get; }
        public int BaseStatTotal { get; private set; }
        public ObservableCollection<string> TMMoves { get; }
        public SpriteViewModel FrontSprite { get; } = new();
        public SpriteViewModel BackSprite { get; } = new();

        public PokemonStatsViewModel(PokemonStats model)
        {
            Model = model;
            Name = model.Name;
            Type1 = model.Type1;
            Type2 = model.Type2;
            CatchRate = model.CatchRate;
            BaseExp = model.BaseExp;
            Item1 = model.Item1;
            Item2 = model.Item2;
            GenderRatio = model.GenderRatio;
            EggCycles = model.EggCycles;
            GrowthRate = model.GrowthRate;
            EggGroup1 = model.EggGroup1;
            EggGroup2 = model.EggGroup2;
            FrontSprite.Path = Path.Combine(@"C:\Users\cjgar\Git\celebi", model.SpritePath.Replace(".dimensions", ".png"));
            FrontSprite.Animation = model.Animation;
            BackSprite.Path = FrontSprite.Path.Replace("front", "back");
            TMMoves = new(model.TMMoves);

            BaseStats = [];
            foreach (PokemonStats.Stat stat in model.BaseStats.Keys)
            {
                StatEntry entry = new(stat, model.BaseStats[stat]);
                BaseStats.Add(entry);
                entry.PropertyChanged += StatEntry_PropertyChanged;
            }
            UpdateBaseStatTotal();
        }

        private void UpdateBaseStatTotal()
        {
            BaseStatTotal = BaseStats.Sum(stat => stat.Value);
        }

        private void StatEntry_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateBaseStatTotal();
        }

        public class StatEntry : ObservableViewModel
        {
            public PokemonStats.Stat Stat { get; }
            public byte Value
            {
                get; set { field = value; OnPropertyChanged(); }
            }

            public StatEntry(PokemonStats.Stat stat, byte value)
            {
                Stat = stat;
                Value = value;
            }
        }
    }
}
