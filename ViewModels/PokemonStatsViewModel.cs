using CrystalLens.Models;
using System.Collections.ObjectModel;
using System.IO;

namespace CrystalLens.ViewModels
{
    public class PokemonStatsViewModel : ObservableViewModel, IChangeTracking
    {
        private readonly ChangeTracker tracker = new();
        public PokemonStats Model { get; }
        public string Species
        {
            get => Model.Species;
            set { Model.Species = value; OnPropertyChanged(); }
        }
        public byte HP
        {
            get => Model.HP;
            set { Model.HP = value; OnPropertyChanged(); OnPropertyChanged(nameof(BaseStatTotal)); }
        }
        public byte Attack
        {
            get => Model.Attack;
            set { Model.Attack = value; OnPropertyChanged(); OnPropertyChanged(nameof(BaseStatTotal)); }
        }
        public byte Defense
        {
            get => Model.Defense;
            set { Model.Defense = value; OnPropertyChanged(); OnPropertyChanged(nameof(BaseStatTotal)); }
        }
        public byte Speed
        {
            get => Model.Speed;
            set { Model.Speed = value; OnPropertyChanged(); OnPropertyChanged(nameof(BaseStatTotal)); }
        }
        public byte SpclAtk
        {
            get => Model.SpclAtk;
            set { Model.SpclAtk = value; OnPropertyChanged(); OnPropertyChanged(nameof(BaseStatTotal)); }
        }
        public byte SpclDef
        {
            get => Model.SpclDef;
            set { Model.SpclDef = value; OnPropertyChanged(); OnPropertyChanged(nameof(BaseStatTotal)); }
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
        public int BaseStatTotal => Model.BaseStatTotal;
        public ObservableCollection<string> TMMoves { get; }
        public string FrontSpritePath { get; }
        public string BackSpritePath { get; }
        public List<(byte R, byte G, byte B)> ShinyColors => Model.ShinyPalette;
        public SpriteAnimation Animation => Model.Animation;
        public bool IsShiny
        {
            get; set { field = value; OnPropertyChanged(); }
        }
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

            ASMProject project = ((IASMData)model).File.Project;
            FrontSpritePath = Path.Combine(project.Path, model.DimensionsPath.Replace(".dimensions", ".png"));
            BackSpritePath = FrontSpritePath.Replace("front", "back");

            PropertyChanged += PokemonStatsViewModel_PropertyChanged;

            ReadConstantsOf(ASMConstantGroup.Species, PokemonConstants);
            ReadConstantsOf(ASMConstantGroup.Type, TypeConstants);
            ReadConstantsOf(ASMConstantGroup.Item, ItemConstants);
            ReadConstantsOf(ASMConstantGroup.GrowthRate, GrowthRateConstants);
            ReadConstantsOf(ASMConstantGroup.EggGroup, EggGroupConstants);
        }

        private void ReadConstantsOf(ASMConstantGroup group, ObservableCollection<string> collection)
        {
            Dictionary<string, byte> constants = IASMData.GetProject(Model).Constants[group];

            foreach (string key in constants.Keys)
            {
                collection.Add(key);
            }
        }

        private void PokemonStatsViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(IsShiny))
            {
                tracker.MarkAsUnsaved();
            }
        }
    }
}
