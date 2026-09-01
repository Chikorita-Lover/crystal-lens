using CrystalLens.Models;

namespace CrystalLens.ViewModels
{
    public class EncounterViewModel : ObservableViewModel
    {
        public string Name
        {
            get; set { field = value; UpdateSprite(); OnPropertyChanged(); }
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

        private void UpdateSprite()
        {
            string spriteName = Name == "UNOWN" ? "unown_a" : Name.ToLower();
            Sprite.Path = $@"C:\Users\cjgar\Git\celebi\gfx\pokemon\{spriteName}\front.png";
        }
    }
}
