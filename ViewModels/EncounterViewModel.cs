using CrystalLens.Models;

namespace CrystalLens.ViewModels
{
    public class EncounterViewModel
    {
        public string Name
        {
            get; set { field = value; UpdateSprite(); }
        }
        public string Level { get; }
        public int Probability { get; }
        public SpriteViewModel Sprite { get; } = new();
        private int minLevel { get; }
        private int maxLevel { get; }

        internal EncounterViewModel(Encounter encounter, int probability)
        {
            Name = encounter.Name;
            minLevel = encounter.MinLevel;
            maxLevel = encounter.MaxLevel;
            Level = minLevel == maxLevel ? minLevel.ToString() : $"{minLevel} – {maxLevel}";
            Probability = probability;
        }

        private void UpdateSprite()
        {
            string spriteName = Name == "UNOWN" ? "unown_a" : Name.ToLower();
            Sprite.Path = $@"C:\Users\cjgar\Git\celebi\gfx\pokemon\{spriteName}\front.png";
        }
    }
}
