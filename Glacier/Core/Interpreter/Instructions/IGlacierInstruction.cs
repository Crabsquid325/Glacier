namespace Glacier.Core.Interpreter.Instructions;

public interface IGlacierInstruction
{
    public abstract IGlacierObject Exec(ExecutionContext ctx);
}