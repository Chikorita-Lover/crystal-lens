namespace CrystalLens.Models
{
    public class EvolutionMoveSet : IASMData
    {
        private readonly ASMFile _file;
        public readonly List<Evolution> Evolutions;
        public readonly List<MoveEntry> Moves;

        public ASMFile File => _file;

        public EvolutionMoveSet(ASMFile file, List<Evolution> evolutions, List<MoveEntry> moves)
        {
            _file = file;
            Evolutions = evolutions;
            Moves = moves;
        }

        public ASMSerializer GetSerializer()
        {
            return ASMSerializers.EvosAttacks;
        }

        public class Evolution
        {
            public string Method;
            public string[] Triggers;
            public string Species;

            public Evolution(string method, string[] triggers, string species)
            {
                Method = method;
                Triggers = triggers;
                Species = species;
            }
        }

        public class MoveEntry
        {
            public byte Level;
            public string Move;

            public MoveEntry(byte level, string move)
            {
                Level = level;
                Move = move;
            }
        }

        public class Serializer : ASMSerializer
        {
            internal override IASMData ReadAssembly(ASMReader reader, ASMFile file)
            {
                List<Evolution> evolutions = [];
                List<MoveEntry> moves = [];

                string method;
                while ((method = reader.Read()) != "0")
                {
                    string[] triggers = new string[method == "EVOLVE_STAT" ? 2 : 1];
                    for (int i = 0; i < triggers.Length; i++)
                    {
                        triggers[i] = reader.Read();
                    }
                    string species = reader.Read();

                    evolutions.Add(new(method, triggers, species));
                }

                byte level;
                while ((level = reader.ReadByte()) != 0)
                {
                    string move = reader.Read();
                    moves.Add(new(level, move));
                }

                return new EvolutionMoveSet(file, evolutions, moves);
            }

            internal override void WriteAssembly(ASMWriter writer, IASMData data)
            {
                throw new NotSupportedException("EvosAttacks data type does not support writing.");

                EvolutionMoveSet evosAttacks = (EvolutionMoveSet)data;

                foreach (Evolution evolution in evosAttacks.Evolutions)
                {
                    object[] bytes = new object[evolution.Triggers.Length + 2];
                    bytes[0] = evolution.Method;
                    Array.ConstrainedCopy(evolution.Triggers, 0, bytes, 1, evolution.Triggers.Length);
                    bytes[^1] = evolution.Species;

                    writer.DeclareBytes(bytes);
                }
                writer.DeclareBytes([0], "no more evolutions");

                foreach (MoveEntry move in evosAttacks.Moves)
                {
                    writer.DeclareBytes([move.Level, move.Move]);
                }
                writer.DeclareBytes([0], "no more level-up moves");
            }
        }
    }
}
