using Glacier.Core.Interpreter.Values;

namespace Glacier.Core.Interpreter.Instructions.Concrete;

public record Block(List<IGlacierInstruction> Instructions) : IGlacierInstruction
{
    public IGlacierObject Exec(ExecutionContext ctx)
    {
        ctx.PushFrame();
        try
        {
            IGlacierObject res = None.Instance;
            foreach (var i in Instructions)
                res = i.Exec(ctx);
            return res;
        }
        finally { ctx.PopFrame(); }
    }

    public override string ToString()
    {
        return "Sequence [" +  string.Join(", ", Instructions) + "]";
    }
}