namespace CrystalLens.Models
{
    public class ASMDataType
    {
        public static readonly ASMDataType GrassEncounters = new(
            commands => commands.Peek().Command == "def_grass_wildmons",
            ASMSerializers.EncounterTableMap
            );
        public static readonly ASMDataType WaterEncounters = new(
            commands => commands.Peek().Command == "def_water_wildmons",
            ASMSerializers.EncounterSetMap
            );
        public static readonly ASMDataType PokemonStats = new(
            commands => { ASMCommand command = commands.Peek(); return command.Command == "db" && command.Count == 1; },
            ASMSerializers.PokemonStats
            );
        public static readonly ASMDataType[] Values = [GrassEncounters, WaterEncounters, PokemonStats];

        public readonly Predicate<Queue<ASMCommand>> CommandPredicate;
        public readonly ASMSerializer Serializer;

        public ASMDataType(Predicate<Queue<ASMCommand>> commandPredicate, ASMSerializer serializer)
        {
            CommandPredicate = commandPredicate;
            Serializer = serializer;
        }
    }
}
