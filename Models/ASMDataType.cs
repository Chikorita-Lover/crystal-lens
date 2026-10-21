using System.IO;
using System.Text.RegularExpressions;

namespace CrystalLens.Models
{
    public class ASMDataType
    {
        public static readonly List<ASMDataType> Values = [];
        public static readonly ASMDataType EncounterTable = Register(new(
            path => false,
            ASMSerializers.EncounterTable
            ));
        public static readonly ASMDataType EncounterTableMap = Register(new(
            path => path.StartsWith(@"data\wild\") && path.EndsWith("_grass.asm"),
            ASMSerializers.EncounterTableMap
            ));
        public static readonly ASMDataType EncounterSet = Register(new(
            path => false,
            ASMSerializers.EncounterSet
            ));
        public static readonly ASMDataType EncounterSetMap = Register(new(
            path => path.StartsWith(@"data\wild\") && path.EndsWith("_water.asm"),
            ASMSerializers.EncounterSetMap
            ));
        public static readonly ASMDataType DynamicEncounterSet = Register(new(
            path => path.StartsWith(@"data\wild\") && Path.GetFileNameWithoutExtension(path).Contains("contest"),
            ASMSerializers.DynamicEncounterSet
            ));
        public static readonly ASMDataType PokemonStats = Register(new(
            path => path.StartsWith(@"data\pokemon\base_stats\"),
            ASMSerializers.PokemonStats
            ));
        public static readonly ASMDataType EvosAttacks = Register(new(
            path => path == @"data\pokemon\evos_attacks.asm",
            ASMSerializers.EvosAttacks
            ));
        public static readonly ASMDataType SpriteAnimation = Register(new(
            path => path == @"gfx\pokemon\anims.asm" || path == @"gfx\pokemon\idles.asm" || Regex.Match(path, @"gfx\\pokemon\\\w+\\anim\w*.asm").Success,
            ASMSerializers.SpriteAnimation
            ));
        public static readonly ASMDataType PicPointers = Register(new(
            path => path.StartsWith(@"data\pokemon\") && path.EndsWith("pic_pointers.asm"),
            ASMSerializers.PicPointers
            ));
        public static readonly ASMDataType Palettes = Register(new(
            path => path == @"data\pokemon\palettes.asm",
            ASMSerializers.Palettes
            ));
        public static readonly ASMDataType AnimPointers = Register(new(
            path => path == @"gfx\pokemon\anim_pointers.asm" || path == @"gfx\pokemon\idle_pointers.asm",
            ASMSerializers.AnimPointers
            ));
        public static readonly ASMDataType GBCPal = Register(new(
            path => path.EndsWith(".pal"),
            ASMSerializers.GBCPal
            ));
        public static readonly ASMDataType Incbin = Register(new(
            path => path == @"gfx\pics.asm",
            ASMSerializers.Incbin
            ));
        public static readonly Predicate<string> ValidPath = path => Values.Any(type => type.PathPredicate.Invoke(path));

        public readonly Predicate<string> PathPredicate;
        public readonly ASMSerializer Serializer;

        public ASMDataType(Predicate<string> pathPredicate, ASMSerializer serializer)
        {
            PathPredicate = pathPredicate;
            Serializer = serializer;
        }

        private static ASMDataType Register(ASMDataType type)
        {
            Values.Add(type);
            return type;
        }
    }
}
