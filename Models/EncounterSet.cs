namespace CrystalLens.Models
{
    public class EncounterSet : IASMData
    {
        private readonly ASMFile _file;
        public List<Encounter> Encounters;
        public List<byte> Probabilities;
        public int EncounterRate;

        ASMFile IASMData.File => _file;

        public EncounterSet(ASMFile file, List<Encounter> encounters, List<byte> probabilities, byte encounterRate)
        {
            _file = file;
            Encounters = encounters;
            Probabilities = probabilities;
            EncounterRate = encounterRate;
        }

        public Encounter Get(int index)
        {
            return Encounters[index];
        }

        public byte GetProbability(int index)
        {
            return Probabilities[index];
        }

        public ASMSerializer GetSerializer()
        {
            return ASMSerializers.EncounterSet;
        }

        public class Serializer : ASMSerializer
        {
            internal override IASMData ReadAssembly(ASMReader reader, ASMFile file)
            {
                byte encounterRate;

                string parameter = reader.Read();
                encounterRate = byte.Parse(parameter.Split(" percent")[0]);

                List<byte> probabilities = [60, 30, 10];
                List<Encounter> encounters = [];
                for (byte b = 0; b < probabilities.Count; b++)
                {
                    byte level = reader.ReadByte();
                    string name = reader.Read();
                    encounters.Add(new(level, name));
                }

                return new EncounterSet(file, encounters, probabilities, encounterRate);
            }

            internal override void WriteAssembly(Queue<ASMCommand> commands, IASMData data)
            {
                EncounterSet encounterSet = (EncounterSet)data;

                string rate = PercentFromInt(encounterSet.EncounterRate);
                commands.Enqueue(new("db", [rate], "encounter rate"));

                foreach (Encounter encounter in encounterSet.Encounters)
                {
                    commands.Enqueue(new("db", [encounter.MinLevel.ToString(), encounter.Name]));
                }
            }

            private static string PercentFromInt(int value)
            {
                return $"{value} percent";
            }
        }
    }
}
