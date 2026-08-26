namespace CrystalLens.Models
{
    public record EncounterSet(List<Encounter> Encounters, List<int> Probabilities, int EncounterRate)
    {
        public Encounter Get(int index)
        {
            return Encounters[index];
        }

        public int GetProbability(int index)
        {
            return Probabilities[index];
        }
    }
}
