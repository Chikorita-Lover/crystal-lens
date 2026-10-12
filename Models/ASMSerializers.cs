namespace CrystalLens.Models
{
    internal static class ASMSerializers
    {
        public static readonly EncounterTableMap.Serializer EncounterTableMap = new();
        public static readonly EncounterSetMap.Serializer EncounterSetMap = new();
        public static readonly EncounterTable.Serializer EncounterTable = new();
        public static readonly EncounterSet.Serializer EncounterSet = new(false);
        public static readonly EncounterSet.Serializer DynamicEncounterSet = new(true);
        public static readonly EvolutionMoveSet.Serializer EvosAttacks = new();
        public static readonly PokemonStats.Serializer PokemonStats = new();
        public static readonly SpriteAnimation.Serializer SpriteAnimation = new();
        public static readonly PicPointerTable.Serializer PicPointers = new();
        public static readonly BinaryFileReference.Serializer Incbin = new();
    }
}
