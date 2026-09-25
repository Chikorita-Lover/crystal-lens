namespace CrystalLens.Models
{
    public class EncounterTable : IASMData
    {
        private readonly ASMFile _file;
        public Dictionary<DayTime, EncounterSet> EncounterSets;

        ASMFile IASMData.File => _file;

        public EncounterTable(ASMFile file, Dictionary<DayTime, EncounterSet> encounterSets)
        {
            _file = file;
            EncounterSets = encounterSets;
        }

        public EncounterSet Get(DayTime time)
        {
            return EncounterSets[time];
        }

        public void Add(DayTime time, EncounterSet encounters)
        {
            EncounterSets[time] = encounters;
        }

        public ASMSerializer GetSerializer()
        {
            return ASMSerializers.EncounterTable;
        }

        public class Serializer : ASMSerializer
        {
            internal override IASMData ReadAssembly(ASMReader reader, ASMFile file)
            {
                List<byte> probabilities = [30, 30, 20, 10, 5, 4, 1];
                Dictionary<DayTime, EncounterSet> encounterSets = [];
                for (int i = 0; i < 3; i++)
                {
                    List<Encounter> encounters = [];
                    foreach (byte probability in probabilities)
                    {
                        byte level = reader.ReadByte();
                        string name = reader.Read();
                        encounters.Add(new(level, name));
                    }
                    DayTime time = Enum.GetValues<DayTime>()[i];
                    encounterSets[time] = new(file, encounters, probabilities);
                }

                return new EncounterTable(file, encounterSets);
            }

            internal override void WriteAssembly(ASMWriter writer, IASMData data)
            {
                EncounterTable encounterTable = (EncounterTable)data;

                foreach (DayTime time in Enum.GetValues<DayTime>())
                {
                    writer.Comment(time.ToString().ToLower());
                    EncounterSet encounters = encounterTable.EncounterSets[time];
                    foreach (Encounter encounter in encounters.Encounters)
                    {
                        writer.DeclareBytes([encounter.MinLevel, encounter.Name]);
                    }
                }
            }

            private static string PercentFromInt(int value)
            {
                return $"{value} percent";
            }
        }
    }
}
