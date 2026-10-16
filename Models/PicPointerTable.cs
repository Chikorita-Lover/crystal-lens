namespace CrystalLens.Models
{
    public class PicPointerTable : IASMData
    {
        private readonly ASMFile _file;
        private readonly List<(string? Frontpic, string? Backpic)> _pictures;

        public ASMFile File => _file;
        public ASMDataType Type => ASMDataType.PicPointers;

        private PicPointerTable(ASMFile file, List<(string?, string?)> pictures)
        {
            _file = file;
            _pictures = pictures;
        }

        public string? GetFrontpicPointer(int index)
        {
            return _pictures[index - 1].Frontpic;
        }

        public string? GetBackpicPointer(int index)
        {
            return _pictures[index - 1].Backpic;
        }

        public class Serializer : ASMSerializer
        {
            internal override IASMData ReadAssembly(ASMReader reader, ASMFile file)
            {
                List<(string?, string?)> pictures = [];
                for (int i = 0; i < 251; i++) // currently hardcoded to read 251 entries
                {
                    string frontpic = reader.Read();
                    string backpic = reader.Read();
                    pictures.Add((frontpic == "-1" ? null : frontpic, backpic == "-1" ? null : backpic));
                }
                return new PicPointerTable(file, pictures);
            }

            internal override void WriteAssembly(ASMWriter writer, IASMData data)
            {
                PicPointerTable pointers = (PicPointerTable)data;
                writer.WriteCommand(new("table_width", ["3 * 2"]));
                foreach ((string? frontpic, string? backpic) in pointers._pictures)
                {
                    if (frontpic == null)
                    {
                        writer.WriteCommand(new("dba_pics"));
                    }
                    else if (backpic == null)
                    {
                        writer.WriteCommand(new("dba_pics", [frontpic]));
                    }
                    else
                    {
                        writer.WriteCommand(new("dba_pics", [frontpic, backpic]));
                    }
                }
            }
        }
    }
}
