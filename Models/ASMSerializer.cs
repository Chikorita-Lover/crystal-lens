namespace CrystalLens.Models
{
    public abstract class ASMSerializer
    {
        internal abstract IASMData ReadAssembly(ASMByteReader reader, ASMFile file);

        internal abstract void WriteAssembly(ASMWriter writer, IASMData data);
    }
}
