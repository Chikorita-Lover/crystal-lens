using CrystalLens.Models;

namespace CrystalLens.ViewModels
{
    public class EncounterViewModel : ObservableViewModel
    {
        public string Species
        {
            get; set { field = value; OnPropertyChanged(); }
        }
        public byte MinLevel
        {
            get; set { field = value; OnPropertyChanged(); }
        }
        public byte MaxLevel
        {
            get; set { field = value; OnPropertyChanged(); }
        }
        public int Probability { get; }
        public SpriteViewModel Sprite { get; } = new();

        internal EncounterViewModel(Encounter encounter, int probability)
        {
            Species = encounter.Species;
            MinLevel = encounter.MinLevel;
            MaxLevel = encounter.MaxLevel;
            Probability = probability;
        }

        internal Encounter ToModel()
        {
            return new(MinLevel, MaxLevel, Species);
        }
    }
}
