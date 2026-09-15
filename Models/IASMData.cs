namespace CrystalLens.Models
{
    public interface IASMData
    {
        public ASMFile File { get; }

        public static ASMProject? GetProject(IASMData data)
        {
            return data.File.Project;
        }

        public static bool HasProject(IASMData data)
        {
            return GetProject(data) != null; 
        }

        ASMSerializer GetSerializer();
    }
}
