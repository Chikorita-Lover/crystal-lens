namespace CrystalLens.Models
{
    public record Encounter(int MinLevel, int MaxLevel, string Name)
    {
        public string LevelDisplay
        {
            get => MinLevel == MaxLevel
                ? MinLevel.ToString()
                : MinLevel.ToString() + " – " + MaxLevel.ToString();
        }

        public Encounter(int level, string name) : this(level, level, name)
        { }
    }
}
