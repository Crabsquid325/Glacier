using Glacier.Core.Interpreter.Values;

namespace Glacier.Core.Interpreter.Instructions.Concrete.Math;

public record Sub(IGlacierInstruction A, IGlacierInstruction B) : IGlacierInstruction
{
    public IGlacierObject Exec(ExecutionContext ctx)
    {
        var a = A.Exec(ctx);
        var b = B.Exec(ctx);
        if (a is Number aNum && b is Number bNum)
            return new Number(aNum.Value - bNum.Value);
        throw new InvalidOperationException($"Cant sub {b.GetType().Name} from {a.GetType().Name}");
    }
}