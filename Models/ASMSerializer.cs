namespace CrystalLens.Models
{
    public abstract class ASMSerializer<T> where T : class
    {
        internal abstract T ReadAssembly(Queue<ASMCommand> commands);

        internal abstract void WriteAssembly(Queue<ASMCommand> commands, T data);
    }
}
