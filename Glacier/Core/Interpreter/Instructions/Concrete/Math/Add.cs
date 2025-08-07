using Glacier.Core.Interpreter.Values;

namespace Glacier.Core.Interpreter.Instructions.Concrete.Math;

public record Add(IGlacierInstruction A, IGlacierInstruction B) : IGlacierInstruction
{
    public IGlacierObject Exec(ExecutionContext ctx)
    {
        var a = A.Exec(ctx);
        var b = B.Exec(ctx);
        if (a is Number aNum && b is Number bNum)
            return new Number(aNum.Value + bNum.Value);
        if (a is Keyword aKeyword && b is Keyword bKeyword)
            return new Keyword(aKeyword.Value + bKeyword.Value);
        throw new InvalidOperationException($"Cant add {a.GetType().Name} to {b.GetType().Name}");
    }
}