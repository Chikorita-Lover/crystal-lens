using System.IO;

namespace CrystalLens.Models
{
    public class PokemonStats : IASMData
    {
        private readonly ASMFile _file;
        public string Name { get; set; }
        public Dictionary<Stat, byte> BaseStats { get; }
        public string Type1 { get; set; }
        public string Type2 { get; set; }
        public byte CatchRate { get; set; }
        public byte BaseExp { get; set; }
        public string Item1 { get; set; }
        public string Item2 { get; set; }
        public string GenderRatio { get; set; }
        public byte EggCycles { get; set; }
        public string SpritePath { get; private set; }
        public string GrowthRate { get; set; }
        public string EggGroup1 { get; set; }
        public string EggGroup2 { get; set; }
        public List<string> TMMoves { get; }
        public int BaseStatTotal => BaseStats.Sum(p => p.Value);
        public SpriteAnimation Animation;

        ASMFile IASMData.File => _file;

        public PokemonStats(ASMFile file, string name, Dictionary<Stat, byte> baseStats, string type1,
            string type2, byte catchRate, byte baseExp, string item1, string item2,
            string genderRatio, byte eggCycles, string spritePath, string growthRate,
            string eggGroup1, string eggGroup2, List<string> tmMoves)
        {
            _file = file;
            Name = name;
            BaseStats = baseStats;
            Type1 = type1;
            Type2 = type2;
            CatchRate = catchRate;
            BaseExp = baseExp;
            Item1 = item1;
            Item2 = item2;
            GenderRatio = genderRatio;
            EggCycles = eggCycles;
            SpritePath = spritePath;
            GrowthRate = growthRate;
            EggGroup1 = eggGroup1;
            EggGroup2 = eggGroup2;
            TMMoves = tmMoves;

            if (ASMFile.TryReadFile(file.Project, Path.Combine(file.Project.Path, Path.GetDirectoryName(spritePath), "anim.asm"), out file))
            {
                Animation = (SpriteAnimation)file.Get(string.Empty);
            }
        }

        public ASMSerializer GetSerializer()
        {
            return ASMSerializers.PokemonStats;
        }

        public enum Stat
        {
            HP,
            Attack,
            Defense,
            Speed,
            SpclAtk,
            SpclDef
        }

        public class Serializer : ASMSerializer
        {
            internal override IASMData ReadAssembly(ASMReader reader, ASMFile file)
            {
                ASMCommand command;
                string name = reader.Read();
                Dictionary<Stat, byte> baseStats = ReadBaseStats(reader);
                string type1 = reader.Read();
                string type2 = reader.Read();
                byte catchRate = reader.ReadByte();
                byte baseExp = reader.ReadByte();
                string item1 = reader.Read();
                string item2 = reader.Read();
                string genderRatio = reader.Read();
                reader.Read(); // unknown 1
                byte eggCycles = reader.ReadByte();
                reader.Read(); // unknown 2
                string spritePath = reader.Read().Replace("\"", ""); // INCBIN
                reader.Read(); // beta front pic
                reader.Read(); // beta back pic
                string growthRate = reader.Read();
                string eggGroup1 = reader.Read(); // \1 (dn)
                string eggGroup2 = reader.Read(); // \2 (dn)
                List<string> tmMoves = ReadTMHMs(reader);

                return new PokemonStats(file, name, baseStats, type1, type2, catchRate, baseExp, item1, item2, genderRatio, eggCycles, spritePath, growthRate, eggGroup1, eggGroup2, tmMoves);
            }

            internal override void WriteAssembly(Queue<ASMCommand> commands, IASMData data)
            {
                PokemonStats pokemon = (PokemonStats)data;
                commands.Enqueue(new("db", [pokemon.Name]));
                commands.Enqueue(new());
                commands.Enqueue(new("db", [.. pokemon.BaseStats.Values.Select(value => value.ToString())]));
                commands.Enqueue(new("", [], "  hp  atk  def  spd  sat  sdf"));
                commands.Enqueue(new());
                commands.Enqueue(new("db", [pokemon.Type1, pokemon.Type2], "type"));
                commands.Enqueue(new("db", [pokemon.CatchRate.ToString()], "catch rate"));
                commands.Enqueue(new("db", [pokemon.BaseExp.ToString()], "base exp"));
                commands.Enqueue(new("db", [pokemon.Item1, pokemon.Item2], "items"));
                commands.Enqueue(new("db", [pokemon.GenderRatio], "gender ratio"));
                commands.Enqueue(new("db", [100.ToString()], "unknown 1"));
                commands.Enqueue(new("db", [pokemon.EggCycles.ToString()], "step cycles to hatch"));
                commands.Enqueue(new("db", [5.ToString()], "unknown 2"));
                commands.Enqueue(new("INCBIN", [$"\"{pokemon.SpritePath}\""]));
                commands.Enqueue(new("dw", ["NULL", "NULL"], "unused (beta front/back pics)"));
                commands.Enqueue(new("db", [pokemon.GrowthRate], "growth rate"));
                commands.Enqueue(new("dn", [pokemon.EggGroup1, pokemon.EggGroup2], "egg groups"));

                commands.Enqueue(new());
                commands.Enqueue(new("", [], "tm/hm learnset"));
                commands.Enqueue(new("tmhm", pokemon.TMMoves.ToArray()));
                commands.Enqueue(new("", [], "end"));
            }

            private static Dictionary<Stat, byte> ReadBaseStats(ASMReader reader)
            {
                Dictionary<Stat, byte> baseStats = [];
                Stat[] stats = Enum.GetValues<Stat>();
                for (int i = 0; i < stats.Length; i++)
                {
                    baseStats.Add(stats[i], reader.ReadByte());
                }
                return baseStats;
            }

            private static List<string> ReadTMHMs(ASMReader reader)
            {
                List<string> tmhms = [];
                string? value;
                while ((value = reader.Read()) != null)
                {
                    tmhms.Add(value);
                }
                return tmhms;
            }
        }
    }
}
