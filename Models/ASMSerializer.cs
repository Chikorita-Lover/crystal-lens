namespace CrystalLens.Models
{
    public abstract class ASMSerializer
    {
        internal abstract IASMData ReadAssembly(Queue<ASMCommand> commands);

        internal abstract void WriteAssembly(Queue<ASMCommand> commands, IASMData data);
    }
}
