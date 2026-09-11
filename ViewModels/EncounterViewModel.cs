using CrystalLens.Models;

namespace CrystalLens.ViewModels
{
    public class EncounterViewModel : ObservableViewModel
    {
        public string Name
        {
            get; set { field = value; OnPropertyChanged(); }
        }
        public int Level
        {
            get; set { field = value; OnPropertyChanged(); }
        }
        public int Probability { get; }
        public SpriteViewModel Sprite { get; } = new();

        internal EncounterViewModel(Encounter encounter, int probability)
        {
            Name = encounter.Name;
            Level = encounter.MinLevel;
            Probability = probability;
        }

        internal Encounter ToModel()
        {
            return new(Level, Name);
        }
    }
}
