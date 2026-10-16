namespace CrystalLens.Models
{
    public class BinaryFileReference : IASMData
    {
        private readonly ASMFile _file;
        public readonly string Path;

        public ASMFile File => _file;
        public ASMDataType Type => ASMDataType.Incbin;

        public BinaryFileReference(ASMFile file, string path)
        {
            _file = file;
            Path = path;
        }

        public class Serializer : ASMSerializer
        {
            internal override IASMData ReadAssembly(ASMReader reader, ASMFile file)
            {
                string path = reader.Read()[1..^1];
                return new BinaryFileReference(file, path);
            }

            internal override void WriteAssembly(ASMWriter writer, IASMData data)
            {
                BinaryFileReference incbin = (BinaryFileReference)data;
                writer.WriteCommand(new("INCBIN", [$"\"{incbin.Path}\""]));
            }
        }
    }
}
