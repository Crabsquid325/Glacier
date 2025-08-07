namespace Glacier.Core.Interpreter.Instructions.Concrete;

public record Const(IGlacierObject Value) : IGlacierInstruction
{
    public IGlacierObject Exec(ExecutionContext ctx)
        => Value;
}