using Glacier.Core.Interpreter.Values;

namespace Glacier.Core.Interpreter.Instructions.Concrete;

public record PushVar(string Name, IGlacierInstruction Src) : IGlacierInstruction
{
    public IGlacierObject Exec(ExecutionContext ctx)
    {
        ctx.PushVar(Name, Src.Exec(ctx));
        return None.Instance;
    }
}