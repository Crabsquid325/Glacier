namespace Glacier.Core.Interpreter.Instructions.Concrete.Return;

public record Return(IGlacierInstruction Src) : IGlacierInstruction
{
    public IGlacierObject Exec(ExecutionContext ctx)
    {
        throw new GlacierFuncReturnException(Src.Exec(ctx));
    }
}