using Glacier.Core.Interpreter.Values;

namespace Glacier.Core.Interpreter.Instructions.Concrete.Compare;

public record Eq(IGlacierInstruction A, IGlacierInstruction B) : IGlacierInstruction
{
    public IGlacierObject Exec(ExecutionContext ctx)
    {
        var a = A.Exec(ctx);
        var b = B.Exec(ctx);
        if (a is Number aNum && b is Number bNum)
            return new Bool(aNum.Value == bNum.Value);
        if (a is Keyword aKeyword && b is Keyword bKeyword)
            return new Bool(aKeyword.Value == bKeyword.Value);
        if (a is None aNone && b is None bNone)
            return new Bool(true);
        if (a is Bool aBool && b is Bool bBool)
            return new Bool(aBool.Value == bBool.Value);
        
        if (a is not None && b is None || a is None && b is not None)
            return new Bool(false);

        throw new InvalidOperationException($"Cant compare {a.GetType().Name} to {b.GetType().Name}");
    }
}