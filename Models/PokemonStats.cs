using System.IO;

namespace CrystalLens.Models
{
    public class PokemonStats : IASMData
    {
        private readonly ASMFile _file;
        public string Species;
        public byte HP;
        public byte Attack;
        public byte Defense;
        public byte Speed;
        public byte SpclAtk;
        public byte SpclDef;
        public string Type1;
        public string Type2;
        public byte CatchRate;
        public byte BaseExp;
        public string Item1;
        public string Item2;
        public string GenderRatio;
        public byte EggCycles;
        public string DimensionsPath;
        public string GrowthRate;
        public string EggGroup1;
        public string EggGroup2;
        public List<string> TMMoves { get; }
        public int BaseStatTotal => HP + Attack + Defense + Speed + SpclAtk + SpclDef;
        public SpriteAnimation Animation;

        ASMFile IASMData.File => _file;

        private PokemonStats(ASMFile file, string species, byte hp, byte attack, byte defense,
            byte speed, byte spclAtk, byte spclDef, string type1, string type2, byte catchRate,
            byte baseExp, string item1, string item2, string genderRatio, byte eggCycles,
            string growthRate, string eggGroup1, string eggGroup2, List<string> tmMoves)
        {
            _file = file;
            Species = species;
            HP = hp;
            Attack = attack;
            Defense = defense;
            Speed = speed;
            SpclAtk = spclAtk;
            SpclDef = spclDef;
            Type1 = type1;
            Type2 = type2;
            CatchRate = catchRate;
            BaseExp = baseExp;
            Item1 = item1;
            Item2 = item2;
            GenderRatio = genderRatio;
            EggCycles = eggCycles;
            GrowthRate = growthRate;
            EggGroup1 = eggGroup1;
            EggGroup2 = eggGroup2;
            TMMoves = tmMoves;

            DimensionsPath = $"gfx/pokemon/{(species == "UNOWN" ? "unown_a" : species.ToLower())}/front.dimensions";
            if (ASMFile.TryReadFile(file.Project, Path.Combine(file.Project.Path, Path.GetDirectoryName(DimensionsPath), "anim.asm"), out file))
            {
                Animation = (SpriteAnimation)file.Get(string.Empty);
            }
        }

        public ASMSerializer GetSerializer()
        {
            return ASMSerializers.PokemonStats;
        }

        public class Builder
        {
            private string _species;
            private byte _hp;
            private byte _attack;
            private byte _defense;
            private byte _speed;
            private byte _spclAtk;
            private byte _spclDef;
            private string _type1;
            private string _type2;
            private byte _catchRate;
            private byte _baseExp;
            private string _item1;
            private string _item2;
            private string _genderRatio;
            private byte _eggCycles;
            private string _growthRate;
            private string _eggGroup1;
            private string _eggGroup2;
            private List<string> _tmMoves;

            public Builder Species(string species)
            {
                _species = species;
                return this;
            }

            public Builder Stats(byte hp, byte attack, byte defense, byte speed, byte spclAtk, byte spclDef)
            {
                _hp = hp;
                _attack = attack;
                _defense = defense;
                _speed = speed;
                _spclAtk = spclAtk;
                _spclDef = spclDef;
                return this;
            }

            public Builder Type(string type1, string type2)
            {
                _type1 = type1;
                _type2 = type2;
                return this;
            }

            public Builder CatchRate(byte catchRate)
            {
                _catchRate = catchRate;
                return this;
            }

            public Builder BaseExp(byte baseExp)
            {
                _baseExp = baseExp;
                return this;
            }

            public Builder Items(string item1, string item2)
            {
                _item1 = item1;
                _item2 = item2;
                return this;
            }

            public Builder GenderRatio(string genderRatio)
            {
                _genderRatio = genderRatio;
                return this;
            }

            public Builder EggCycles(byte eggCycles)
            {
                _eggCycles = eggCycles;
                return this;
            }
            
            public Builder GrowthRate(string growthRate)
            {
                _growthRate = growthRate;
                return this;
            }

            public Builder EggGroups(string eggGroup1, string eggGroup2)
            {
                _eggGroup1 = eggGroup1;
                _eggGroup2 = eggGroup2;
                return this;
            }

            public Builder TMMoves(List<string> tmMoves)
            {
                _tmMoves = tmMoves;
                return this;
            }

            public PokemonStats Build(ASMFile file)
            {
                return new(file, _species, _hp, _attack, _defense, _speed, _spclAtk, _spclDef, _type1, _type2, _catchRate,
                    _baseExp, _item1, _item2, _genderRatio, _eggCycles, _growthRate, _eggGroup1, _eggGroup2, _tmMoves);
            }
        }

        public class Serializer : ASMSerializer
        {
            internal override IASMData ReadAssembly(ASMReader reader, ASMFile file)
            {
                Builder builder = new Builder()
                    .Species(reader.Read())
                    .Stats(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte())
                    .Type(reader.Read(), reader.Read())
                    .CatchRate(reader.ReadByte())
                    .BaseExp(reader.ReadByte())
                    .Items(reader.Read(), reader.Read())
                    .GenderRatio(reader.Read());
                reader.Read(); // unknown 1
                builder.EggCycles(reader.ReadByte());
                reader.Read(); // unknown 2
                reader.Read(); // dimensions
                reader.Read(); // beta front pic
                reader.Read(); // beta back pic
                builder
                    .GrowthRate(reader.Read())
                    .EggGroups(reader.Read(), reader.Read())
                    .TMMoves(ReadTMHMs(reader));

                return builder.Build(file);
            }

            internal override void WriteAssembly(ASMWriter writer, IASMData data)
            {
                PokemonStats pokemon = (PokemonStats)data;
                writer.DeclareBytes([pokemon.Species]);
                writer.NewLine();
                writer.DeclareBytes([pokemon.HP, pokemon.Attack, pokemon.Defense, pokemon.Speed, pokemon.SpclAtk, pokemon.SpclDef]);
                writer.Comment("  hp  atk  def  spd  sat  sdf");
                writer.NewLine();
                writer.DeclareBytes([pokemon.Type1, pokemon.Type2], "type");
                writer.DeclareBytes([pokemon.CatchRate], "catch rate");
                writer.DeclareBytes([pokemon.BaseExp], "base exp");
                writer.DeclareBytes([pokemon.Item1, pokemon.Item2], "items");
                writer.DeclareBytes([pokemon.GenderRatio], "gender ratio");
                writer.DeclareBytes([100], "unknown 1");
                writer.DeclareBytes([pokemon.EggCycles], "step cycles to hatch");
                writer.DeclareBytes([5], "unknown 2");
                writer.WriteCommand(new("INCBIN", [$"\"{pokemon.DimensionsPath}\""]));
                writer.DeclareWords(["NULL", "NULL"], "unused (beta front/back pics)");
                writer.DeclareBytes([pokemon.GrowthRate], "growth rate");
                writer.DeclareNybbles([pokemon.EggGroup1, pokemon.EggGroup2], "egg groups");

                writer.NewLine();
                writer.Comment("tm/hm learnset");
                writer.WriteCommand(new("tmhm", [.. pokemon.TMMoves]));
                writer.Comment("end");
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
