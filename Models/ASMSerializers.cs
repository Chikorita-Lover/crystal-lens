namespace CrystalLens.Models
{
    internal static class ASMSerializers
    {
        public static readonly EncounterTableMap.Serializer EncounterTableMap = new();
        public static readonly TimedEncounterTable.Serializer EncounterTable = new();
    }
}
