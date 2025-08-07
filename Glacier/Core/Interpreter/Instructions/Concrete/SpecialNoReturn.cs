using System.Linq.Expressions;
using Glacier.Core.Interpreter.Values;

namespace Glacier.Core.Interpreter.Instructions.Concrete;

public record SpecialNoReturn(Expression<Action<ExecutionContext>> Expr) : IGlacierInstruction
{
    private readonly Action<ExecutionContext> _action = Expr.Compile();

    public IGlacierObject Exec(ExecutionContext ctx)
    {
        _action.Invoke(ctx);
        return None.Instance; 
    }

    public override string ToString()
    {
        return Expr.Body.ToString();
    }
}