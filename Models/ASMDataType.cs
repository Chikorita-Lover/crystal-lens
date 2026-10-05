using System.IO;

namespace CrystalLens.Models
{
    public class ASMDataType
    {
        public static readonly ASMDataType GrassEncounters = new(
            path => path.StartsWith(@"data\wild\") && path.EndsWith("_grass.asm"),
            ASMSerializers.EncounterTableMap
            );
        public static readonly ASMDataType WaterEncounters = new(
            path => path.StartsWith(@"data\wild\") && path.EndsWith("_water.asm"),
            ASMSerializers.EncounterSetMap
            );
        public static readonly ASMDataType ContestEncounters = new(
            path => path.StartsWith(@"data\wild\") && Path.GetFileNameWithoutExtension(path).Contains("contest"),
            ASMSerializers.DynamicEncounterSet
            );
        public static readonly ASMDataType PokemonStats = new(
            path => path.StartsWith(@"data\pokemon\base_stats\"),
            ASMSerializers.PokemonStats
            );
        public static readonly ASMDataType SpriteAnimation = new(
            path => path.StartsWith(@"gfx\pokemon\"),
            ASMSerializers.SpriteAnimation
            );
        public static readonly ASMDataType[] Values = [GrassEncounters, WaterEncounters, ContestEncounters, PokemonStats, SpriteAnimation];
        public static readonly Predicate<string> ValidPath = path => Values.Any(type => type.PathPredicate.Invoke(path));

        public readonly Predicate<string> PathPredicate;
        public readonly ASMSerializer Serializer;

        public ASMDataType(Predicate<string> pathPredicate, ASMSerializer serializer)
        {
            PathPredicate = pathPredicate;
            Serializer = serializer;
        }
    }
}
