namespace CrystalLens.Models
{
    public interface IASMData
    {
        public ASMFile File { get; }
        public ASMDataType Type { get; }

        public static ASMProject GetProject(IASMData data)
        {
            return data.File.Project;
        }
    }
}
