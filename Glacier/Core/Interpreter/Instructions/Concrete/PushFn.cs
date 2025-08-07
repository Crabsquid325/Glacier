using Glacier.Core.Interpreter.Values;

namespace Glacier.Core.Interpreter.Instructions.Concrete;

public record PushFn(string Name, FunctionMetadata Metadata) : IGlacierInstruction
{
    public IGlacierObject Exec(ExecutionContext ctx)
    {
        var fn = new FunctionObject(Metadata);
        fn.MakeClosure(ctx);
        ctx.PushVar(Name, fn);
        return None.Instance;
    }
}