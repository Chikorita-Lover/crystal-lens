namespace CrystalLens.Models
{
    public record Encounter(byte MinLevel, byte MaxLevel, string Species)
    {
        public Encounter(byte level, string species) : this(level, level, species)
        { }
    }
}
