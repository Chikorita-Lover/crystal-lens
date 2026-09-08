namespace CrystalLens.Models
{
    internal static class ASMSerializers
    {
        public static readonly EncounterTableMap.Serializer EncounterTableMap = new();
        public static readonly EncounterSetMap.Serializer EncounterSetMap = new();
        public static readonly EncounterTable.Serializer EncounterTable = new();
        public static readonly EncounterSet.Serializer EncounterSet = new();
        public static readonly PokemonStats.Serializer PokemonStats = new();
        public static readonly SpriteAnimation.Serializer SpriteAnimation = new();
    }
}
