using Glacier.Core.Interpreter.Values;

namespace Glacier.Core.Interpreter.Instructions.Concrete.Compare;

public record Neq(IGlacierInstruction A, IGlacierInstruction B) : IGlacierInstruction
{
    public IGlacierObject Exec(ExecutionContext ctx)
    {
        return new Bool(!((Bool)new Eq(A, B).Exec(ctx)).Value);
    }
}