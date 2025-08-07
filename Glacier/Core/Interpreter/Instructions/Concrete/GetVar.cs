namespace Glacier.Core.Interpreter.Instructions.Concrete;

public record GetVar(string VarName) : IGlacierInstruction
{
    public IGlacierObject Exec(ExecutionContext ctx)
        => ctx.GetVar(VarName);
}