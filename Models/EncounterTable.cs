namespace CrystalLens.Models
{
    public abstract record EncounterTable<K>(Dictionary<K, EncounterSet> EncounterSets) where K : notnull
    {
        public EncounterSet Get(K key)
        {
            return EncounterSets[key];
        }

        public void Add(K key, EncounterSet encounters)
        {
            EncounterSets[key] = encounters;
        }
    }
}
