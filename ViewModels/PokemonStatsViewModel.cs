using CrystalLens.Models;
using System.Collections.ObjectModel;
using System.IO;

namespace CrystalLens.ViewModels
{
    public class PokemonStatsViewModel : ObservableViewModel, IChangeTracking
    {
        private readonly ChangeTracker tracker = new();
        public PokemonStats Model { get; }
        public string Name
        {
            get => Model.Name;
            set { Model.Name = value; OnPropertyChanged(); }
        }
        public string Type1
        {
            get => Model.Type1;
            set { Model.Type1 = value; OnPropertyChanged(); }
        }
        public string Type2
        {
            get => Model.Type2;
            set { Model.Type2 = value; OnPropertyChanged(); }
        }
        public byte CatchRate
        {
            get => Model.CatchRate;
            set { Model.CatchRate = value; OnPropertyChanged(); }
        }
        public byte BaseExp
        {
            get => Model.BaseExp;
            set { Model.BaseExp = value; OnPropertyChanged(); }
        }
        public string Item1
        {
            get => Model.Item1;
            set { Model.Item1 = value; OnPropertyChanged(); }
        }
        public string Item2
        {
            get => Model.Item2;
            set { Model.Item2 = value; OnPropertyChanged(); }
        }
        public string GenderRatio
        {
            get => Model.GenderRatio;
            set { Model.GenderRatio = value; OnPropertyChanged(); }
        }
        public byte EggCycles
        {
            get => Model.EggCycles;
            set { Model.EggCycles = value; OnPropertyChanged(); }
        }
        public string GrowthRate
        {
            get => Model.GrowthRate;
            set { Model.GrowthRate = value; OnPropertyChanged(); }
        }
        public string EggGroup1
        {
            get => Model.EggGroup1;
            set { Model.EggGroup1 = value; OnPropertyChanged(); }
        }
        public string EggGroup2
        {
            get => Model.EggGroup2;
            set { Model.EggGroup2 = value; OnPropertyChanged(); }
        }
        public ObservableCollection<StatEntry> BaseStats { get; }
        public int BaseStatTotal => BaseStats.Sum(stat => stat.Value);
        public ObservableCollection<string> TMMoves { get; }
        public SpriteViewModel FrontSprite { get; } = new();
        public SpriteViewModel BackSprite { get; } = new();
        public ObservableCollection<string> PokemonConstants { get; } = [];
        public ObservableCollection<string> TypeConstants { get; } = [];
        public ObservableCollection<string> ItemConstants { get; } = [];
        public ObservableCollection<string> GrowthRateConstants { get; } = [];
        public ObservableCollection<string> EggGroupConstants { get; } = [];

        ChangeTracker IChangeTracking.Tracker => tracker;

        public PokemonStatsViewModel(PokemonStats model)
        {
            Model = model;
            TMMoves = new(model.TMMoves);

            ASMProject? project = ((IASMData)model).File.Project;
            if (project != null)
            {
                FrontSprite.Path = Path.Combine(project.Path, model.SpritePath.Replace(".dimensions", ".png"));
                FrontSprite.Animation = model.Animation;
                FrontSprite.PlayAnimationOnLoad = true;
                BackSprite.Path = FrontSprite.Path.Replace("front", "back");
            }

            PropertyChanged += PokemonStatsViewModel_PropertyChanged;

            BaseStats = [];
            foreach (PokemonStats.Stat stat in model.BaseStats.Keys)
            {
                StatEntry entry = new(stat, model.BaseStats[stat]);
                BaseStats.Add(entry);
                entry.PropertyChanged += StatEntry_PropertyChanged;
            }

            ReadConstantsOf(ASMConstantGroup.Species, PokemonConstants);
            ReadConstantsOf(ASMConstantGroup.Type, TypeConstants);
            ReadConstantsOf(ASMConstantGroup.Item, ItemConstants);
            ReadConstantsOf(ASMConstantGroup.GrowthRate, GrowthRateConstants);
            ReadConstantsOf(ASMConstantGroup.EggGroup, EggGroupConstants);
        }

        private void ReadConstantsOf(ASMConstantGroup group, ObservableCollection<string> collection)
        {
            if (IASMData.HasProject(Model)) {
                Dictionary<string, byte> constants = IASMData.GetProject(Model).Constants[group];

                foreach (string key in constants.Keys)
                {
                    collection.Add(key);
                }
            }
        }

        private void PokemonStatsViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            tracker.MarkAsUnsaved();
        }

        private void StatEntry_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            StatEntry entry = (StatEntry)sender;
            Model.BaseStats[entry.Stat] = entry.Value;
            OnPropertyChanged(nameof(BaseStatTotal));
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
