namespace CrystalLens.Models
{
    public interface IASMData
    {
        public ASMFile File { get; }

        ASMSerializer GetSerializer();
    }
}
