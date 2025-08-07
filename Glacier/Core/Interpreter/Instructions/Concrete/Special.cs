using System.Linq.Expressions;

namespace Glacier.Core.Interpreter.Instructions.Concrete;

public record Special(Expression<Func<ExecutionContext, IGlacierObject>> Expr) : IGlacierInstruction
{
    private readonly Func<ExecutionContext, IGlacierObject> _action = Expr.Compile();
    
    public IGlacierObject Exec(ExecutionContext ctx)
        => _action.Invoke(ctx);

    public override string ToString()
    {
        return Expr.Body.ToString();
    }
}
