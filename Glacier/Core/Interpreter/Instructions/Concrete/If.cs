using Glacier.Core.Interpreter.Values;

namespace Glacier.Core.Interpreter.Instructions.Concrete;

public record If(IGlacierInstruction Condition, IGlacierInstruction TrueBlock, IGlacierInstruction? FalseBlock) : IGlacierInstruction
{
    public IGlacierObject Exec(ExecutionContext ctx)
    {
        var condRes = Condition.Exec(ctx);
        bool isTrue = condRes switch
        {
            Bool    bRes => bRes.Value,
            Number  nRes => nRes.Value != 0,
            Keyword kRes => kRes.Value != "",
            None    nRes => false,
            _ => false
        };
        if (isTrue)
            TrueBlock.Exec(ctx);
        else
            FalseBlock?.Exec(ctx);
        return None.Instance;
    }
}