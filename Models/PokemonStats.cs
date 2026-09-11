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

            if (file.Project != null)
            {
                if (ASMFile.TryReadFile(file.Project, Path.Combine(file.Project.Path, Path.GetDirectoryName(spritePath), "anim.asm"), out file))
                {
                    Animation = (SpriteAnimation)file.Get(string.Empty);
                }
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
            internal override IASMData ReadAssembly(Queue<ASMCommand> commands, ASMFile file)
            {
                ASMCommand command;
                command = commands.Dequeue();
                command.VerifyOrThrow("db");
                string name = command.Get(0);

                command = commands.Dequeue();
                command.VerifyOrThrow("db");
                Dictionary<Stat, byte> baseStats = ReadStatsFromCommand(command);

                command = commands.Dequeue();
                command.VerifyOrThrow("db");
                string type1 = command.Get(0);
                string type2 = command.Get(1);

                command = commands.Dequeue();
                command.VerifyOrThrow("db");
                byte catchRate = byte.Parse(command.Get(0));

                command = commands.Dequeue();
                command.VerifyOrThrow("db");
                byte baseExp = byte.Parse(command.Get(0));

                command = commands.Dequeue();
                command.VerifyOrThrow("db");
                string item1 = command.Get(0);
                string item2 = command.Get(1);

                command = commands.Dequeue();
                command.VerifyOrThrow("db");
                string genderRatio = command.Get(0);

                command = commands.Dequeue();
                command.VerifyOrThrow("db");
                // unknown 1

                command = commands.Dequeue();
                command.VerifyOrThrow("db");
                byte eggCycles = byte.Parse(command.Get(0));

                command = commands.Dequeue();
                command.VerifyOrThrow("db");
                // unknown 2

                command = commands.Dequeue();
                command.VerifyOrThrow("INCBIN");
                string spritePath = command.Get(0).Replace("\"", "");

                command = commands.Dequeue();
                command.VerifyOrThrow("dw");
                // gen 1 pics

                command = commands.Dequeue();
                command.VerifyOrThrow("db");
                string growthRate = command.Get(0);

                command = commands.Dequeue();
                command.VerifyOrThrow("dn");
                string eggGroup1 = command.Get(0);
                string eggGroup2 = command.Get(1);

                command = commands.Dequeue();
                command.VerifyOrThrow("tmhm");
                List<string> tmMoves = ReadTMsFromCommand(command);

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

            private static Dictionary<Stat, byte> ReadStatsFromCommand(ASMCommand command)
            {
                Dictionary<Stat, byte> baseStats = [];
                Stat[] stats = Enum.GetValues<Stat>();
                for (int i = 0; i < stats.Length; i++)
                {
                    baseStats.Add(stats[i], byte.Parse(command.Get(i)));
                }
                return baseStats;
            }

            private static List<string> ReadTMsFromCommand(ASMCommand command)
            {
                List<string> tms = [];
                for (int i = 0; i < command.Count; i++)
                {
                    tms.Add(command.Get(i));
                }
                return tms;
            }
        }
    }
}
