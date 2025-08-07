using Glacier.Core.Interpreter.Values;
namespace Glacier.Core.Interpreter.Instructions.Concrete.Compare;

public record Lti(IGlacierInstruction A, IGlacierInstruction B) : IGlacierInstruction
{
    public IGlacierObject Exec(ExecutionContext ctx)
    {
        var a = A.Exec(ctx);
        var b = B.Exec(ctx);
        if (a is Number aNum && b is Number bNum)
            return new Bool(aNum.Value <= bNum.Value);
        if (a is Bool aBool && b is Bool bBool)
            return new Bool((aBool.Value ? 1 : 0) <= (bBool.Value ? 1 : 0));
        
        throw new InvalidOperationException($"Cant compare {a.GetType().Name} to {b.GetType().Name}");
    }
}