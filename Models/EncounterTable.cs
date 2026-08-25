namespace CrystalLens.Models
{
    internal class EncounterTable<K> where K : notnull
    {
        private readonly Dictionary<K, EncounterSet> encounterSets;

        public EncounterTable(Dictionary<K, EncounterSet> encounterSets)
        {
            this.encounterSets = encounterSets;
        }

        public EncounterSet Get(K key)
        {
            return encounterSets[key];
        }
    }
}
